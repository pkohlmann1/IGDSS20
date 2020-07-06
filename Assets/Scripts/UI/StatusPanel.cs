using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    public Text _childPanel;
    public Text _workPanel;
    public Text _retirePanel;
    public Text _buildingPanel;
    public Text _happyPanel;
    public Text _moneyPanel;
    public Text _resourcePanel;
    public GameManager gm;
    public JobManager jm;
    void Start()
    {
        _childPanel.text = "0";
        _workPanel.text = "0";
        _retirePanel.text = "0";
        _happyPanel.text = "0";
        _happyPanel.text = "0";
        _resourcePanel.text = "";

    }

    // Update is called once per frame
    void Update()
    {
        int childcount = 0;
        int workcount = 0;
        int retirecount = 0;
        float happycount = 0f;
        foreach(HousingBuilding h in gm._buildings.OfType<HousingBuilding>())
        { 
            foreach(Worker w in h._workers) if (w.gameObject.activeInHierarchy) 
                {
                    switch (w._state) 
                    {
                        case Worker.WorkingState.Child:
                            childcount++;
                            break;
                        case Worker.WorkingState.Working:
                            workcount++;
                            break;
                        case Worker.WorkingState.Retired:
                            retirecount++;
                            break;
                        default:
                            break;
                    }
                    happycount += w._happiness;
                }
        }
        float happiness = happycount / (childcount+workcount+retirecount);
        int unemployment = jm._unoccupiedWorkers.Count;
        int employed = workcount - unemployment;
        int openJobs = jm._availableJobs.Count;
        _childPanel.text = childcount.ToString();
        _workPanel.text = workcount.ToString()+"("+unemployment.ToString()+ "/" + employed.ToString() +")\t["+openJobs.ToString()+"]" ;
        _retirePanel.text = retirecount.ToString();
        _happyPanel.text = happiness.ToString("0.00");
        _moneyPanel.text = gm._money.ToString("0.00");
        _buildingPanel.text = gm._selectedBuildingIndex.ToString();
        string resources = "";
        foreach(KeyValuePair<GameManager.ResourceTypes,float> entry in gm._resourcesInWarehouse) 
        {
            resources += entry.Key.ToString() + ": " + entry.Value.ToString("0.00") + "\t";
        }
        _resourcePanel.text = resources;
    }
}
