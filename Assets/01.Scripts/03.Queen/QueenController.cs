using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [SerializeField] private MonsterSlotUI monsterSlot;
    [SerializeField] private MagicSlotUI magicSlot;

    [SerializeField] private float summonGaugeRecoverySpeed = 10f;
    [SerializeField] private float magicGaugeRecoverySpeed = 5f;

    public string selectedSlotName;
    public GameObject cursorIcon;
    public QueenSlot curSlot = QueenSlot.MONSTER;

    public Sprite[] test;


    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
        objectPoolManager = ObjectPoolManager.Instance;

        // 테스트용 몬스터 추가
        monsterSlot.AddSlot("Circle", test[0]);
        monsterSlot.AddSlot("Capsule", test[1]);
        monsterSlot.AddSlot("Hexagon Flat-Top", test[2]);
    }

    private void Update()
    {
        switch (curSlot)
        {
            case QueenSlot.MONSTER:
                SummonMonster();
                break;
            case QueenSlot.MAGIC:
                UseMagic();
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
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }

        int index = Mathf.RoundToInt(context.ReadValue<float>()) - 1;

        BaseSlotUI curBaseSlotUI = curSlot == QueenSlot.MONSTER ? monsterSlot : magicSlot;
        selectedSlotName = curBaseSlotUI.GetKey(index);

        if (selectedSlotName == null)
        {
            return;
        }

        cursorIcon.GetComponent<SpriteRenderer>().sprite = curBaseSlotUI.GetIcon(selectedSlotName);
    }

    // 마우스의 월드좌표를 계산해서 해당 위치에 몬스터를 소환함
    private void SummonMonster()
    {
        if (!Pointer.current.press.isPressed)
        {
            return;
        }
        if (selectedSlotName == null)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        float monsterRadius = 0.5f;
        // CameraLimit 레이어만 제외하고 충돌 하도록 함
        int layerMask = ~(1 << (LayerMask.NameToLayer("CameraLimit")));

        Collider2D hit = Physics2D.OverlapCircle(worldMousePos, monsterRadius, layerMask);

        if (hit != null)
        {
            return;
        }

        objectPoolManager.GetObject(selectedSlotName, worldMousePos);
    }


    private void UseMagic()
    {
        if (!Pointer.current.press.isPressed)
        {
            return;
        }
        if (selectedSlotName == null)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 슬롯에 따라 다른 권능 구현
    }

    // 자동 게이지 회복
    private void RecoveryGauge()
    {
        condition.AdjustCurMagicGauge(magicGaugeRecoverySpeed * Time.deltaTime);
        condition.AdjustCurSummonGauge(summonGaugeRecoverySpeed * Time.deltaTime);
    }
}