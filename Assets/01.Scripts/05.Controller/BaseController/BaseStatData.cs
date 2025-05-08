
public class BaseStatData
{
    public int id;
    public string name;
    public string description;
    public float health;
    public float moveSpeed;
    public float reward;

    public BaseStatData() { }

    public BaseStatData(BaseStatData other)
    {
        id = other.id;
        name = other.name;
        description = other.description;
        health = other.health;
        moveSpeed = other.moveSpeed;
        reward = other.reward;
    }
}