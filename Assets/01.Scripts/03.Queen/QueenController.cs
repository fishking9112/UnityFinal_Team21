using System;
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

    public MonsterSlotUI monsterSlotUI;
    public MagicSlotUI magicSlotUI;

    public Magic selectedMagic;
    [NonSerialized] public MonsterInfo selectedMonster;
    public GameObject cursorIcon;
    public QueenSlot curSlot = QueenSlot.MONSTER;

    public List<MonsterInfo> monsterList;

    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
        objectPoolManager = ObjectPoolManager.Instance;
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
            MonsterInfo monster = monsterSlotUI.GetValue(index);

            if (monster == null)
            {
                return;
            }

            selectedMonster = monster;
            cursorIcon.GetComponent<SpriteRenderer>().sprite = DataManager.Instance.iconData.GetSprite(selectedMonster.outfit);
        }
        else if (curSlot == QueenSlot.MAGIC)
        {
            // 매직 슬롯일 경우 처리
        }
    }

    // 마우스의 월드좌표를 계산해서 해당 위치에 몬스터를 소환함
    private void SummonMonster()
    {
        if (selectedMonster == null)
        {
            return;
        }
        if (!Pointer.current.press.isPressed)
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

        Physics2D.SyncTransforms();
        Collider2D hit = Physics2D.OverlapCircle(worldMousePos, monsterRadius, layerMask);

        if (hit != null)
        {
            return;
        }

        condition.AdjustCurSummonGauge(-selectedMonster.cost);
        var monster = objectPoolManager.GetObject<MonsterController>(selectedMonster.outfit, worldMousePos);
        monster.StatInit(selectedMonster);
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
        condition.AdjustCurMagicGauge(condition.MagicGaugeRecoverySpeed * Time.deltaTime);
        condition.AdjustCurSummonGauge(condition.SummonGaugeRecoverySpeed * Time.deltaTime);
    }
}