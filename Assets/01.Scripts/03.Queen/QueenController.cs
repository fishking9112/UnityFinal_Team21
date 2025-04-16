using System;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[Serializable]
public class TestMonster
{
    public int id;
    public string name;
    public float cost;
    public Sprite icon;
    public GameObject prefabs;
}

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

    [SerializeField] private MonsterSlotUI monsterSlotUI;
    [SerializeField] private MagicSlotUI magicSlotUI;

    [SerializeField] private float summonGaugeRecoverySpeed = 10f;
    [SerializeField] private float magicGaugeRecoverySpeed = 5f;

    public Magic selectedMagic;
    public TestMonster selectedMonster;
    public GameObject cursorIcon;
    public QueenSlot curSlot = QueenSlot.MONSTER;

    public TestMonster[] testMonster;


    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
        objectPoolManager = ObjectPoolManager.Instance;

        // 테스트용 몬스터 추가
        foreach (var monster in testMonster)
        {
            monsterSlotUI.AddSlot(monster);
        }
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

    // 슬롯에 따라 다른 처리
    public void SelectSlot(int index)
    {
        if (curSlot == QueenSlot.MONSTER)
        {
            BaseSlotUI<TestMonster> curBaseSlotUI = monsterSlotUI;
            TestMonster monster = curBaseSlotUI.GetKey(index);

            if (monster == null)
            {
                return;
            }
            selectedMonster = monster;
            cursorIcon.GetComponent<SpriteRenderer>().sprite = selectedMonster.icon;
        }
        else if (curSlot == QueenSlot.MAGIC)
        {
            // 매직 슬롯일 경우 처리
        }
    }

    // 마우스의 월드좌표를 계산해서 해당 위치에 몬스터를 소환함
    private void SummonMonster()
    {
        if (!Pointer.current.press.isPressed)
        {
            return;
        }
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

        float monsterRadius = 0.5f;

        // CameraLimit 레이어만 제외하고 충돌 하도록 함
        int layerMask = ~(1 << (LayerMask.NameToLayer("CameraLimit")));

        Collider2D hit = Physics2D.OverlapCircle(worldMousePos, monsterRadius, layerMask);

        if (hit != null)
        {
            return;
        }

        condition.AdjustCurSummonGauge(-selectedMonster.cost);
        //objectPoolManager.GetObject(selectedMonster.name, worldMousePos);
    }


    private void UseMagic()
    {
        if (!Pointer.current.press.isPressed)
        {
            return;
        }
        if (selectedMagic == null)
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