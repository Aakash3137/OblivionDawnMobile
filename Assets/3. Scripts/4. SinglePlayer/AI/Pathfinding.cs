using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    // 6 cube directions
    private static readonly Vector3[] cubeDirs = {
        new Vector3(+1, -1, 0),
        new Vector3(+1, 0, -1),
        new Vector3(0, +1, -1),
        new Vector3(-1, +1, 0),
        new Vector3(-1, 0, +1),
        new Vector3(0, -1, +1)
    };

    // Input axial coord (q,r), output axial neighbors
    public static List<Vector2Int> GetNeighbors(Vector2Int axial)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        var gm = HexGridManager.Instance;
        Vector3 cube = gm.AxialToCube(axial);

        foreach (var dir in cubeDirs)
        {
            Vector3 nextCube = cube + dir;
            Vector2Int nextAxial = gm.CubeToAxial(nextCube);

            if (gm.GetHex(nextAxial) != null)
                neighbors.Add(nextAxial);
        }

        return neighbors;
    }

    public static List<Vector2Int> BFS(Vector2Int startAxial, Vector2Int goalAxial)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(startAxial);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[startAxial] = startAxial;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            if (current == goalAxial) break;

            foreach (var next in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goalAxial)) return new List<Vector2Int>();
        return ReconstructPath(cameFrom, startAxial, goalAxial);
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int step = end;

        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}
