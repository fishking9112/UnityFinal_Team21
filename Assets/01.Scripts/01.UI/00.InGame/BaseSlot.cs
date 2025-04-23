using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSlot<T> : MonoBehaviour
{
    [SerializeField] protected List<Image> slotIcon;

    protected Dictionary<int, T> slotDic = new Dictionary<int, T>();

    // 슬롯에 추가
    public virtual void AddSlot(int index, T slot)
    {
        slotDic[index] = slot;
    }

    // 슬롯에서 제거
    public virtual void RemoveSlot(int index)
    {
        if (slotDic.ContainsKey(index))
        {
            slotDic.Remove(index);

            if (index >= 0 && index < slotIcon.Count)
            {
                slotIcon[index].sprite = null;
                slotIcon[index].enabled = false;
            }
        }
    }

    // 슬롯의 번호로 해당 슬롯의 값 가져오기
    public T GetValue(int index)
    {
        return slotDic.ContainsKey(index) ? slotDic[index] : default;
    }
}
