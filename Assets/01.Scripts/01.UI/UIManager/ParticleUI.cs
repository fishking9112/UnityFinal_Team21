using System;
using UnityEngine;
using UnityEngine.UI;

public class ParticleUI : MonoBehaviour
{
    public Image iconImg;
    private float startTime = 0.2f;  // 주변으로 랜덤하게 이동
    private float stopTime = 0.8f;   // 멈춤
    private float endTime = 1.2f;    // 지정된 endPos로 이동
    private float randomPosRange = 100.0f;

    private float elapsed = 0f;
    private UIParticleLayer owner;

    private Vector2 startPos;
    private Vector2 randomTargetPos;  // startPos + 랜덤 offset
    private Vector2 endPos;
    private Action endAction;

    public void Init(string imgName, UIParticleLayer owner, Vector2 endPos, Action callback)
    {
        this.owner = owner;
        this.endPos = endPos;
        this.endAction = callback;

        iconImg.sprite = DataManager.Instance.iconAtlas.GetSprite(imgName);
        elapsed = 0f;

        // 현재 위치를 시작점으로 삼음
        startPos = transform.localPosition;

        // 원형 범위 내 랜덤 위치 생성
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-randomPosRange, randomPosRange), UnityEngine.Random.Range(-randomPosRange, randomPosRange));
        randomTargetPos = startPos + randomDirection;

        transform.localScale = Vector2.one;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed < startTime)
        {
            float t = elapsed / startTime;
            transform.localPosition = Vector2.Lerp(startPos, randomTargetPos, t);
        }
        else if (elapsed < stopTime)
        {
            // transform.localPosition = randomTargetPos;
        }
        else if (elapsed < endTime)
        {
            float t = (elapsed - stopTime) / (endTime - stopTime);
            transform.localPosition = Vector2.Lerp(randomTargetPos, endPos, t);
        }
        else
        {
            endAction?.Invoke();
            owner.ReturnToPool(this);
        }
    }
}