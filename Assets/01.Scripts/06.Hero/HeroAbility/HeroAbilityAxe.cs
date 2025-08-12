using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class HeroAbilityAxe : HeroAbilitySystem
{
    private ObjectPoolManager _objectPoolManager;
    private Hero _hero;

    private void OnEnable()
    {
        // 객체가 활성화될 때 호출됩니다.
        Initialize((int)IDHeroAbility.AXE);
        
        _hero = GetComponent<Hero>();
        _objectPoolManager = ObjectPoolManager.Instance;
        
        // 취소 토큰을 새로고침합니다.
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();
    }

    protected override void ActionAbility()
    {
        // 히어로가 없거나 작업이 취소된 경우 일찍 종료
        if (_hero == null || token.IsCancellationRequested)
        {
            return;
        }

        // 타겟을 찾고, 유효하고 활성화된 경우에만 진행
        target = _hero.FindNearestTarget();
        if (target != null && target.activeInHierarchy)
        {
            ShootAxe(token.Token).Forget();
        }
    }

    private async UniTaskVoid ShootAxe(CancellationToken cancellationToken)
    {
        // 비동기 메서드 내에서 세 번 확인하여 안전을 보장
        // 작업이 실행되는 동안 히어로 또는 타겟이 파괴될 수 있기에 예외처리
        if (this == null || _hero == null || target == null || !target.activeInHierarchy || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        // 로직 시작 시 위치를 안전하게 캡처
        Vector3 heroPosition = _hero.transform.position;
        Vector3 targetPosition = target.transform.position;

        float angle = Mathf.Atan2(targetPosition.y - heroPosition.y, targetPosition.x - heroPosition.x) * Mathf.Rad2Deg;

        for (int i = 0; i < count; i++)
        {
            // 이 비동기 루프의 각 반복에서 히어로와 토큰을 다시 확인
            if (_hero == null || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var bullet = _objectPoolManager.GetObject<HeroBullet>("axe", _hero.transform.position);
            if (bullet != null)
            {
                bullet.SetBullet(duration, pierce, damage, speed, rotateSpeed, size, knockback);
                bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(delay), false, PlayerLoopTiming.Update, cancellationToken: cancellationToken);
        }
    }

    public override void DespawnAbility()
    {
        // 진행 중인 비동기 작업을 취소하고 정리
        token?.Cancel();
        token?.Dispose();
        token = null;
        
        // 컴포넌트를 비활성화
        this.enabled = false;
    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();
    }

    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
        // 레벨 변경 시 어빌리티 로직을 다시 시작하기 위해 토큰을 재설정
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();
    }
}

