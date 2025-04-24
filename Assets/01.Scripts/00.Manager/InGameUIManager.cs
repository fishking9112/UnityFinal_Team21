using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIManager : MonoSingleton<InGameUIManager>
{
    [Header("UI에 대한 Controller들")]
    public InGameHUDController inGameHUD;
    public QueenEnhanceController queenEnhance;
    public EvolutionTreeController evolutionTree;
    public PauseController pauseController;
    public GameResultController gameResult;



    [Header("현재 상태")]
    public bool isPaused = false;
    [NonSerialized] GameObject openWindow = null;

    /// <summary>
    /// 컨트롤러들 타입을 넣어서 활성화
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller"></param>
    public void ShowWindow<T>(T controller = null) where T : MonoBehaviour
    {
        // 만약 오픈된 창이 있다면 이미 열린 창이 있다고 표시
        if (openWindow != null)
        {
            Utils.Log("이미 열려있는 창이 있습니다");
            return;
        }

        // 타입별로 분리
        if (typeof(T) == typeof(QueenEnhanceController))
        {
            openWindow = queenEnhance.gameObject;
        }
        else if (typeof(T) == typeof(EvolutionTreeController))
        {
            // 다른 타입 처리
            openWindow = evolutionTree.gameObject;
        }
        else if (typeof(T) == typeof(PauseController))
        {
            openWindow = pauseController.gameObject;
        }
        else if (typeof(T) == typeof(GameResultController))
        {
            openWindow = gameResult.gameObject;
        }
        else
        {
            Utils.Log("없는 타입의 Controller입니다.");
            return;
        }

        openWindow.SetActive(true);
        Time.timeScale = 0f; // 시간 멈춤
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideWindow()
    {
        if (openWindow == null) return;

        openWindow.SetActive(false);
        openWindow = null;
        Time.timeScale = 1f; // 시간 흐름
        isPaused = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (pauseController != null)
            {
                ShowWindow(pauseController);
            }
        }
    }

    // 테스트 버튼
    public void OnClickTestLevelUp()
    {
        ShowWindow(queenEnhance);
    }

    public void OnEvolutionWindow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // 참조값을 비교해서 성능상 빠름
            if (!ReferenceEquals(openWindow, evolutionTree.gameObject))
                ShowWindow<EvolutionTreeController>();
            else
                HideWindow();
        }
    }
}
