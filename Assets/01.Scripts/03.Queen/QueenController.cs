using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public enum QueenSlot
{
    MONSTER,
    QueenActiveSkill,
}

public class QueenController : MonoBehaviour
{
    private GameHUD gameHUD;
    public MonsterSlot monsterSlot;
    public QueenActiveSkillSlot queenActiveSkillSlot;
    public int selectedSlotIndex = -1;


    [Header("스킬 범위")]
    public GameObject skillRangeObject;
    public GameObject skillSizeObject;
    public SpriteRenderer skillRangeSprite;
    public SpriteRenderer skillSizeSprite;
    private float skillRangeSpriteRadius;
    private float skillSizeSpriteRadius;

    [Header("내부 값")]
    public Vector3 worldMousePos;
    public GameObject cursorIcon;
    public QueenSlot curSlot = QueenSlot.MONSTER;

    private QueenCondition condition;
    private ObjectPoolManager objectPoolManager;
    private SpriteAtlas atlas;
    private Vector3 castlePos;

    [NonSerialized] public int selectedMonsterId = -1;
    [NonSerialized] public QueenActiveSkillBase selectedQueenActiveSkill;

    private bool isDrag;
    public bool isMinimapDrag;
    //private float summonDistance;
    //private Vector3 lastSummonPosition;

    public ToastMessage toastMessage;

    private float lastSummon;
    private float cooldown;

    private async void Start()
    {
        condition = GameManager.Instance.queen.condition;
        objectPoolManager = ObjectPoolManager.Instance;
        atlas = DataManager.Instance.iconAtlas;
        castlePos = GameManager.Instance.castle.transform.position;

        //summonDistance = 0.5f;
        //lastSummonPosition = Vector3.positiveInfinity;

        skillSizeSpriteRadius = skillSizeSprite.bounds.size.x;
        skillRangeSpriteRadius = skillRangeSprite.bounds.size.x;
        lastSummon = Time.time;
        cooldown = 0.1f;

        await GameHuDInit();
    }

    private void Update()
    {
        ImageFollowCursor();
        RecoveryGauge();
        SkillRangeView();

        if (isDrag)
        {
            SummonMonster();
        }
    }

    private async UniTask GameHuDInit()
    {
        await UniTask.WaitUntil(() => StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>() != null);

        gameHUD = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>();
        monsterSlot = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().monsterSlot;
        queenActiveSkillSlot = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenActiveSkillSlot;
    }

