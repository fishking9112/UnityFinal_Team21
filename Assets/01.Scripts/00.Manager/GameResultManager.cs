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
    public void SeUI(GameResultUI resultUI)
    {
        this.resultUI = resultUI;
    }
}
