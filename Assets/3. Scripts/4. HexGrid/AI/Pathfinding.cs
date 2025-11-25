using System.Collections.Generic; // V.2
using UnityEngine;

public class Pathfinding
{
    private static readonly Vector2Int[] directions = {
        new Vector2Int(+1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, +1), new Vector2Int(0, -1),
        new Vector2Int(+1, -1), new Vector2Int(-1, +1)
    };

    public static List<Vector2Int> BFS(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        // track nearest explored tile to goal
        Vector2Int closest = start;
        float closestDist = AxialDistance(start, goal);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            float dist = AxialDistance(current, goal);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = current;
            }

            if (current == goal) break;

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (!cameFrom.ContainsKey(next) && HexGridManager.Instance.GetHex(next) != null)
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        Vector2Int end = cameFrom.ContainsKey(goal) ? goal : closest;
        return ReconstructPath(cameFrom, start, end);
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        if (!cameFrom.ContainsKey(end)) return path;

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

    // axial distance for hexes (q,r) using cube coords: d = (|q| + |r| + |q+r|)/2
    private static int AxialDistance(Vector2Int a, Vector2Int b)
    {
        int dq = a.x - b.x;
        int dr = a.y - b.y;
        return (Mathf.Abs(dq) + Mathf.Abs(dr) + Mathf.Abs(dq + dr)) / 2;
    }
}
