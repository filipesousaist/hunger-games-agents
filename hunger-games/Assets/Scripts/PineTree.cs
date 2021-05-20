public class PineTree : Entity
{ 
    public override EntityData GetData()
    {
        return new TreeData()
        {
            position = transform.position
        };
    }
}

public class TreeData : EntityData
{
    public TreeData()
    {
        type = Entity.Type.TREE;
    }
}
