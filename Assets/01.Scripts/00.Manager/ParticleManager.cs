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
    /// <param name="position"> 생성할 위치 </param>
    /// <param name="rotation"> 생성할 때 회전 값. 입력안하면 identity </param>
    /// <param name="scale"> 파티클 크기. 입력안하면 1 </param>
    /// <param name="parent"> 부모 객체. 입력안하면 없음 </param>
    /// <returns> 생성한 파티클 반환 </returns>
    public ParticleObject SpawnParticle(string key, Vector3 position, Vector3 scale, Quaternion rotation = default, Transform parent = default)
    {
        if (scale == default)
        {
            scale = new Vector3(1f, 1f, 1f);
        }
        if (rotation == default)
        {
            rotation = Quaternion.identity;
        }
        if (parent == default)
        {
            parent = transform;
        }

        ParticleObject particle = objectPoolManager.GetObject<ParticleObject>(key, position);

        particle.transform.rotation = rotation;
        particle.transform.localScale = scale;
        particle.transform.SetParent(parent);

        return particle;
    }
}
