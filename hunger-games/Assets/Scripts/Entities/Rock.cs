public class Rock : Entity
{
    public override EntityData GetData()
    {
        return new RockData()
        {
            position = transform.position
        };
    }
}

public class RockData : EntityData
{
    public RockData()
    {
        type = Entity.Type.ROCK;
    }
}
