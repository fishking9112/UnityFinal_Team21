using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    private ObjectPoolManager objectPoolManager;

    protected override void Awake()
    {
        base.Awake();

        objectPoolManager = ObjectPoolManager.Instance;
    }

    /// <summary>
    /// 파티클 생성
    /// </summary>
    /// <param name="key"> 생성할 파티클 </param>
    /// <param name="position"> 생성할 위치 </param>
    /// <param name="rotation"> 생성할 때 회전 값. 입력안하면 identity </param>
    /// <param name="scale"> 파티클 크기. 입력안하면 1 </param>
    /// <param name="parent"> 부모 객체. 입력안하면 없음 </param>
    /// <returns> 생성한 파티클 반환 </returns>
    public ParticleObject SpawnParticle(string key, Vector2 position, Quaternion rotation = default, float scale = 1f, Transform parent = default)
    {
        if(rotation == default)
        {
            rotation = Quaternion.identity;
        }
        if(parent == default)
        {
            parent = transform;
        }

        ParticleObject particle = objectPoolManager.GetObject<ParticleObject>(key, position);

        particle.transform.rotation = rotation;
        particle.transform.localScale = new Vector2(scale, scale);
        particle.transform.SetParent(parent);

        return particle;
    }
}
