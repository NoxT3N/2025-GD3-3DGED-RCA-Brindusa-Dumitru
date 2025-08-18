using UnityEngine;

public class PenguinNPC : Interactable
{
    public override string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public override void Interact(PlayerController player)
    {
        Debug.Log("Penguin NPC interaction triggered.");
    }


   public InventorySO Pinventory;
  
    void Awake()
    {
        base.Awake();
    }


    void Update()
    {
        
    }
}
