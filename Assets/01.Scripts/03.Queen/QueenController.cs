using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum QueenSlot
{
    MONSTER,
    QueenActiveSkill,
}

public class QueenController : MonoBehaviour
{
    private QueenCondition condition;
    private ObjectPoolManager objectPoolManager;

    public Vector3 worldMousePos;

    public MonsterSlotUI monsterSlotUI;
    public QueenActiveSkillSlotUI queenActiveSkillSlotUI;

    [NonSerialized] public MonsterInfo selectedMonster;
    [NonSerialized] public QueenActiveSkillBase selectedQueenActiveSkill;
    public GameObject cursorIcon;
    public QueenSlot curSlot = QueenSlot.MONSTER;

    public List<MonsterInfo> monsterList;

    private bool isDrag;

    [SerializeField] private float summonDistance = 0.5f;
    private Vector3 lastSummonPosition = Vector3.positiveInfinity;

    public List<QueenActiveSkillBase> queenActiveSkillList = new List<QueenActiveSkillBase>();

    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
        objectPoolManager = ObjectPoolManager.Instance;

        //테스트 코드
        queenActiveSkillSlotUI.AddSlot(0, queenActiveSkillList[0]);
    }

    private void Update()
    {
        ImageFollowCursor();
        RecoveryGauge();
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
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }

        int index = Mathf.RoundToInt(context.ReadValue<float>()) - 1;

        SelectSlot(index);
    }

    // 슬롯 버튼을 클릭했을 때 해당 슬롯 선택
    public void OnClickSlotButton(int index)
    {
        SelectSlot(index);
    }

    // 슬롯에 따라 다른 처리
    public void SelectSlot(int index)
    {
        if (curSlot == QueenSlot.MONSTER)
        {
            MonsterInfo monster = monsterSlotUI.GetValue(index);

            if (monster == null)
            {
                return;
            }

            selectedMonster = monster;
            cursorIcon.GetComponent<SpriteRenderer>().sprite = DataManager.Instance.iconData.GetSprite(selectedMonster.outfit);
        }
        else if (curSlot == QueenSlot.QueenActiveSkill)
        {
            QueenActiveSkillBase skill = queenActiveSkillSlotUI.GetValue(index);

            if(skill == null)
            {
                return;
            }

            selectedQueenActiveSkill = skill;
            //스킬 아이콘 처리
            cursorIcon.GetComponent<SpriteRenderer>().sprite = DataManager.Instance.iconData.GetSprite(selectedQueenActiveSkill.outfit);
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

    // 드래그 시 처리
    public void OnDrag(InputAction.CallbackContext context)
    {
        if (!isDrag)
        {
            return;
        }

        switch (curSlot)
        {
            case QueenSlot.MONSTER:
                if (context.ReadValue<Vector2>() != Vector2.zero)
                {
                    SummonMonster();
                }
                break;
            case QueenSlot.QueenActiveSkill:
                break;
        }
    }

    // 몬스터 소환
    private void SummonMonster()
    {
        if (selectedMonster == null)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (condition.CurSummonGauge.Value < selectedMonster.cost)
        {
            return;
        }
        // 마지막 생성위치에서 일정 거리 이상 떨어져야 소환가능
        if(Vector3.Distance(worldMousePos,lastSummonPosition) < summonDistance)
        {
            return;
        }

        // 커서, 미니맵콜라이더 레이어를 제외한 레이어와 충돌 처리가 일어나면 몬스터 소환 불가
        ContactFilter2D layerFilter = new ContactFilter2D();
        layerFilter.SetLayerMask(~LayerMask.GetMask("Cursor", "MiniMapCollider"));
        Collider2D[] results = new Collider2D[1];

        int hitCount = Physics2D.OverlapCircle(worldMousePos, 0.5f, layerFilter, results);

        if (hitCount > 0)
        {
            return;
        }

        condition.AdjustCurSummonGauge(-selectedMonster.cost);
        var monster = objectPoolManager.GetObject<MonsterController>(selectedMonster.outfit, worldMousePos);
        monster.StatInit(selectedMonster);

        // 마지막 생성위치 갱신
        lastSummonPosition = worldMousePos;
    }

    // 퀸의 액티브 스킬 사용
    private void UseQueenActiveSkill()
    {
        if (selectedQueenActiveSkill == null)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (condition.CurQueenActiveSkillGauge.Value < selectedQueenActiveSkill.cost)
        {
            return;
        }

        condition.AdjustCurQueenActiveSkillGauge(-selectedQueenActiveSkill.cost);
        selectedQueenActiveSkill.UseSkill();
    }

    // 자동 게이지 회복
    private void RecoveryGauge()
    {
        condition.AdjustCurQueenActiveSkillGauge(condition.QueenActiveSkillGaugeRecoverySpeed * Time.deltaTime);
        condition.AdjustCurSummonGauge(condition.SummonGaugeRecoverySpeed * Time.deltaTime);
    }
}