    private void SkillRangeView()
    {
        if (curSlot == QueenSlot.QueenActiveSkill && selectedQueenActiveSkill != null)
        {
            skillSizeObject.SetActive(true);
            skillRangeObject.SetActive(true);

            skillSizeObject.transform.position = worldMousePos;
            skillRangeObject.transform.position = castlePos;

            float skillSize = selectedQueenActiveSkill.info.size / (skillSizeSpriteRadius / 2f);
            float skillRange = selectedQueenActiveSkill.info.range / (skillRangeSpriteRadius / 2f);

            if (selectedQueenActiveSkill.info.range == -1)
            {
                skillRangeObject.transform.localScale = new Vector3(150f, 150f, 1f);
            }
            else
            {
                skillRangeObject.transform.localScale = new Vector3(skillRange, skillRange, 1f);
            }

            if (selectedQueenActiveSkill.info.id == (int)IDQueenActiveSkill.SUMMON_OBSTACLE)
            {
                skillSizeSprite.sprite = atlas.GetSprite("SkillSize_Rect");

                Vector3 mousePos = worldMousePos;
                Vector3 dir = (mousePos - castlePos).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 90f);

                skillSizeObject.transform.localScale = new Vector3(skillSize, 1.5f, 1f);
                skillSizeObject.transform.rotation = rotation;
            }
            else
            {
                skillSizeSprite.sprite = atlas.GetSprite("SkillSize_Circle");
                skillSizeObject.transform.localScale = new Vector3(skillSize, skillSize, 1f);
            }
        }
        else
        {
            skillSizeObject.SetActive(false);
            skillRangeObject.SetActive(false);
        }
    }

    // 선택한 슬롯의 이미지를 마우스커서에 붙힘
    private void ImageFollowCursor()
    {
        Vector2 mousePos = Pointer.current.position.ReadValue();
        worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0f;

        cursorIcon.transform.position = worldMousePos;
    }

    // 번호 키를 누르면 해당 슬롯의 인덱스를 토대로 슬롯 선택
    public void OnPressSlotNumber(InputAction.CallbackContext context)
    {
        if (StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().isPaused)
            return;

        if (context.phase != InputActionPhase.Started)
            return;

        int index = Mathf.RoundToInt(context.ReadValue<float>()) - 1;

        SelectSlot(index);
    }

    // 슬롯 버튼을 클릭했을 때 해당 슬롯 선택
    public void OnClickSlotButton(int index, QueenSlot slotType)
    {
        if (curSlot != slotType)
        {
            return;
        }
        SelectSlot(index);
    }

    // 슬롯에 따라 다른 처리
    public void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        if (curSlot == QueenSlot.MONSTER)
        {
            MonsterInfo monster = monsterSlot.GetValue(index);

            if (monster == null)
            {
                return;
            }

            selectedMonsterId = monster.id;
            var tempMonster = MonsterManager.Instance.monsterInfoList[selectedMonsterId];
            cursorIcon.GetComponent<SpriteRenderer>().sprite = DataManager.Instance.iconAtlas.GetSprite(tempMonster.outfit);
        }
        else if (curSlot == QueenSlot.QueenActiveSkill)
        {
            QueenActiveSkillBase skill = queenActiveSkillSlot.GetValue(index);

            if (skill == null)
            {
                return;
            }

            selectedQueenActiveSkill = skill;

            if (selectedQueenActiveSkill.info.size == -1)
            {
                UseQueenActiveSkill();
                selectedQueenActiveSkill = null;
                return;
            }
        }
    }

    // 클릭 시 처리
    public void OnClick(InputAction.CallbackContext context)
    {
        switch (curSlot)
        {
            // 현재 슬롯이 몬스터 슬롯이면 몬스터 소환
            case QueenSlot.MONSTER:
                if (context.phase == InputActionPhase.Started)
                {
                    isDrag = true;
                    SummonMonster();
                }
                else if (context.phase == InputActionPhase.Canceled)
                {
                    isDrag = false;
                }
                break;
            // 현재 슬롯이 매직 슬롯이면 스킬 발동
            case QueenSlot.QueenActiveSkill:
                if (context.phase == InputActionPhase.Started)
                {
                    UseQueenActiveSkill();
                }
                break;
        }
    }

    //// 드래그 시 처리
    //public void OnDrag(InputAction.CallbackContext context)
    //{
    //    if (!isDrag)
    //    {
    //        return;
    //    }

    //    switch (curSlot)
    //    {
    //        case QueenSlot.MONSTER:
    //            if (context.ReadValue<Vector2>() != Vector2.zero)
    //            {
    //                SummonMonster();
    //            }
    //            break;
    //        case QueenSlot.QueenActiveSkill:
    //            break;
    //    }
    //}

    // 몬스터 소환
    private void SummonMonster()
    {
        if (isMinimapDrag)
        {
            return;
        }

        if (selectedMonsterId == -1)
        {
            return;
        }

        var tempMonster = MonsterManager.Instance.monsterInfoList[selectedMonsterId];

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Time.time - lastSummon < cooldown)
        {
            return;
        }
        lastSummon = Time.time;

        //// 마지막 생성위치에서 일정 거리 이상 떨어져야 소환가능
        //if (Vector3.Distance(worldMousePos, lastSummonPosition) < summonDistance)
        //{
        //    return;
        //}

        if (!SpawnPointManager.Instance.MonsterPoint.IsAreaIn(worldMousePos))
        {
            SpawnPointManager.Instance.MonsterPoint.ShowAndHideAreas();
            return;
        }
        if (condition.CurSummonGauge.Value < tempMonster.cost)
        {
            // 테이블 나오면 적용 필요
            ToastMessage msg = Instantiate(toastMessage, gameHUD.HUDGroup.transform);
            msg.SetText("<color=red>소환 게이지가 부족합니다.</color>");

            return;
        }

        SoundManager.Instance.PlaySFX("SFX_UI_Click_Designed_Liquid_Generic_Open_2");

        // 미니맵콜라이더 레이어를 제외한 레이어와 충돌 처리가 일어나면 몬스터 소환 불가
        //ContactFilter2D layerFilter = new ContactFilter2D();
        //layerFilter.SetLayerMask(~LayerMask.GetMask("MiniMapCollider"));
        //Collider2D[] results = new Collider2D[1];

        //int hitCount = Physics2D.OverlapCircle(worldMousePos, 0.5f, layerFilter, results);

        //if (hitCount > 0)
        //{
        //    return;
        //}

        condition.AdjustCurSummonGauge(-tempMonster.cost);
        var monster = objectPoolManager.GetObject<MonsterController>(tempMonster.outfit, worldMousePos);
        monster.StatInit(tempMonster, MonsterManager.Instance.isHealthUI);

        // 마지막 생성위치 갱신
        //lastSummonPosition = worldMousePos;
    }

    // 퀸의 액티브 스킬 사용
    private async void UseQueenActiveSkill()
    {
        if (selectedQueenActiveSkill == null)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (condition.CurQueenActiveSkillGauge.Value < selectedQueenActiveSkill.info.cost)
        {
            // 테이블 나오면 적용 필요
            ToastMessage msg = Instantiate(toastMessage, gameHUD.HUDGroup.transform);
            msg.SetText("<color=red>마나가 부족합니다.</color>");
            return;
        }
        if (selectedQueenActiveSkill.info.range != -1f)
        {
            if (Vector3.Distance(worldMousePos, GameManager.Instance.castle.transform.position) > selectedQueenActiveSkill.info.range)
            {
                Utils.Log("범위 밖 스킬 사용 불가");
                return;
            }
        }

        await selectedQueenActiveSkill.TryUseSkill(selectedQueenActiveSkill.info.cost);
    }

    // 자동 게이지 회복
    private void RecoveryGauge()
    {
        condition.AdjustCurQueenActiveSkillGauge(condition.QueenActiveSkillGaugeRecoverySpeed * Time.deltaTime);
        condition.AdjustCurSummonGauge(condition.SummonGaugeRecoverySpeed * Time.deltaTime);
    }

    public void OnEvolutionWindow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // 참조값을 비교해서 성능상 빠름
            if (!ReferenceEquals(gameHUD.openWindow, gameHUD.evolutionTreeUI.gameObject))
            {
                gameHUD.ShowWindow<EvolutionTreeUI>();
            }
            else
            {
                gameHUD.HideWindow();
            }
        }
    }

    public void CloseWindow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (ReferenceEquals(gameHUD.openWindow, gameHUD.evolutionTreeUI.gameObject))
            {
                gameHUD.HideWindow();
            }
        }
    }
}