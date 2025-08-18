using UnityEngine;
using System.Collections;

public class GardenBed : Interactable
{
    [System.Serializable]
    public class PlantingSpot
    {
        public Transform spotTransform;
        public CropData currentCrop;
        public GameObject plantedObject;
        public int currentStage = 0;
        public bool isReady => currentCrop != null && currentStage == currentCrop.growthStages.Length - 1;
    }

    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = true;
    [SerializeField] private float growthSpeedMultiplier = 1f; // Set to 10 for demo
    
    [Header("Planting Spots")]
    [SerializeField] private PlantingSpot[] plantingSpots = new PlantingSpot[4];

    protected override void Awake()
    {
        base.Awake();
        
        if (debugMode) Debug.Log($"Initializing GardenBed: {name}");

        // Initialize array if empty
        if (plantingSpots == null || plantingSpots.Length == 0)
        {
            plantingSpots = new PlantingSpot[4];
            if (debugMode) Debug.Log("Reset planting spots array");
        }

        // Find planting spots
        int foundSpots = 0;
        foreach (Transform child in transform)
        {
            if (foundSpots >= plantingSpots.Length) break;
            
            if (child.name.StartsWith("crop_spawn"))
            {
                if (plantingSpots[foundSpots] == null)
                {
                    plantingSpots[foundSpots] = new PlantingSpot();
                }
                
                plantingSpots[foundSpots].spotTransform = child;
                foundSpots++;
                
                if (debugMode) Debug.Log($"Assigned spot {foundSpots} to {child.name}");
            }
        }
        
        if (debugMode) Debug.Log($"Found {foundSpots} planting spots");
    }

    public bool PlantCrop(CropData seeds)
    {
        if (seeds == null)
        {
            if (debugMode) Debug.LogError("Tried to plant null seeds!");
            return false;
        }

        foreach (PlantingSpot spot in plantingSpots)
        {
            if (spot.currentCrop == null)
            {
                if (debugMode) Debug.Log($"Planting {seeds.name} at {spot.spotTransform.position}");
                
                spot.currentCrop = seeds;
                spot.plantedObject = Instantiate(
                    seeds.growthStages[0], 
                    spot.spotTransform.position, 
                    Quaternion.identity, 
                    spot.spotTransform
                );
                
                StartCoroutine(GrowCrop(spot));
                return true;
            }
        }
        
        if (debugMode) Debug.Log("No empty planting spots available");
        return false;
    }

    public bool HarvestCrop(PlayerController player)
    {
        PlayerInventoryHolder inventoryHolder = player.GetComponent<PlayerInventoryHolder>();
        if (inventoryHolder == null || inventoryHolder.inventory == null)
        {
            if (debugMode) Debug.LogError("Player inventory missing!");
            return false;
        }

        foreach (PlantingSpot spot in plantingSpots)
        {
            if (spot.isReady)
            {
                if (debugMode) Debug.Log($"Harvesting {spot.currentCrop.HarvestedItem.name} from {spot.spotTransform.position}");
                
                // Try to add to inventory
                inventoryHolder.inventory.AddItem(spot.currentCrop.HarvestedItem, spot.currentCrop.yieldAmount);
                
                // Clear spot
                Destroy(spot.plantedObject);
                spot.currentCrop = null;
                spot.plantedObject = null;
                spot.currentStage = 0;
                
                return true;
            }
        }
        
        if (debugMode) Debug.Log("No crops ready to harvest");
        return false;
    }

    private IEnumerator GrowCrop(PlantingSpot spot)
    {
        if (spot.currentCrop == null) yield break;
        
        int stages = spot.currentCrop.growthStages.Length;
        
        for (int i = 1; i < stages; i++)
        {
            float stageTime = spot.currentCrop.growthTimePerStage / growthSpeedMultiplier;
            
            if (debugMode) Debug.Log($"Growing stage {i}/{stages} for {spot.currentCrop.name} (waiting {stageTime}s)");
            
            yield return new WaitForSeconds(stageTime);
            
            if (spot.plantedObject != null)
            {
                Destroy(spot.plantedObject);
            }

            spot.plantedObject = Instantiate(
                spot.currentCrop.growthStages[i], 
                spot.spotTransform.position, 
                Quaternion.identity, 
                spot.spotTransform
            );
            
            spot.currentStage = i;
        }
        
        if (debugMode) Debug.Log($"Crop fully grown: {spot.currentCrop.name}");
    }
    
    public override string GetInteractionPrompt()
    {
        foreach (PlantingSpot spot in plantingSpots)
        {
            if (spot.isReady) return "[E] Harvest";
            if (spot.currentCrop == null) return "[E] Plant";
        }
        return "[E] Garden Bed";
    }

    public override void Interact(PlayerController player)
    {
        if (debugMode) Debug.Log("Garden bed interacted");
        
        PlayerInventoryHolder inventoryHolder = player.GetComponent<PlayerInventoryHolder>();
        if (inventoryHolder == null || inventoryHolder.inventory == null)
        {
            Debug.LogError("Player inventory missing!");
            return;
        }

        InventorySO inventory = inventoryHolder.inventory;
        
        // Try planting if holding seeds
        if (inventory.SelectedItem is CropData seeds)
        {
            if (PlantCrop(seeds))
            {
                inventory.RemoveItem(seeds, 1);
            }
        }
        else // Try harvesting
        {
            HarvestCrop(player);
        }
    }
}