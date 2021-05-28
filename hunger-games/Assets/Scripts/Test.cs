using EpPathFinding.cs;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Test : MonoBehaviour
{
    private void Start()
    {
        int width = 4, height = 4;

        bool[][] movableMatrix = new bool[width][];
        for (int widthTrav = 0; widthTrav < width; widthTrav++)
        {
            movableMatrix[widthTrav] = new bool[height];
            for (int heightTrav = 0; heightTrav < height; heightTrav++)
            {
                movableMatrix[widthTrav][heightTrav] = true;
            }
        }
        BaseGrid searchGrid = new StaticGrid(width, height, movableMatrix);
        searchGrid.SetWalkableAt(1, 1, false);
        searchGrid.SetWalkableAt(1, 2, false);
        searchGrid.SetWalkableAt(2, 1, false);
        searchGrid.SetWalkableAt(2, 2, false);
        searchGrid.SetWalkableAt(0, 1, false);

        GridPos startPos = new GridPos(0, 0);
        GridPos endPos = new GridPos(0, 2);
        JumpPointParam jpParam = new JumpPointParam(searchGrid, EndNodeUnWalkableTreatment.ALLOW, DiagonalMovement.OnlyWhenNoObstacles);
        jpParam.Reset(startPos, endPos);

        List<GridPos> resultPathList = JumpPointFinder.FindPath(jpParam);
        for (int i = 0; i < resultPathList.Count; i++)
            Debug.Log(resultPathList[i]);
        Debug.Log(resultPathList);

        Pathfinder pathfinder = new Pathfinder(4, 4);
        pathfinder.SetWalkable(1, 1, false);
        pathfinder.SetWalkable(1, 2, false);
        pathfinder.SetWalkable(2, 1, false);
        pathfinder.SetWalkable(2, 2, false);
        pathfinder.SetWalkable(0, 1, false);
        Stack<Agent.Action> actions = new Stack<Agent.Action>();
        pathfinder.AddActionsToStack(
            Vector3.zero - Const.WORLD_SIZE * new Vector3(1, 0, 1), 
            new Vector3(0, 0, 2) - Const.WORLD_SIZE * new Vector3(1, 0, 1),
            0, actions);

        while (actions.Any())
        {
            Debug.Log(actions.Pop());
        }
    }
}

