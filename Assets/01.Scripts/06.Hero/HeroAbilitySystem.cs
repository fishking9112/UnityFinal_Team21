using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


public abstract class HeroAbilitySystem : MonoBehaviour
{
    protected GameObject target;

    protected float delayTime = 1f;

    /// <summary>
    /// 능력 획득
    /// </summary>
    protected virtual void AddAbility()
    {
        AutoAction().Forget();
    }


    /// <summary>
    /// delayTime간격으로 ActionAbility 호출
    /// </summary>
    /// <returns></returns>
    protected async UniTaskVoid AutoAction() 
    {

        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
            ActionAbility();
        }

    }

    /// <summary>
    /// 실제 하는 행동 호출
    /// </summary>
    protected abstract void ActionAbility();

    public abstract void AbilityLevelUp(int nowLv);

}
