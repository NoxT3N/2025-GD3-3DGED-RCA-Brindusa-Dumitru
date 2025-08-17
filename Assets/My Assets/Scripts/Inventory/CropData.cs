using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Scriptable Objects/CropData")]
public class CropData : ItemData
{
    public string cropName;
    public GameObject[] growthStages;
    public float growthTimePerStage; //in seconds
    public int yieldAmount; //how many items harvested
    public HarvestedCropData HarvestedItem;
    public float marketValue;
}
