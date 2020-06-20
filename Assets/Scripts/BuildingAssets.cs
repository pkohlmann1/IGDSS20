using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "fileName.asset", menuName = "Anno/Building Repository")]
public class BuildingAssets : ScriptableObject
{
    [CreateAssetMenu(fileName = "fileName.asset", menuName = "Anno/Building Asset")]
    [System.Serializable]
    public class BuildingAsset : ScriptableObject
    {
        public ProductionBuilding.BuildingTypes bt;
        public GameObject asset;
    }

    void OnEnable()
    {
        foreach (BuildingAsset ba in BuildingAssetEntries)
        {
            Dict.Add(ba.bt, ba.asset);
            UnityEngine.Debug.Log("added " + ba.bt.ToString() + " to Dictionary.");
        }
    }

    public BuildingAsset[] BuildingAssetEntries;
    public Dictionary<ProductionBuilding.BuildingTypes, GameObject> Dict = new Dictionary<ProductionBuilding.BuildingTypes, GameObject>();
}
