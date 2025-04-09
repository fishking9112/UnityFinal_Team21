using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum QueenSlot
{
    MONSTER,
    MAGIC,
}

public class QueenController : MonoBehaviour
{
    private QueenCondition condition;
    private ObjectPoolManager objectPoolManager;

    private Vector3 worldMousePos;
    private List<string> monsterSlot = new List<string>();
    private Dictionary<string, Sprite> monsterSlotIcon = new Dictionary<string, Sprite>();

    private string selectedSlotName;

    [SerializeField] private float summonGaugeRecoverySpeed = 10f;
    [SerializeField] private float magicGaugeRecoverySpeed = 5f;

    public GameObject cursorIcon;

    public QueenSlot slot = QueenSlot.MONSTER;

    public Sprite[] test;


    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
        objectPoolManager = ObjectPoolManager.Instance;

        AddMonsterToSlot("Circle", test[0]);
        AddMonsterToSlot("Capsule", test[1]);
        AddMonsterToSlot("Hexagon Flat-Top", test[2]);
    }

    private void Update()
    {
        switch (slot)
        {
            case QueenSlot.MONSTER:
                SummonMonster();
                break;
            case QueenSlot.MAGIC:
                break;
        }
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

    /// <summary>
    /// 번호 키를 누르면 해당 슬롯에 저장되어 있는 몬스터를 소환할 준비
    /// </summary>
    /// <param name="context"> 1,2,3,4,5,6번 키 </param>
    public void OnPressSlotNumber(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (slot == QueenSlot.MONSTER)
            {
                int key = Mathf.RoundToInt(context.ReadValue<float>());

                if (key > monsterSlot.Count)
                {
                    return;
                }

                selectedSlotName = monsterSlot[key - 1];
                cursorIcon.GetComponent<SpriteRenderer>().sprite = monsterSlotIcon[monsterSlot[key - 1]];
            }
            else if (slot == QueenSlot.MAGIC)
            {

            }
        }
    }

    // 마우스의 월드좌표를 계산해서 해당 위치에 몬스터를 소환함
    private void SummonMonster()
    {
        if (Pointer.current.press.isPressed)
        {
            if (selectedSlotName == null)
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            float monsterRadius = 0.5f;

            int layerMask = ~(1 << (LayerMask.NameToLayer("CameraLimit")));


            Collider2D hit = Physics2D.OverlapCircle(worldMousePos, monsterRadius, layerMask);

            if (hit != null)
            {
                return;
            }

            objectPoolManager.GetObject(selectedSlotName, worldMousePos);
        }
    }

    //public void AddMonsterToSlot(Monster monster)
    //{
    //    monsterSlot.Add(monster.monsterData.name);
    //    monsterPrefabs.Add(monster.monsterData.name, monster.monsterData.outfit);
    //}

    //public void RemoveMonsterFromSlot(Monster monster)
    //{
    //    monsterSlot.Remove(monster.monsterData.name);
    //    monsterPrefabs.Remove(monster.monsterData.name);
    //}

    // 슬롯에 몬스터 추가
    public void AddMonsterToSlot(string key, Sprite sprite)
    {
        monsterSlot.Add(key);
        monsterSlotIcon.Add(key, sprite);
    }

    // 슬롯에서 몬스터 제거
    public void RemoveMonsterFromSlot(string key)
    {
        monsterSlot.Remove(key);
        monsterSlotIcon.Remove(key);
    }

    // 자동 게이지 회복
    private void RecoveryGauge()
    {
        condition.AdjustCurMagicGauge(magicGaugeRecoverySpeed * Time.deltaTime);
        condition.AdjustCurSummonGauge(summonGaugeRecoverySpeed * Time.deltaTime);
    }


    private bool IsPointerOverUIObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0;
    }
}