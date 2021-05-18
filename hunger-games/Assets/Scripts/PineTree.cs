public class PineTree : Entity
{
    public class TreeData : Data
    {
        public TreeData()
        {
            type = Type.TREE;
        }
    }
    public override Data GetData()
    {
        return new TreeData()
        {
            position = transform.position
        };
    }
}
