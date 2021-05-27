
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Belief
{
    private EntityData[][] map;
    private Dictionary<Vector3,BushData> bushes;
    private Dictionary<Vector3, ChestData> chests;
    private Tuple<AgentData, int>[] agentsData;
    private HazardEffectData[] hazardsOrder;

    private AgentData myData;


    public Belief()
    {
        map = new EntityData[501][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new EntityData[501];
        }

        bushes = new Dictionary<Vector3, BushData>();
        chests = new Dictionary<Vector3, ChestData>();
        agentsData = new Tuple<AgentData, int>[Const.NUM_AGENTS];
        hazardsOrder = new HazardEffectData[Const.NUM_REGIONS];

    }

    public void UpdateAgentsData(Tuple<AgentData, int> agentData)
    {
        agentsData[agentData.Item1.index - 1] = agentData;
    }

    public void UpdateMyData(AgentData agentData)
    {
        myData = agentData;
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

    private Vector3 GetMatrixPosition(Vector3 position)
    {
        return new Vector3(position.x + 250, 0, position.z + 250);
    }

}

