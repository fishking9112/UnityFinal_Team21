using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueenSelectUI : MonoBehaviour
{
    public List<Button> queenSelectButtonList;

    public void Start()
    {
        // TODO : 게임 시작 버튼 초기화
        foreach (var button in queenSelectButtonList)
        {
            button.onClick.AddListener(() =>
            {
                SceneLoadManager.Instance.LoadScene("GameScene").Forget();
            });
        }
    }
}
