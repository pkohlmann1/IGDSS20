using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    public GameManager _gameManager;//Reference to the GameManager
    #endregion

    public float _age; // The age of this worker
    public float _happiness; // The happiness of this worker|| assume value range [0,1]

    public float _fishConsumption;
    public float _clothConsumption;
    public float _schnappsConsumtpion;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        InvokeRepeating("Age", 15.0f, 15.0f);
        InvokeRepeating("´ConsumeRessources", 60.0f, 60.0f);
    }

    // Update is called once per frame
    void Update()
    {
       
    }


    private void Age()
    {
        //TODO: Implement a life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.

       _age += 1;

        if (_age > 14)
        {
            BecomeOfAge();
        }

        if (_age > 64)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
        }
    }


    public void BecomeOfAge()
    {
        _jobManager.RegisterWorker(this);
    }

    private void Retire()
    {
        _jobManager.RemoveWorker(this);
    }

    private void Die()
    {
        Destroy(this.gameObject, 1f);
    }

    private void ConsumeRessources()
    {
        _gameManager._resourcesInWarehouse[GameManager.ResourceTypes.Fish] -= _fishConsumption;
        _gameManager._resourcesInWarehouse[GameManager.ResourceTypes.Clothes] -= _clothConsumption;
        _gameManager._resourcesInWarehouse[GameManager.ResourceTypes.Schnapps] -= _schnappsConsumtpion;
    }
}
