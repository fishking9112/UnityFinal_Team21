using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveAchievement : MonoBehaviour
{
    public async UniTask LoadAchievementsAsync()
    {
        Utils.Log("업적 정보 불러오기 (향후 구현 예정)");
        await UniTask.Yield();
    }
}
