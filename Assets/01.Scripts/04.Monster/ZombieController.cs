using UnityEngine;

public class ZombieController : MonsterController
{
    [Header("좀비 설정")]
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float explosionSize = 3f;
    [SerializeField] private float explosionDamage = 50f;

    private string die = "Die";
    private bool isDie;

    public override void OnSpawn()
    {
        base.OnSpawn();

        isDie = false;
        Invoke(die, 5f);
    }


    public override void Die()
    {
        if (isDie)
        {
            return;
        }

        isDie = true;

        base.Die();

        ParticleObject explosionParticle = ParticleManager.Instance.SpawnParticle("Zombie_Explosion", transform.position, new Vector3(0.7f, 0.7f, 1f));

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionSize, attackLayer);

        foreach (var hit in hits)
        {
            if (HeroManager.Instance.hero.TryGetValue(hit.gameObject, out var hero))
            {
                hero.TakeDamaged(explosionDamage);
            }
        }
    }

    // 테스트 코드
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f); // 주황색 반투명
        Gizmos.DrawWireSphere(transform.position, explosionSize);
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, explosionSize);
    }
}
