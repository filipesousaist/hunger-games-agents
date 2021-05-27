using EpPathFinding.cs;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        int width = 64, height = 32;

        bool[][] movableMatrix = new bool[width][];
        for (int widthTrav = 0; widthTrav < 64; widthTrav++)
        {
            movableMatrix[widthTrav] = new bool[height];
            for (int heightTrav = 0; heightTrav < 32; heightTrav++)
            {
                movableMatrix[widthTrav][heightTrav] = true;
            }
        }
        BaseGrid searchGrid = new StaticGrid(64, 32, movableMatrix);
        searchGrid.SetWalkableAt(15, 10, false);

        GridPos startPos = new GridPos(10, 10);
        GridPos endPos = new GridPos(20, 10);
        JumpPointParam jpParam = new JumpPointParam(searchGrid, EndNodeUnWalkableTreatment.ALLOW);
        jpParam.Reset(startPos, endPos);

        List<GridPos> resultPathList = JumpPointFinder.FindPath(jpParam);
        for (int i = 0; i < resultPathList.Count; i++)
            Debug.Log(resultPathList[i]);
        Debug.Log(resultPathList);
    }
}

