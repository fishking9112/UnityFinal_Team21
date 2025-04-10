using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseSlotUI : MonoBehaviour
{
    [SerializeField] protected List<Image> slotIcon;

    protected List<string> keyList = new List<string>();
    protected Dictionary<string, Sprite> iconMap = new Dictionary<string, Sprite>();

    public virtual void AddSlot(string key, Sprite sprite)
    {
        keyList.Add(key);
        iconMap[key] = sprite;

        int index = keyList.Count - 1;

        if(index < slotIcon.Count)
        {
            slotIcon[index].sprite = sprite;
            slotIcon[index].enabled = true;
            slotIcon[index].preserveAspect = true;
        }
    }

    public virtual void RemoveSlot(string key)
    {
        int index = keyList.IndexOf(key);

        if(index >=0 && index < slotIcon.Count)
        {
            slotIcon[index].sprite = null;
            slotIcon[index].enabled = false;
        }

        keyList.Remove(key);
        iconMap.Remove(key);
    }

    public string GetKey(int index)
    {
        return (index >= 0 && index < keyList.Count) ? keyList[index] : null;
    }

    public Sprite GetIcon(string key)
    {
        return iconMap.TryGetValue(key, out Sprite sprite) ? sprite : null;
    }
}
