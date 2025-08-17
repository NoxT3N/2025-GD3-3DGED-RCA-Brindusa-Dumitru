using UnityEngine;

[CreateAssetMenu(fileName = "HarvestedCropData", menuName = "Scriptable Objects/HarvestedCropData")]
public class HarvestedCropData : ItemData
{
    public string cropName;
    public float sellingPrice; //the price the player can sell it for
}
