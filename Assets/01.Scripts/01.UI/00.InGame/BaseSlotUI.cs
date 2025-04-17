using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSlotUI<T> : MonoBehaviour
{
    [SerializeField] protected List<Image> slotIcon;

    protected List<T> slotList = new List<T>();

    // 슬롯에 추가
    public virtual void AddSlot(T slot)
    {
        if (slotList.Contains(slot))
        {
            return;
        }

        slotList.Add(slot);
        OnSlotAdd(slotList.Count - 1, slot);
    }

    // T에 맞는 슬롯 추가 처리
    protected abstract void OnSlotAdd(int index, T slot);

    // 슬롯에서 제거
    public virtual void RemoveSlot(T slot)
    {
        int index = slotList.IndexOf(slot);

        if(index >= 0 && index < slotIcon.Count)
        {
            slotIcon[index].sprite = null;
            slotIcon[index].enabled = false;
        }

        slotList.Remove(slot);
    }

    // 슬롯의 번호로 해당 슬롯의 값 가져오기
    public T GetValue(int index)
    {
        return (index >= 0 && index < slotList.Count) ? slotList[index] : default;
    }
}
