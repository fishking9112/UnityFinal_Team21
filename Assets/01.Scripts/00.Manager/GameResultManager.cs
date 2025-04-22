using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResultUnitData
{
    public int spawnCount;
    public float allDamage;
}

public class GameResultManager : MonoSingleton<GameResultManager>
{
    private GameResultUI resultUI;
    public GameResultUI ResultUI => resultUI;

    public Dictionary<int, ResultUnitData> resultDatas = new();

    /// <summary>
    /// UI 패널 스크립트 등록
    /// </summary>
    /// <param name="script">UI 컨트롤러</param>
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
        ResultUI.ShowUnitResult();
    }

    /// <summary>
    /// 게임이 종료 되었으니 메인씬으로 이동합니다.
    /// </summary>
    public void ReturnToTitle()
    {
        SceneLoadManager.Instance.LoadScene("MenuScene");
    }
}
