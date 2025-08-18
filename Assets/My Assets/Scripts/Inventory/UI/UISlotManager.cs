using System.Linq;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UISlotManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Image[] slotIcons;
    [SerializeField] private TextMeshProUGUI[] slotQuantities;
    [SerializeField] private Color selectedColour;
    [SerializeField] private Color defaultColour;
    private int selectedIndex;

    private InventorySO playerInventory;


    private void Start()
    {
        playerInventory = player.GetComponent<PlayerInventoryHolder>().inventory;
        if (playerInventory != null)
        {
            playerInventory.OnInventoryUpdated.AddListener(UpdateUI);
            playerInventory.OnItemSelected.AddListener(UpdateUI);
        }
    
        SelectSlot(0);
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryUpdated.RemoveListener(UpdateUI);
            playerInventory.OnItemSelected.RemoveListener(UpdateUI);
        }
    }

    private void Update()
    {
       
    }

    public void SelectSlot(int index)
    {
        if (selectedIndex != index)
        {
            slotIcons[selectedIndex].color = defaultColour;
            selectedIndex = index;
            slotIcons[selectedIndex].color = selectedColour;
        }
    }


    public void UpdateUI()
    {
        //update slot icons and quantities
        for (int i = 0; i < playerInventory.SlotCount; i++)
        {
            if (i < slotIcons.Length)
            {
                var slot = playerInventory.Slots.ElementAtOrDefault(i);
                if (slot != null && slot.item != null)
                {
                    slotIcons[i].sprite = slot.item.icon;
                    slotIcons[i].enabled = true;
                    slotQuantities[i].text = slot.quantity.ToString();
                }
                else
                {
                    slotIcons[i].sprite = null;
                    slotIcons[i].enabled = false;
                    slotQuantities[i].text = string.Empty;
                }
            }
        }
    }
    
    public void UpdateUI(int selectedIndex)
    {
        SelectSlot(selectedIndex);
        UpdateUI();
    }
}
