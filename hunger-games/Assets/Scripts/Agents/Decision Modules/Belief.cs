
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using static Agent;


public class Belief
{
    private readonly EntityData[][] map;
    private readonly Dictionary<Vector3,BushData> bushes;
    private readonly Dictionary<Vector3, ChestData> chests;
    private readonly Tuple<AgentData, int>[] agentsData;
    private HazardEffectData[] hazardsOrder;
    private AgentData myData;
    private readonly Pathfinder pathfinder;
    
    public Belief(Pathfinder pathfinder)
    {
        // Create map
        int size = Const.WORLD_SIZE * 2 + 1;
        map = new EntityData[size][];
        for (int x = 0; x < size; x++)
        {
            map[x] = new EntityData[size];
            for (int z = 0; z < size; z++)
                map[x][z] = null;
        }

        bushes = new Dictionary<Vector3, BushData>();
        chests = new Dictionary<Vector3, ChestData>();
        agentsData = new Tuple<AgentData, int>[Const.NUM_AGENTS];
        hazardsOrder = new HazardEffectData[Const.NUM_REGIONS];
        this.pathfinder = pathfinder;
    }

    public void UpdateAgentsData(Tuple<AgentData, int> agentData)
    {
        /*Vector3 oldAgentPos = agentsData[agentData.Item1.index - 1].Item1.position;
        Vector3 newAgentPos = agentData.Item1.position;

        pathfinder.SetWalkable(oldAgentPos.x, oldAgentPos.z, true);
        pathfinder.SetWalkable(newAgentPos.x, newAgentPos.z, false);*/

        agentsData[agentData.Item1.index - 1] = agentData;

    }

    public AgentData GetMyData()
    {
        return myData;
    }

    public void UpdateMyData(AgentData agentData)
    {
        myData = agentData;
    }

    public HazardEffectData[] GetHazardsOrder()
    {
        return hazardsOrder;
    }

    public Dictionary<Vector3, ChestData> GetChests()
    {
        return chests;
    }

    public Dictionary<Vector3, BushData> GetBushes()
    {
        return bushes;
    }

    public void UpdateHazardsOrder(HazardEffectData[] order)
    {
        hazardsOrder = order;
        if (hazardsOrder.Contains(null))
        {
            int filled = 0;
            int missingIndex = 0;
            HazardEffectData[] filledHazards = new HazardEffectData[hazardsOrder.Length];
            
            for (int i = 0; i < hazardsOrder.Length; i++)
            {
                HazardEffectData hazard = order[i];
                
                if (hazard == null)
                {
                    missingIndex = i;
                }
                else
                {
                    filledHazards[i] = hazard;
                    filled++;
                }
            }
            if (filled == Const.NUM_REGIONS-1)
            {
                hazardsOrder[missingIndex] = new HazardEffectData();
                foreach (Hazard.Type hazardType in new Hazard.Type[]{Hazard.Type.FOG, Hazard.Type.FIRE,  Hazard.Type.RAIN,  Hazard.Type.RADIATION})
                {
                    if ((filledHazards.Count((hazardAux) => hazardAux.hazardType == hazardType)) == 1)
                    {
                        hazardsOrder[missingIndex].hazardType = hazardType;
                    }
                }

                for (int i = 0; i < Const.NUM_REGIONS; i++)
                {
                    if ((filledHazards.Count((hazardAux) => hazardAux.region == i)) == 0)
                    {
                        hazardsOrder[missingIndex].region = i;
                    }
                }
            }
        }
    }

    public void UpdateMap(IEnumerable<EntityData> entities)
    {
        foreach (EntityData entity in entities)
        {
            Vector3 matrixPosition = GetMatrixPosition(entity.position); 
            map[(int)matrixPosition.x][(int)matrixPosition.z] = entity;
            pathfinder.SetWalkable(matrixPosition.x, matrixPosition.z, false);
        }
    }

    public void AddBushes(BushData bush)
    {
        Vector3 matrixPosition = GetMatrixPosition(bush.position); 
        if (map[(int)matrixPosition.x][(int)matrixPosition.z] == null)
        {
            bushes.Add(bush.position,bush);
        }
        else
        { 
            bushes[bush.position]= bush; 
        }

    }
    
