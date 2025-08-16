using UnityEngine;

public class GardenBed : Interactable
{

    public class SeedSlot
    {
        public CropData plantedCrop;
        public int growthStage;
    }
    public override string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public override void Interact(PlayerController player)
    {
        throw new System.NotImplementedException();
    }


}
