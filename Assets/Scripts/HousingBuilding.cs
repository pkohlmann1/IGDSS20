using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HousingBuilding : Building
{
    public static Dictionary<BuildingTypes, int> startingWorkers = new Dictionary<BuildingTypes, int>() { { BuildingTypes.Farm_House, 2 } };

    float respawnTime = 30f;
    float respawn_timer = 0f;


    override public void Construct(BuildingTypes bt, Tile t, GameManager gm)
    {
        if (Building.neighborsScaleEfficiency.Contains(bt))
        {
            Tile.TileTypes nt = Building.Neighbor_Type[bt];
            int m1 = Building.Neighbor_minmax[bt].Item1;
            int m2 = Building.Neighbor_minmax[bt].Item2;
            int neighborCount = 0;
            foreach (Tile n in t._neighborTiles) if (n._type == nt) neighborCount++;
        }

        GM = gm;
        _type = bt;
        t._building = this;
        _tile = t;
        GM._buildings.Add(this);
        //deducting resources used in construction
        GM._resourcesInWarehouse[GameManager.ResourceTypes.Planks] -= Building.cost_plank[bt];
        GM._money -= Building.cost_money[bt];
    }

    public void createWorker()
    {
        foreach (Worker w in _workers)
        {
            if (!w.gameObject.activeInHierarchy)
            {
                w._age = 0f;
                w.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void createWorker(float age)
    {
        UnityEngine.Debug.Assert(Building.workerCapacity[_type] > _workerCount, "Building is allready fully occupied", this);
        foreach (Worker w in _workers)
        {
            if (!w.gameObject.activeInHierarchy)
            {
                w._age = age;
                w.gameObject.SetActive(true);
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _workers = new List<Worker>(Building.workerCapacity[_type]);
        for(int i = 0;i< Building.workerCapacity[_type]; i++) 
        {
            GameObject workerCopy = Instantiate(GM.WorkerModel, transform);
            Worker w = workerCopy.GetComponent<Worker>();
            if (w == null) w = workerCopy.AddComponent(typeof(Worker)) as Worker;
            _workers.Add(w);
            workerCopy.SetActive(false);
        }
        for (int i = 0; i < startingWorkers[_type]; i++) createWorker();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        float tempHap = 0f;
        int activeworkers = 0;
        foreach (Worker w in _workers) if (w.gameObject.activeInHierarchy)
            {
                tempHap += w._happiness;
                activeworkers++;
            }

        _happiness = tempHap / (float)activeworkers;

        if (activeworkers < _workers.Count)
        {
            respawn_timer += Time.deltaTime * _happiness;
            if (respawn_timer > respawnTime)
            {
                respawn_timer = respawn_timer - respawnTime;
                createWorker();
            }
        }
    }

    public override int WorkerCount()
    {
        int count = 0;
        foreach (Worker w in _workers) if (w.gameObject.activeInHierarchy) count++;
        return count;
    }
}
