// using System.Collections.Generic; // V.2
// using UnityEngine;

// public class Pathfinding
// {
//     private static readonly Vector2Int[] directions = {
//         new Vector2Int(+1, 0), new Vector2Int(-1, 0),
//         new Vector2Int(0, +1), new Vector2Int(0, -1),
//         new Vector2Int(+1, -1), new Vector2Int(-1, +1)
//     };

//     public static List<Vector2Int> BFS(Vector2Int start, Vector2Int goal)
//     {
//         Queue<Vector2Int> frontier = new Queue<Vector2Int>();
//         frontier.Enqueue(start);

//         Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
//         cameFrom[start] = start;

//         // track nearest explored tile to goal
//         Vector2Int closest = start;
//         float closestDist = AxialDistance(start, goal);

//         while (frontier.Count > 0)
//         {
//             Vector2Int current = frontier.Dequeue();

//             float dist = AxialDistance(current, goal);
//             if (dist < closestDist)
//             {
//                 closestDist = dist;
//                 closest = current;
//             }

//             if (current == goal) break;

//             foreach (var dir in directions)
//             {
//                 Vector2Int next = current + dir;
//                 if (!cameFrom.ContainsKey(next) && HexGridManager.Instance.GetHex(next) != null)
//                 {
//                     frontier.Enqueue(next);
//                     cameFrom[next] = current;
//                 }
//             }
//         }

//         Vector2Int end = cameFrom.ContainsKey(goal) ? goal : closest;
//         return ReconstructPath(cameFrom, start, end);
//     }

//     private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
//     {
//         List<Vector2Int> path = new List<Vector2Int>();
//         if (!cameFrom.ContainsKey(end)) return path;

//         Vector2Int step = end;
//         while (step != start)
//         {
//             path.Add(step);
//             step = cameFrom[step];
//         }
//         path.Add(start);
//         path.Reverse();
//         return path;
//     }

//     // axial distance for hexes (q,r) using cube coords: d = (|q| + |r| + |q+r|)/2
//     private static int AxialDistance(Vector2Int a, Vector2Int b)
//     {
//         int dq = a.x - b.x;
//         int dr = a.y - b.y;
//         return (Mathf.Abs(dq) + Mathf.Abs(dr) + Mathf.Abs(dq + dr)) / 2;
//     }
// }









// using System.Collections.Generic;
// using UnityEngine;

// public class Pathfinding
// {
//     // Proper neighbors for odd-r offset hex grid
//     public static List<Vector2Int> GetNeighbors(Vector2Int coord)
//     {
//         List<Vector2Int> neighbors = new List<Vector2Int>();

//         // Even row
//         if ((coord.y & 1) == 0)
//         {
//             neighbors.Add(new Vector2Int(coord.x - 1, coord.y));     // left
//             neighbors.Add(new Vector2Int(coord.x + 1, coord.y));     // right
//             neighbors.Add(new Vector2Int(coord.x - 1, coord.y - 1)); // up-left
//             neighbors.Add(new Vector2Int(coord.x,     coord.y - 1)); // up-right
//             neighbors.Add(new Vector2Int(coord.x - 1, coord.y + 1)); // down-left
//             neighbors.Add(new Vector2Int(coord.x,     coord.y + 1)); // down-right
//         }
//         // Odd row
//         else
//         {
//             neighbors.Add(new Vector2Int(coord.x - 1, coord.y));     // left
//             neighbors.Add(new Vector2Int(coord.x + 1, coord.y));     // right
//             neighbors.Add(new Vector2Int(coord.x,     coord.y - 1)); // up-left
//             neighbors.Add(new Vector2Int(coord.x + 1, coord.y - 1)); // up-right
//             neighbors.Add(new Vector2Int(coord.x,     coord.y + 1)); // down-left
//             neighbors.Add(new Vector2Int(coord.x + 1, coord.y + 1)); // down-right
//         }

//         return neighbors;
//     }

