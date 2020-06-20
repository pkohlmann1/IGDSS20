using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{

    public List<Job> _availableJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();



    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleUnoccupiedWorkers();
    }
    #endregion


    #region Methods

    private void HandleUnoccupiedWorkers()
    {
        if (_unoccupiedWorkers.Count > 0)
        {
            foreach(Worker w in new List<Worker>(_unoccupiedWorkers))
            {
                if (_availableJobs.Count > 0)
                {
                    Job topJ = _availableJobs[0];
                    topJ.AssignWorker(w);
                }
                else break;
            }

        }
    }
    public void RegisterJob(Job j) 
    {
        _availableJobs.Add(j);
    }

    public void RemoveJob(Job j) 
    {
        _availableJobs.Remove(j);
    }

    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkers.Add(w);
    }



    public void RemoveWorker(Worker w)
    {
        _unoccupiedWorkers.Remove(w);
    }

    #endregion
}
