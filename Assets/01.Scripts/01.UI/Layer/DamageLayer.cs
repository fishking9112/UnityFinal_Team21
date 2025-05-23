using System.Collections.Generic;
using UnityEngine;

public class DamageLayer : MonoBehaviour
{
    public DamageUI damageTextPrefab;
    public int initialPoolSize = 10;
    private List<DamageUI> poolList = new();

    void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewInstance();
        }
    }

    public void ShowDamage(float damage, Vector3 position, float fontSize = 0.3f)
    {
        DamageUI ui = GetFromPool();
        ui.transform.position = position;
        ui.Init(damage, this, fontSize); // DamageLayer를 넘겨줌
        ui.gameObject.SetActive(true);
    }

    private DamageUI GetFromPool()
    {
        foreach (var item in poolList)
        {
            if (!item.gameObject.activeInHierarchy)
                return item;
        }

        return CreateNewInstance();
    }

    private DamageUI CreateNewInstance()
    {
        DamageUI newObj = Instantiate(damageTextPrefab, transform);
        newObj.gameObject.SetActive(false);
        poolList.Add(newObj);
        return newObj;
    }

    public void ReturnToPool(DamageUI ui)
    {
        ui.gameObject.SetActive(false);
    }
}
