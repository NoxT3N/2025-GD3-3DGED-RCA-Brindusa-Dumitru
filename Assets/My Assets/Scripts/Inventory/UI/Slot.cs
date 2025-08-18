using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UISlotManager slotManager;
    [SerializeField] private int slotIndex;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        slotManager.SelectSlot(slotIndex);
        Debug.Log($"Slot {slotIndex} clicked");
    }
}
