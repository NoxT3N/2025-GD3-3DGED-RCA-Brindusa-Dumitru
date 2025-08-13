using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    CustomInput input;
    NavMeshAgent agent;
    [SerializeField] LayerMask groundLayer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        input = new CustomInput();
        AssignInputs();

    }

    void AssignInputs()
    {
        input.Player.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 200f, groundLayer))
        {
            agent.SetDestination(hit.point);
        }
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

}
