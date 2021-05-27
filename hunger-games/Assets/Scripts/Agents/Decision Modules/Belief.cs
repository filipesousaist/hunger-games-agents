
using System;
using System.Collections.Generic;
using UnityEngine;


public class Belief
{
    private EntityData[][] map;
    private Dictionary<Vector3,BushData> bushes;
    private Dictionary<Vector3, ChestData> chests;
    private Tuple<AgentData, int>[] agentsData;


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


    }

    public void UpdateAgentsData(Tuple<AgentData, int> agentData)
    {
        agentsData[agentData.Item1.index - 1] = agentData;
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

