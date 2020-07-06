using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job
{
    public Worker _worker; //The worker occupying this job
    public ProductionBuilding _building; //The building offering the job

    //Constructor. Call new Job(this) from the Building script to instanciate a job
    public Job(ProductionBuilding building)
    {
        _building = building;
    }

    public void AssignWorker(Worker w)
    {
        _worker = w;
        w._job = this;
        _building.WorkerAssignedToBuilding(w);
        _building._jobManager.RemoveJob(this);
        _building._jobManager.RemoveWorker(w);

    }

    public void RemoveWorker(Worker w)
    {
        _worker = null;
        w._job = null;
        _building.WorkerRemovedFromBuilding(w);
        _building._jobManager.RegisterJob(this);
        _building._jobManager.RegisterWorker(w);
    }
}
