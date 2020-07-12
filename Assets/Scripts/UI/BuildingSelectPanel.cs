using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectPanel : MonoBehaviour
{
    public Dropdown BuildingMenu;
    public GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        BuildingMenu.options.Clear();

        foreach(BuildingAssets.BuildingAsset entry in gm._buildingPrefabs.BuildingAssetEntries) 
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = entry.bt.ToString();
            option.image = entry.icon;
            BuildingMenu.options.Add(option);
        }
        Dropdown.OptionData opt = new Dropdown.OptionData();
        opt.text = Building.BuildingTypes.Empty.ToString();
        BuildingMenu.options.Add(opt);

        BuildingMenu.onValueChanged.AddListener(delegate { gm.handleBuildingSelectInput(BuildingMenu); });
        BuildingMenu.RefreshShownValue();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
