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
        Invoke(die, lifeTime);
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
}
