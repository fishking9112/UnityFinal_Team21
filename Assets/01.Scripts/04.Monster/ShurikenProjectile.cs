using UnityEngine;

public class ShurikenProjectile : MonsterProjectileObject
{
    [SerializeField] private float rotationSpeed = 1080f;

    protected override void Update()
    {
        base.Update();

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