//     public static List<Vector2Int> BFS(Vector2Int start, Vector2Int goal)
//     {
//         Queue<Vector2Int> frontier = new Queue<Vector2Int>();
//         frontier.Enqueue(start);

//         Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
//         cameFrom[start] = start;

//         // track nearest explored tile to goal (fallback if goal unreachable)
//         Vector2Int closest = start;
//         float closestDist = AxialDistance(start, goal);

//         while (frontier.Count > 0)
//         {
//             Vector2Int current = frontier.Dequeue();

//             float dist = AxialDistance(current, goal);
//             if (dist < closestDist)
//             {
//                 closestDist = dist;
//                 closest = current;
//             }

//             if (current == goal) break;

//             foreach (var next in GetNeighbors(current))
//             {
//                 if (!cameFrom.ContainsKey(next) && HexGridManager.Instance.GetHex(next) != null)
//                 {
//                     frontier.Enqueue(next);
//                     cameFrom[next] = current;
//                 }
//             }
//         }

//         Vector2Int end = cameFrom.ContainsKey(goal) ? goal : closest;
//         return ReconstructPath(cameFrom, start, end);
//     }

//     private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
//     {
//         List<Vector2Int> path = new List<Vector2Int>();
//         if (!cameFrom.ContainsKey(end)) return path;

//         Vector2Int step = end;
//         while (step != start)
//         {
//             path.Add(step);
//             step = cameFrom[step];
//         }
//         path.Add(start);
//         path.Reverse();
//         return path;
//     }

//     // axial distance using offset (treated as q,r)
//     private static int AxialDistance(Vector2Int a, Vector2Int b)
//     {
//         int dq = a.x - b.x;
//         int dr = a.y - b.y;
//         return (Mathf.Abs(dq) + Mathf.Abs(dr) + Mathf.Abs(dq + dr)) / 2;
//     }
// }






// using System.Collections.Generic;
// using UnityEngine;

// public class Pathfinding
// {
//     // Cube directions (always 6 neighbors)
//     private static readonly Vector3[] cubeDirs = {
//         new Vector3(+1, -1, 0),
//         new Vector3(+1, 0, -1),
//         new Vector3(0, +1, -1),
//         new Vector3(-1, +1, 0),
//         new Vector3(-1, 0, +1),
//         new Vector3(0, -1, +1)
//     };

//     public static List<Vector2Int> BFS(Vector2Int startOffset, Vector2Int goalOffset)
//     {
//         // Convert to cube coords
//         Vector3 start = HexGridManager.Instance.OffsetToCube(startOffset);
//         Vector3 goal = HexGridManager.Instance.OffsetToCube(goalOffset);

//         Queue<Vector3> frontier = new Queue<Vector3>();
//         frontier.Enqueue(start);

//         Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
//         cameFrom[start] = start;

//         while (frontier.Count > 0)
//         {
//             Vector3 current = frontier.Dequeue();
//             if (current == goal) break;

//             foreach (var dir in cubeDirs)
//             {
//                 Vector3 next = current + dir;
//                 Vector2Int offset = HexGridManager.Instance.CubeToOffset(next);

//                 // Only explore if tile exists
//                 if (!cameFrom.ContainsKey(next) && HexGridManager.Instance.GetHex(offset) != null)
//                 {
//                     frontier.Enqueue(next);
//                     cameFrom[next] = current;
//                 }
//             }
//         }

//         // Reconstruct path in offset coords
//         List<Vector2Int> path = new List<Vector2Int>();
//         if (!cameFrom.ContainsKey(goal)) return path;

//         Vector3 step = goal;
//         while (step != start)
//         {
//             path.Add(HexGridManager.Instance.CubeToOffset(step));
//             step = cameFrom[step];
//         }
//         path.Add(startOffset);
//         path.Reverse();
//         return path;
//     }
// }




// using System.Collections.Generic;
// using UnityEngine;

// public class Pathfinding
// {
//     // Correct neighbors for odd-r offset hex grid
//     public static List<Vector2Int> GetNeighbors(Vector2Int coord)
//     {
//         bool oddRow = (coord.y & 1) == 1;

