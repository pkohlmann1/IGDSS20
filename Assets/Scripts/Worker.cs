using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region References
    public JobManager _jobManager; //Reference to the JobManager
    public GameManager _gameManager;//Reference to the GameManager
    public Job _job;
    public HousingBuilding _house;
    #endregion

    public float _age; // The age of this worker
    public float _happiness; // The happiness of this worker|| assume value range [0,1]

    public static float workingAge = 14f;
    public static float retiringAge = 64f;
    public static float dyingAge = 100f;
    
    public enum WorkingState { Working,Retired,Child};
    private WorkingState _state;

    public float _fishConsumption;
    public float _clothConsumption;
    public float _schnappsConsumtpion;

    // Start is called before the first frame update
    void Start()
    {
        _state = WorkingState.Child;
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        _jobManager = GameObject.FindWithTag("GameManager").GetComponent<JobManager>();
        InvokeRepeating("Age", 15.0f, 15.0f);
        //InvokeRepeating("ConsumeRessources", 60.0f, 60.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float jobHappy = 0f;
        if (_job != null) jobHappy = 1f;
        _happiness = jobHappy;
    }


    private void Age()
    {
        //TODO: Implement a life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.

       _age += 1;
        switch (_age) 
        {
            case float a when a > workingAge && _state == WorkingState.Child:
                BecomeOfAge();
                break;
           
            case float a when a > retiringAge && _state != WorkingState.Retired:
                Retire();
                break;
            
            case float a when a > dyingAge:
                Die();
                break;

            default:
                break;
        }
    }


    public void BecomeOfAge()
    {
        _jobManager.RegisterWorker(this);
        _state = WorkingState.Working;
    }

    private void Retire()
    {
        _jobManager.RemoveWorker(this);
        _state = WorkingState.Retired;
    }

    private void Die()
    {
        this.gameObject.SetActive(false);
        _state = WorkingState.Child;
    }

    private void ConsumeRessources()
    {
        _gameManager._resourcesInWarehouse[GameManager.ResourceTypes.Fish] -= _fishConsumption;
        _gameManager._resourcesInWarehouse[GameManager.ResourceTypes.Clothes] -= _clothConsumption;
        _gameManager._resourcesInWarehouse[GameManager.ResourceTypes.Schnapps] -= _schnappsConsumtpion;
    }
}
