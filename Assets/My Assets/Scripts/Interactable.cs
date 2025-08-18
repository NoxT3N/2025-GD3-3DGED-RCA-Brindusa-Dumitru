using Mono.Cecil.Cil;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Interactable : MonoBehaviour, IInteractable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Outline outline;
    public float interactionRange = 3f;

    protected virtual void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 2f;

        outline.enabled = false;
    }

    public abstract void Interact(PlayerController player);

    public abstract string GetInteractionPrompt();

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            Interact(player);
        }
        else
        {
            Debug.LogError("PlayerController not found in scene!");
        }
    }
}
