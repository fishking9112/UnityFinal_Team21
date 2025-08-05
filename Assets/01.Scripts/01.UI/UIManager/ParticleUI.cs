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
    private Vector2 controlPos; // 베지어 곡선 제어점
    private Action endAction;

    public void Init(string imgName, UIParticleLayer owner, Vector2 endPos, Action callback)
    {
        this.owner = owner;
        this.endPos = endPos;
        this.endAction = callback;

        iconImg.sprite = DataManager.Instance.iconAtlas.GetSprite(imgName);
        elapsed = 0f;

        startPos = transform.localPosition;

        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-randomPosRange, randomPosRange), UnityEngine.Random.Range(-randomPosRange, randomPosRange));
        randomTargetPos = startPos + randomDirection;

        // 제어점을 (시작점의 X, 도착점의 Y)로 설정하여 코너를 도는 듯한 곡선 생성
        controlPos = new Vector2(startPos.x, endPos.y);

        transform.localScale = Vector2.one;
    }

    public void ForceEnd()
    {
        endAction?.Invoke();
        owner.ReturnToPool(this);
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
            // 멈춤
        }
        else if (elapsed < endTime)
        {
            float t = (elapsed - stopTime) / (endTime - stopTime);

            // 2차 베지어 곡선 계산
            Vector2 newPos = (1 - t) * (1 - t) * randomTargetPos +
                             2 * (1 - t) * t * controlPos +
                             t * t * endPos;
            transform.localPosition = newPos;
        }
        else
        {
            endAction?.Invoke();
            owner.ReturnToPool(this);
        }
    }
}