//         if (oddRow)
//         {
//             return new List<Vector2Int> {
//                 new Vector2Int(coord.x - 1, coord.y),     // left
//                 new Vector2Int(coord.x + 1, coord.y),     // right
//                 new Vector2Int(coord.x,     coord.y - 1), // up-left
//                 new Vector2Int(coord.x + 1, coord.y - 1), // up-right
//                 new Vector2Int(coord.x,     coord.y + 1), // down-left
//                 new Vector2Int(coord.x + 1, coord.y + 1)  // down-right
//             };
//         }
//         else
//         {
//             return new List<Vector2Int> {
//                 new Vector2Int(coord.x - 1, coord.y),     // left
//                 new Vector2Int(coord.x + 1, coord.y),     // right
//                 new Vector2Int(coord.x - 1, coord.y - 1), // up-left
//                 new Vector2Int(coord.x,     coord.y - 1), // up-right
//                 new Vector2Int(coord.x - 1, coord.y + 1), // down-left
//                 new Vector2Int(coord.x,     coord.y + 1)  // down-right
//             };
//         }
//     }

//     public static List<Vector2Int> BFS(Vector2Int start, Vector2Int goal)
//     {
//         Queue<Vector2Int> frontier = new Queue<Vector2Int>();
//         frontier.Enqueue(start);

//         Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
//         cameFrom[start] = start;

//         // track nearest explored tile to goal (fallback if goal unreachable)
//         Vector2Int closest = start;
//         float closestDist = AxialDistance(start, goal);

//         while (frontier.Count > 0)
//         {
//             Vector2Int current = frontier.Dequeue();

//             float dist = AxialDistance(current, goal);
//             if (dist < closestDist)
//             {
//                 closestDist = dist;
//                 closest = current;
//             }

//             if (current == goal) break;

//             foreach (var next in GetNeighbors(current))
//             {
//                 if (!cameFrom.ContainsKey(next) && HexGridManager.Instance.GetHex(next) != null)
//                 {
//                     frontier.Enqueue(next);
//                     cameFrom[next] = current;
//                 }
//             }
//         }

//         Vector2Int end = cameFrom.ContainsKey(goal) ? goal : closest;
//         return ReconstructPath(cameFrom, start, end);
//     }

//     private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
//     {
//         List<Vector2Int> path = new List<Vector2Int>();
//         if (!cameFrom.ContainsKey(end)) return path;

//         Vector2Int step = end;
//         while (step != start)
//         {
//             path.Add(step);
//             step = cameFrom[step];
//         }
//         path.Add(start);
//         path.Reverse();
//         return path;
//     }

//     // axial distance using offset (treated as q,r)
//     private static int AxialDistance(Vector2Int a, Vector2Int b)
//     {
//         int dq = a.x - b.x;
//         int dr = a.y - b.y;
//         return (Mathf.Abs(dq) + Mathf.Abs(dr) + Mathf.Abs(dq + dr)) / 2;
//     }
// }


using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    // Always 6 cube directions (edges only)
    private static readonly Vector3[] cubeDirs = {
        new Vector3(+1, -1, 0),
        new Vector3(+1, 0, -1),
        new Vector3(0, +1, -1),
        new Vector3(-1, +1, 0),
        new Vector3(-1, 0, +1),
        new Vector3(0, -1, +1)
    };

    public static List<Vector2Int> GetNeighbors(Vector2Int coord)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector3 cube = HexGridManager.Instance.OffsetToCube(coord);

        foreach (var dir in cubeDirs)
        {
            Vector3 nextCube = cube + dir;
            Vector2Int nextOffset = HexGridManager.Instance.CubeToOffset(nextCube);

            if (HexGridManager.Instance.GetHex(nextOffset) != null)
                neighbors.Add(nextOffset);
        }

        return neighbors;
    }

    public static List<Vector2Int> BFS(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            if (current == goal) break;

            foreach (var next in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal)) return new List<Vector2Int>();

        return ReconstructPath(cameFrom, start, goal);
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
