public class Rock : Entity
{
    public class RockData : Data
    {
        public RockData()
        {
            type = Type.ROCK;
        }
    }
    public override Data GetData()
    {
        return new RockData()
        {
            position = transform.position
        };
    }
}
