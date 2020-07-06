using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Assertions;

public class NavigationManager : MonoBehaviour
{
    public GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updateTravelMap(Tile start) 
    {
        Debug.Log("Adding new Routes.",this);
        IEnumerator coroutine = Dijkstra(start);
        StartCoroutine(coroutine);
    }

    IEnumerator Dijkstra(Tile start) 
    {
        List<Tile> Q = new List<Tile>();
        Dictionary<Tile, Tile> prev = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> dist = new Dictionary<Tile, float>();
        foreach(Tile t in gm._tileMap) 
        {
            dist.Add(t, float.PositiveInfinity);
            prev.Add(t, null);
            Q.Add(t);
        }
        dist[start] = 0f;
        yield return null;//this gives the Mic back to the Game Engine
        while (Q.Any()) 
        {
            Tile point = Q.FirstOrDefault();
            foreach(Tile t in Q) if (dist[point] > dist[t]) point = t;

            UnityEngine.Debug.Assert(Q.Contains(point),"point not in Q!",this);
            Q.Remove(point);
            int count = 0;
            foreach(Tile neighbor in point._neighborTiles) 
            {
                if (Q.Contains(neighbor)) 
                {
                    float alternate = dist[point] + ((Tile.traversalTime[point._type] + Tile.traversalTime[neighbor._type]) * 0.5f);
                    if (alternate < dist[neighbor]) 
                    {
                        dist[neighbor] = alternate;
                        prev[neighbor] = point;
                        count++;
                    }
                }
            }
            //Debug.Log("remainding Tiles: " + Q.Count.ToString()+"\tupdates: "+count.ToString(), this);
            yield return null;//this gives the Mic back to the Game Engine
        }
        int deadend = 0;
        foreach (KeyValuePair<Tile, Tile> entry in prev) if (entry.Value is null) deadend++;
        
        List<Tile> RoutePoints = gm._buildings.Select(i => i._tile).ToList();
        RoutePoints.Remove(start);
        Debug.Log("Done applying dijkstra. Now generating "+RoutePoints.Count.ToString()+" paths.\t" + deadend.ToString() + " deadends counted.", this);
        yield return null;//this gives the Mic back to the Game Engine
        int countPaths = 0;
        while (RoutePoints.Any()) 
        {
            Tile u = RoutePoints.First();
            RoutePoints.Remove(u);
            List<Tile> path = getPath(start, u, prev);
            Debug.Log("from: "+start._building.ToString()+"\t to:"+u._building.ToString()+"\tpath length: "+path.Count.ToString(),this);
            Queue<Tile> ToPath = new Queue<Tile>(path);
            path.Reverse();
            Queue<Tile> FromPath = new Queue<Tile>(path);
            enterPath(ToPath,start);
            enterPath(FromPath, u);
            countPaths += 2;
            yield return null;//this gives the Mic back to the Game Engine
        }
        Debug.Log(countPaths.ToString()+" Paths generated. Applying updates to routing tables.",this);
        int updateCount = 0;
        foreach(Tile t in gm._tileMap) 
        {
            updateCount += t.n_routeTo.Count;
            t.mergeNewRouting();
            yield return null;//this gives the Mic back to the Game Engine
        }
        Debug.Log(updateCount.ToString()+" Routing table Entries successfully updated.",this);
    }

    private List<Tile> getPath(Tile start, Tile goal,Dictionary<Tile,Tile> prev) 
    {
        List<Tile> Weg = new List<Tile>();
        Weg.Add(goal);
        Tile u = goal;
        while (prev[u] != null) 
        {
            u = prev[u];
            Weg.Add(u);
        }
        UnityEngine.Debug.Assert(u!=start,"No path to tile found! (stuck in a dead end)",this);
        return Weg;
    }

    private void enterPath(Queue<Tile> path,Tile end) 
    {
        Tile n = path.Dequeue();
        Tile o;
        int pathSteps = 0;
        while (path.Count>0) 
        {
            o = n;
            n = path.Dequeue();
            pathSteps++;
            if (!(o.n_routeTo.ContainsKey(end)))
            { 
                o.n_routeTo.Add(end, n);//add route to next tile in path 
            }
            if(!(o.n_travelTime.ContainsKey(n)))
            {
                o.n_travelTime.Add(n, 0.5f * (Tile.traversalTime[o._type] + Tile.traversalTime[n._type]));//Add traveltime to neighbor tile

            }
        }
        Debug.Log(pathSteps.ToString()+" step path added.",this);
    }
}
