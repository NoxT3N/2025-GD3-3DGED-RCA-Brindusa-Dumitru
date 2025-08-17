using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GardenBed : Interactable
{
    [System.Serializable]
    public class PlantingSpot
    {
        public Transform spotTransform; //position of the crop
        public CropData currentCrop; //the scriptable object representing the crop
        public GameObject plantedObject; //the prefab representing the planted crop
        public int currentStage = 0; //the current growth stage of the crop
        public bool isReady => currentCrop && currentStage == currentCrop.growthStages.Length - 1; //checks if the crop is ready to be harvested
        
    }
    private PlantingSpot[] plantingSpots = new PlantingSpot[4];

    protected override void Awake()
    {
        base.Awake(); //calling the base class Awake to ensure outline setup is done

        //preparing the positions of the planting spots
        Transform[] children = GetComponentsInChildren<Transform>();
        // for (int i = 0; i < plantingSpots.Length; i++)
        // {
        //     plantingSpots[i] = new PlantingSpot();
        //     if (i < children.Length)
        //     {
        //         plantingSpots[i].spotTransform = children[i];
        //     }
        // }
        int i = 0;
        foreach (Transform child in children)
        {
            if (child != transform)
            {
                if (child.name.StartsWith("crop_spawn"))
                {
                    if (i<plantingSpots.Length)
                    {
                        plantingSpots[i] = new PlantingSpot { spotTransform = child };
                        i++;
                    }
                }
            }
        }
    }


    public void PlantCrop(CropData seeds)
    {
        foreach (PlantingSpot spot in plantingSpots)
        {
            if (spot.currentCrop == null) //first empty spot
            {
                GameObject crop = Instantiate(seeds.growthStages[0], spot.spotTransform.position, Quaternion.identity, spot.spotTransform);
                spot.currentCrop = seeds;
                spot.plantedObject = crop;

                StartCoroutine(GrowCrop(spot));
                break; //exit after planting in the first available spot
            }
        }
    }

    public void HarvestCrop(PlayerController player)
    {
        foreach (PlantingSpot spot in plantingSpots)
        {
            if (spot.isReady)
            {
                player.GetComponent<PlayerInventoryHolder>().inventory.AddItem(spot.currentCrop.HarvestedItem, spot.currentCrop.yieldAmount);
                Destroy(spot.plantedObject); //remove the crop from the scene
                spot.currentCrop = null; //reset the crop data
                spot.currentStage = 0; //reset the growth stage
                spot.plantedObject = null; //remove the planted object reference
                Debug.Log($"Harvested from spot at {spot.spotTransform.position}");
            }
            else
            {
                Debug.Log("Crop is not ready for harvest yet.");
            }
        }
    }
    private IEnumerator GrowCrop(PlantingSpot spot)
    {
        for(int i = 1; i < spot.currentCrop.growthStages.Length; i++)
        {
            yield return new WaitForSeconds(spot.currentCrop.growthTimePerStage);
            if (spot.plantedObject != null)
            {
                Destroy(spot.plantedObject); //remove the old crop
            }
            spot.plantedObject = Instantiate(spot.currentCrop.growthStages[i], spot.spotTransform.position, Quaternion.identity, spot.spotTransform);
            spot.currentStage = i; //update the current stage
        }
    }
    
    public override string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public override void Interact(PlayerController player)
    {
        //Check if player has seeds to plant
        InventorySO inventory = player.GetComponent<PlayerInventoryHolder>().inventory;
        if (inventory.SelectedItem is CropData cropData)
        {
            PlantCrop(cropData);
            Debug.Log($"Planted {cropData.cropName} in the garden bed.");
        }
        else
        {
            if (inventory.SlotCount != inventory.maxSlots)
            {
                HarvestCrop(player);
            }
            else
            {
                Debug.Log("Inventory is full, cannot harvest crops.");
            }

        }
    }
}
