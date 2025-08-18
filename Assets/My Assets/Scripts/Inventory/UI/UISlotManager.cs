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
    public PlayerController player;
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
        playerInventory.OnItemSelected.AddListener(UpdateSelectedSlot);
    }

    playerInventory.SelectItemByIndex(0); //force first slot selected
}

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryUpdated.RemoveListener(UpdateUI);
            playerInventory.OnItemSelected.RemoveListener(UpdateUI);
        }
    }

    private void UpdateSelectedSlot(int newIndex)
{
    //reset all to default colour
    for (int i = 0; i < slotIcons.Length; i++)
    {
        slotIcons[i].color = defaultColour;
    }

    //highlight only the selected
    if (newIndex >= 0 && newIndex < slotIcons.Length)
    {
        slotIcons[newIndex].color = selectedColour;
        selectedIndex = newIndex;
    }
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
    for (int i = 0; i < slotIcons.Length; i++)
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
    
    public void UpdateUI(int selectedIndex)
    {
        SelectSlot(selectedIndex);
        UpdateUI();
    }
}
