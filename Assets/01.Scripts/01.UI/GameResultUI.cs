using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class GameResultUI : MonoBehaviour
{
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI resourceText;

    /// <summary>   
    /// 시작 시 매니저가 생성될 때까지 대기한 뒤 UI를 초기화합니다.
    /// </summary>
    private void Start()
    {
        GameResultManager.Instance.SeUI(this);
    }

    public void OnClickTitleMenu()
    {
        SceneLoadManager.Instance.LoadScene("MenuScene");
    }
}
