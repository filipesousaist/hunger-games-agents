using EpPathFinding.cs;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

    public void SetWalkable(float x, float z, bool value)
    {
        searchGrid.SetWalkableAt(new GridPos(Mathf.RoundToInt(x), Mathf.RoundToInt(z)), value);
    }

    private List<GridPos> FindPath(Vector2Int startVec, Vector2Int endVec)
    {
        GridPos start = new GridPos(startVec.x, startVec.y);
        GridPos end = new GridPos(endVec.x, endVec.y);
        JumpPointParam jpParam = new JumpPointParam(searchGrid, EndNodeUnWalkableTreatment.ALLOW, DiagonalMovement.OnlyWhenNoObstacles);
        jpParam.Reset(start, end);

        return JumpPointFinder.FindPath(jpParam);
    }

    public void AddActionsToStack2(Vector3 start, Vector3 end, float rotationY, Stack<Action> actions)
    {
        Vector2Int roundedStart = new Vector2Int(
            Mathf.RoundToInt(start.x) + Const.WORLD_SIZE, 
            Mathf.RoundToInt(start.z) + Const.WORLD_SIZE);
        Vector2Int roundedEnd = new Vector2Int(
            Mathf.RoundToInt(end.x) + Const.WORLD_SIZE, 
            Mathf.RoundToInt(end.z) + Const.WORLD_SIZE);
        if (roundedStart.x == roundedEnd.x && roundedStart.y == roundedEnd.y)
            return;

        List<GridPos> jumpPoints = FindPath(roundedStart, roundedEnd);

        // Go from end to beginning
        int len = jumpPoints.Count;

        Vector2Int from = new Vector2Int(jumpPoints[len - 1].x, jumpPoints[len - 1].y);
        Vector2Int to = new Vector2Int(jumpPoints[len - 2].x, jumpPoints[len - 2].y);
        Color color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        for (int i = len - 1; i > 0; i--)
        {
            int numWalks = Mathf.RoundToInt((from - to).magnitude / Const.WALK_DISTANCE);

            for (int a = 0; a < numWalks; a++)
                actions.Push(Action.WALK);

            float angleToRotate;
            Vector2Int next;
            if (i > 1)
            {
                next = new Vector2Int(jumpPoints[i - 2].x, jumpPoints[i - 2].y);

                angleToRotate = -Vector2.SignedAngle(to - next, from - to);
            }
            else
            {
                next = Vector2Int.zero; // Ignore this
                Vector2Int diff = from - to;
                Vector3 forward = Utils.GetForward(rotationY);
                Debug.Log(forward.x + "," + forward.z);
                //Debug.DrawLine(new Vector3(to.x - 250, 1, to.y - 250), new Vector3(to.x - 250, 1, to.y - 250) + forward, Color.red, 1);
                angleToRotate = Vector3.SignedAngle(forward, new Vector3(diff.x, 0, diff.y), Vector3.up);
                Debug.Log("angle " + angleToRotate);
            }

            int numRots = Mathf.Abs(Mathf.RoundToInt(angleToRotate / Const.ROTATE_ANGLE));
            Action rotAction = angleToRotate > 0 ? Action.ROTATE_RIGHT : Action.ROTATE_LEFT;

            for (int a = 0; a < numRots; a++)
                actions.Push(rotAction);

            Debug.DrawLine(new Vector3(from.x - 250, 1, from.y - 250), new Vector3(to.x - 250, 1, to.y - 250), color, 30);
            
            from = to;
            to = next;  
        }
    }

    public void AddActionsToStack(Vector3 start, Vector3 end, float rotationY, Stack<Action> actions)
    {
        Vector2Int roundedStart = new Vector2Int(
            Mathf.RoundToInt(start.x) + Const.WORLD_SIZE,
            Mathf.RoundToInt(start.z) + Const.WORLD_SIZE);
        Vector2Int roundedEnd = new Vector2Int(
            Mathf.RoundToInt(end.x) + Const.WORLD_SIZE,
            Mathf.RoundToInt(end.z) + Const.WORLD_SIZE);
        if (roundedStart.x == roundedEnd.x && roundedStart.y == roundedEnd.y)
            return;

        List<GridPos> jumpPoints = FindPath(roundedStart, roundedEnd);

        Stack<Action> reverseActions = new Stack<Action>();

        // Go from end to beginning
        int len = jumpPoints.Count;

        Vector2Int from = new Vector2Int(jumpPoints[0].x, jumpPoints[0].y);
        Vector2Int to = new Vector2Int(jumpPoints[1].x, jumpPoints[1].y);
        Color color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        for (int i = 0; i < len - 2; i++)
        {
            float angleToRotate;
            Vector2Int next;
            if (i >= 1)
            {
                next = new Vector2Int(jumpPoints[i + 2].x, jumpPoints[i + 2].y);

                angleToRotate = Vector2.SignedAngle(to - from, next - to);
            }
            else
            {
                next = Vector2Int.zero; // Ignore this
                Vector2Int diff = to - from;
                Vector3 forward = Utils.GetForward(rotationY);
                Debug.Log(forward.x + "," + forward.z);
                //Debug.DrawLine(new Vector3(to.x - 250, 1, to.y - 250), new Vector3(to.x - 250, 1, to.y - 250) + forward, Color.red, 1);
                angleToRotate = Vector3.SignedAngle(forward, new Vector3(diff.x, 0, diff.y), Vector3.up);
                Debug.Log("angle " + angleToRotate);
            }

            int numRots = Mathf.Abs(Mathf.RoundToInt(angleToRotate / Const.ROTATE_ANGLE));
            Action rotAction = angleToRotate > 0 ? Action.ROTATE_RIGHT : Action.ROTATE_LEFT;

            for (int a = 0; a < numRots; a++)
                reverseActions.Push(rotAction);

            int numWalks = Mathf.RoundToInt((from - to).magnitude / Const.WALK_DISTANCE);

            for (int a = 0; a < numWalks; a++)
                reverseActions.Push(Action.WALK);

            Debug.DrawLine(new Vector3(from.x - 250, 1, from.y - 250), new Vector3(to.x - 250, 1, to.y - 250), color, 30);

            from = to;
            to = next;
        }

        while (reverseActions.Any())
            actions.Push(reverseActions.Pop());
    }
}

