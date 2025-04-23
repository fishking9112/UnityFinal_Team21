using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InGameHUD))]
public class InGameHUDController : MonoBehaviour
{
    [SerializeField] private InGameHUD hudUI;

    private ReactiveProperty<float> CurTime = new ReactiveProperty<float>();
    private QueenCondition condition;

    private bool isTimeOver = false;
    private float limitTime = 1800f;

    private void Start()
    {
        hudUI = GetComponent<InGameHUD>();
        Init();
    }

    private void Init()
    {
        condition = GameManager.Instance.queen.condition;

        condition.Level.AddAction(hudUI.UpdateLevelText);
        hudUI.UpdateLevelText(condition.Level.Value);

        condition.Gold.AddAction(hudUI.UpdateGoldText);
        hudUI.UpdateGoldText(condition.Gold.Value);

        hudUI.BindGauges(condition);

        hudUI.BindTimer(CurTime);
        CurTime.Value = limitTime;

        hudUI.PauseButton(() => InGameUIManager.Instance.ShowWindow<PauseController>());
    }

    private void Update()
    {
        OnTimer();
    }

    private void OnTimer()
    {
        if (isTimeOver) return;

        CurTime.Value -= Time.deltaTime;

        if (CurTime.Value <= 0f)
        {
            CurTime.Value = 0f;
            isTimeOver = true;
            StageClear();
        }
    }

    private void StageClear()
    {
        // 제한 시간 동안 버텼을 경우
    }


    private void OnDestroy()
    {
        condition.Level.RemoveAction(hudUI.UpdateLevelText);
        condition.Gold.RemoveAction(hudUI.UpdateGoldText);
    }

    public float GetTimer() => CurTime.Value;

}