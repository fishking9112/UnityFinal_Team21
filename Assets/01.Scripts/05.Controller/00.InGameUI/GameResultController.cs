using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameResultUnitData
{
    public int spawnCount;
    public float allDamage;
}

[RequireComponent(typeof(GameResultUI))]
public class GameResultController : MonoBehaviour
{
    public GameResultUI resultUI;

    public Dictionary<int, GameResultUnitData> resultDatas = new();

    public void Start()
    {
        resultUI.titleMenuBtn.onClick.AddListener(ReturnToTitle);
    }

    /// <summary>
    /// 게임 종료 후 결과창을 띄웁니다.
    /// </summary>
    public void OnEnable()
    {
        resultUI.InitMiddlePanel();
        resultUI.InitUnitResult();
    }

    /// <summary>
    /// 게임이 종료 되었으니 메인씬으로 이동합니다.
    /// </summary>
    public void ReturnToTitle()
    {
        SceneLoadManager.Instance.LoadScene("MenuScene");
    }
}
