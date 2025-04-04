using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGSSave : MonoBehaviour
{
    public void SaveData(string key, string jsonData)
    {
        // Cloud Save API 호출
    }

    public void LoadData(string key, Action<string> onSuccess)
    {
        // 데이터 불러오기
    }
}
