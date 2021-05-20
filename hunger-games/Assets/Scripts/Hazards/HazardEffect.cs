public class HazardEffect : Entity
{
    public Hazard.Type type;
    public override EntityData GetData()
    {
        return new HazardEffectData()
        {
            hazardType = type
        };
    }
}

public class HazardEffectData : EntityData
{
    public Hazard.Type hazardType;
    public HazardEffectData()
    {
        type = Entity.Type.HAZARD_EFFECT;
    }
}