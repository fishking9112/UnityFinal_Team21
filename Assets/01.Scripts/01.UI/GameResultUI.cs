using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class GameResultUI : MonoBehaviour
{
    public GameObject resultWindow;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI resourceText;

    /// <summary>   
    /// 시작 시 UI를 초기화합니다.
    /// </summary>
    private void Start()
    {
        GameResultManager.Instance.SetUI(this);
    }

    public void OnClickTitleMenu()
    {
        GameResultManager.Instance.ReturnToTitle();
    }
}
