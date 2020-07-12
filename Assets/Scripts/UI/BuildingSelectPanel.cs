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
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach(BuildingAssets.BuildingAsset entry in gm._buildingPrefabs.BuildingAssetEntries) 
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = entry.bt.ToString();
            option.image = entry.icon;
            options.Add(option);
        }
        BuildingMenu.AddOptions(options);
        BuildingMenu.RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