    public void AddChests(ChestData chest)
    {
        Vector3 matrixPosition = GetMatrixPosition(chest.position); 
        if (map[(int)matrixPosition.x][(int)matrixPosition.z] == null)
        {
            chests.Add(chest.position,chest);
        }
        else
        { 
            chests[chest.position]= chest; 
        }
       

    }

    public Vector3 GetNearestBushPosition()
    {
        Vector3 nearestBushPosition = bushes.First().Key;
        foreach (KeyValuePair<Vector3, BushData> bush in bushes)
        {
            if ((bush.Key - myData.position).magnitude < (nearestBushPosition - myData.position).magnitude)
            {
                nearestBushPosition = bush.Key;
            }
        }
        return nearestBushPosition;
    }

    public Vector3 GetNearestStrongerChestPosition()
    {
        KeyValuePair<Vector3, ChestData> nearestStrongerChest = chests.First();
        foreach (KeyValuePair<Vector3, ChestData> chest in chests)
        {
            if ((chest.Key - myData.position).magnitude <= (nearestStrongerChest.Key - myData.position).magnitude && chest.Value.weaponAttack > myData.weaponAttack )
            {
                nearestStrongerChest = chest;
            }
        }
        return nearestStrongerChest.Key;
        
    }

    public Vector3 GetNearestStrongerDifferentChestPosition()
    {
        KeyValuePair<Vector3, ChestData> nearestStrongerDifferentChest = chests.First();
        foreach (KeyValuePair<Vector3, ChestData> chest in chests)
        {
            if ((chest.Key - myData.position).magnitude <= (nearestStrongerDifferentChest.Key - myData.position).magnitude 
                && chest.Value.weaponAttack > myData.weaponAttack 
                && chest.Value.weaponType != myData.weaponType)
            {
                nearestStrongerDifferentChest = chest;
            }
        }
        return nearestStrongerDifferentChest.Key;

    }
    
    public Vector3 GetUnexploredPoint(int radius, Perception perception)
    { 
        int minX = Mathf.Max((int) myData.position.x - radius, -(int)perception.shieldRadius);
        int maxX = Mathf.Min((int) myData.position.x + radius, (int)perception.shieldRadius);
        int minZ = Mathf.Max((int) myData.position.z - radius, -(int)perception.shieldRadius);
        int maxZ = Mathf.Min((int) myData.position.z + radius, (int)perception.shieldRadius);

        while (true)
        {
            int x_index = Random.Range(minX, maxX + 1) + 250;
            int z_index = Random.Range(minX, maxX + 1) + 250;

            if (map[x_index][z_index] == null)
                return new Vector3(x_index - Const.WORLD_SIZE, 0, z_index - Const.WORLD_SIZE);
        }
        
        //return new Vector3(30, 0, 30);
    }

    public Tuple<AgentData,int>[] GetAgentsData()
    {
        return agentsData;
    }


    public Vector3 GetRandomSafe(int radius, int clock, Perception perception)
    {
        IEnumerable<EntityData> visionData = perception.visionData;
        IEnumerable<AgentData> otherAgents = visionData.Where(entityData => entityData.type == Entity.Type.AGENT).Cast<AgentData>();
        IEnumerable<AgentData> dangerousAgents = DeciderUtils.GetDangerousAgentDatas(otherAgents, myData);

        int minX = (int)Mathf.Max( myData.position.x - radius, -perception.shieldRadius);
        int maxX = (int)Mathf.Min( myData.position.x + radius, perception.shieldRadius);
        int minZ = (int)Mathf.Max( myData.position.z - radius, -perception.shieldRadius);
        int maxZ = (int)Mathf.Min( myData.position.z + radius, perception.shieldRadius);

        while (true)
        {
            int x_index = Random.Range(minX, maxX + 1);
            int z_index = Random.Range(minZ, maxZ + 1);
            Vector3 point = new Vector3(x_index, 0, z_index);
            if (!(HazardsManager.GetRegion(point) == hazardsOrder[perception.timeslot].region && 
                HazardsManager.GetRegion(perception.myData.position) == hazardsOrder[perception.timeslot].region)
                && !dangerousAgents.Any(otherAgent => (otherAgent.position-myData.position).magnitude<=radius)) //TODO 

                return point;
        }
    }
    private Vector3 GetMatrixPosition(Vector3 position)
    {
        return new Vector3(position.x + Const.WORLD_SIZE, 0, position.z + Const.WORLD_SIZE);
    }
}

