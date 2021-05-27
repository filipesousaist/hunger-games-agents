using EpPathFinding.cs;
using System.Collections.Generic;
using UnityEngine;
using static Agent;
public class Pathfinder
{
    private readonly BaseGrid searchGrid;
    public Pathfinder(int width, int height)
    {
        bool[][] movableMatrix = new bool[width][];
        for (int w = 0; w < width; w++)
        {
            movableMatrix[w] = new bool[height];
            for (int h = 0; h < height; h++)
            {
                movableMatrix[w][h] = true;
            }
        }
        searchGrid = new StaticGrid(width, height, movableMatrix);
    }

    private List<GridPos> FindPath(Vector2Int startVec, Vector2Int endVec)
    {
        GridPos start = new GridPos(startVec.x, startVec.y);
        GridPos end = new GridPos(endVec.x, endVec.y);
        JumpPointParam jpParam = new JumpPointParam(searchGrid, EndNodeUnWalkableTreatment.DISALLOW);
        jpParam.Reset(start, end);

        return JumpPointFinder.FindPath(jpParam);
    }

    public void AddActionsToStack(Vector3 start, Vector3 end, float rotationY, Stack<Action> actions)
    {
        Vector2Int roundedStart = new Vector2Int(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.z));
        Vector2Int roundedEnd = new Vector2Int(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.z));
        if (roundedStart.x == roundedEnd.x && roundedStart.y == roundedEnd.y)
            return;

        List<GridPos> jumpPoints = FindPath(roundedStart, roundedEnd);

        // Go from end to beginning
        int len = jumpPoints.Count;

        Vector2Int from = new Vector2Int(jumpPoints[len - 1].x, jumpPoints[len - 1].y);
        Vector2Int to = new Vector2Int(jumpPoints[len - 2].x, jumpPoints[len - 2].y);

        for (int i = len - 1; i > 0; i --)
        {
            int numWalks = Mathf.RoundToInt((from - to).magnitude / Const.WALK_DISTANCE);

            for (int a = 0; a < numWalks; a++)
                actions.Push(Action.WALK);

            float angleToRotate;
            Vector2Int next;
            if (i > 1) { 
                next = new Vector2Int(jumpPoints[i - 2].x, jumpPoints[i - 2].y);

                angleToRotate = Vector2.SignedAngle(to - next, from - to);
            }
            else
            {
                next = Vector2Int.zero; // Ignore this
                Vector2Int diff = from - to;
                angleToRotate = Vector3.SignedAngle(Utils.GetForward(rotationY), new Vector3(diff.x, 0, diff.y), Vector3.up);
            }

            int numRots = Mathf.Abs(Mathf.RoundToInt(angleToRotate / Const.ROTATE_ANGLE));
            Action rotAction = angleToRotate > 0 ? Action.ROTATE_RIGHT : Action.ROTATE_LEFT;

            for (int a = 0; a < numRots; a++)
                actions.Push(rotAction);

            from = to;
            to = next;
        }
    }
}

