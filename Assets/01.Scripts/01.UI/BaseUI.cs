using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    /// <summary>
    /// UI 초기화 (1회만 호출)
    /// </summary>
    public virtual void Initialize()
    {
        // Debug.Log($"{gameObject.name} Initialized");
    }

    /// <summary>
    /// UI가 활성화될 때 호출
    /// </summary>
    public virtual void OnShow()
    {
        // Debug.Log($"{gameObject.name} Shown");
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UI가 비활성화될 때 호출
    /// </summary>
    public virtual void OnHide()
    {
        // Debug.Log($"{gameObject.name} Hidden");
        gameObject.SetActive(false);
    }
}
