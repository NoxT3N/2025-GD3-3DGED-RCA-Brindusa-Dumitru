using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Scriptable Objects/CropData")]
public class CropData : ScriptableObject
{
    public string cropName;
    public string description;
    public Sprite icon;
    public GameObject[] growthStages;
    public float growthTimePerStage; //in seconds
    public int yieldAmount; //how many items harvested
}
