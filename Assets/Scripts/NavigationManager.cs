using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updateTravelMap() 
    {
        //first empty all navigation maps
        foreach(Tile t in gm._tileMap) 
        {
            t.travelTime = new Dictionary<Tile, float>() { { t, 0f } };
            t.routeTo = new Dictionary<Tile, Tile>() { { t, t } };
            //now that only the self reference remains, enter neighbor tiles into map.
            foreach(Tile n in t._neighborTiles) 
            {
                float tm = (Tile.traversalTime[t._type] + Tile.traversalTime[n._type]) * 0.5f;
                t.n_routeTo.Add(n, n);
                t.n_travelTime.Add(n, tm);
            }

        }
        Debug.Log("Reset Routing Tables.");
        StartCoroutine("Relax");
    }

    IEnumerator Relax() 
    {
        int change = 1;
        while (change > 0) 
        {
            change = 0;
            foreach(Tile t in gm._tileMap) 
            {
                //commiting new routes to table
                t.mergeNewRouting();
                foreach(KeyValuePair<Tile,Tile> route in t.routeTo)
                {
                    Tile midpoint = route.Key;
                    Tile startRoute = route.Value;
                    float midTime = t.travelTime[midpoint];
                    foreach(KeyValuePair<Tile,float> nRoute in midpoint.n_travelTime) 
                    {
                        Tile endpoint = nRoute.Key;
                        float routeTime = nRoute.Value + midTime;
                        if (!(t.n_travelTime.ContainsKey(endpoint)))//is this allready in the new Routes?
                        {
                            if (!t.travelTime.ContainsKey(endpoint)) //if not is it in our current Routemap?
                            { 
                                change++;
                                t.n_travelTime.Add(endpoint, routeTime);
                                t.n_routeTo.Add(endpoint, startRoute);
                            }
                            else if (t.travelTime[endpoint] > routeTime)//ok, check if this ones faster.
                            {
                                change++;
                                t.n_travelTime.Add(endpoint, routeTime);
                                t.n_routeTo.Add(endpoint, startRoute);
                            }
                                
                            
                        }
                        else if (t.n_travelTime[endpoint] > routeTime)//ok, check if this ones faster than the other one; you don't need to check Routemap, we've allready found a faster one.
                        {
                            t.n_routeTo[endpoint] = startRoute;
                            t.n_travelTime[endpoint] = routeTime;
                        }
                        
                    }
                }
                yield return null;//this gives the Mic back to the Game Engine
            }
            Debug.Log("Updating "+change.ToString()+" Routes.");
            
        }
    }
}

public class Route : ScriptableObject 
{
    Tile start;
    Tile next;
    Route cont;
    Tile end;
    float travelTime;
}