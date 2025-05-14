using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueenSelectUI : MonoBehaviour
{
    public List<Toggle> queenSelectToggleList;
    public int toggleIndex = -1;

    public void Start()
    {
        // TODO : 게임 시작 버튼 초기화
        for (int i = 0; i < queenSelectToggleList.Count; i++)
        {
            int index = i; // 클로저 캡처 방지용
            queenSelectToggleList[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    SelectToggle(index);
                }
            });
        }

        // 첫번째 무조건 선택
        queenSelectToggleList[0].isOn = true;
    }

    public void SelectToggle(int index)
    {
        // TODO : 나중에 어떻게 토글을 이용해 퀸 스텟 바뀔 지 쓰기
        toggleIndex = index;
    }

}
