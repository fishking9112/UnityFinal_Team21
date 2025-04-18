using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultManager : MonoSingleton<GameResultManager>
{
    private GameResultUI resultUI;
    public GameResultUI ResultUI => resultUI;

    /// <summary>
    /// UI 패널 스크립트 등록
    /// </summary>
    /// <param name="script">강화 UI 컨트롤러</param>
    public void SetUI(GameResultUI resultUI)
    {
        this.resultUI = resultUI;
    }
    
    /// <summary>
    /// 게임 종료 후 결과창을 띄웁니다.
    /// </summary>
    public void DisplayGameResult()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        ResultUI.resultWindow.SetActive(true);  

    }

    /// <summary>
    /// 게임이 종료 되었으니 메인씬으로 이동합니다.
    /// </summary>
    public void ReturnToTitle()
    {
        SceneLoadManager.Instance.LoadScene("MenuScene");
    }
}
