public class HazardEffect : Entity
{
    public Hazard.Type type;
    private HazardsManager hazardsManager;
    public override EntityData GetData()
    {
        return new HazardEffectData()
        {
            hazardType = type,
            region = hazardsManager.GetRegion(transform.position)
        };
    }
}

public class HazardEffectData : EntityData
{
    public Hazard.Type hazardType;
    public int region;
    public HazardEffectData()
    {
        type = Entity.Type.HAZARD_EFFECT;
    }
}