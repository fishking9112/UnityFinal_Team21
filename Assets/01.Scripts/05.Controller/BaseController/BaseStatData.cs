
public class BaseStatData
{
    public float health;
    public float defence;
    public float moveSpeed;
    public float attack;
    public float attackRange;
    public float attackSpeed;

    public BaseStatData() { }

    public BaseStatData(BaseStatData other)
    {
        health = other.health;
        defence = other.defence;
        moveSpeed = other.moveSpeed;
        attack = other.attack;
        attackRange = other.attackRange;
        attackSpeed = other.attackSpeed;
    }
}