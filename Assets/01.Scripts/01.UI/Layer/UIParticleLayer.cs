using System;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleLayer : MonoBehaviour
{
    public ParticleUI uiParticlePrefab;
    public int initialPoolSize = 10;
    private List<ParticleUI> poolList = new();

    void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewInstance();
        }
    }

    public void ShowParticle(string imgName, Vector3 startPos, Vector2 endPos, Action callback = null)
    {
        ParticleUI ui = GetFromPool();
        ui.transform.position = startPos;
        ui.Init(imgName, this, endPos, callback); // DamageLayer를 넘겨줌
        ui.gameObject.SetActive(true);
    }

    private ParticleUI GetFromPool()
    {
        foreach (var item in poolList)
        {
            if (!item.gameObject.activeInHierarchy)
                return item;
        }

        return CreateNewInstance();
    }

    private ParticleUI CreateNewInstance()
    {
        ParticleUI newObj = Instantiate(uiParticlePrefab, transform);
        newObj.gameObject.SetActive(false);
        poolList.Add(newObj);
        return newObj;
    }

    public void ReturnToPool(ParticleUI ui)
    {
        ui.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (var item in poolList)
        {
            if (item.gameObject.activeInHierarchy)
            {
                item.ForceEnd();
            }
        }
    }
}
