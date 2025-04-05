using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveProperty<T> where T : struct
{
    private T value;

    private Action<T> actions;

    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
            actions?.Invoke(this.value);   
        }
    }

    public void AddAction(Action<T> action)
    {
        this.actions += action;
    }

    // 수정 필요
    public void RemoveAction(Action<T> action)
    {
        this.actions -= action;
    }

    private void OnDestroy()
    {
        actions = null;
    }
}
