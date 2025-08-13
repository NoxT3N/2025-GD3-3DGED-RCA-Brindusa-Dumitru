using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Runtime.CompilerServices;
public class PlayerController : MonoBehaviour
{
    private CustomInput input;
    private NavMeshAgent agent;
    private Animator animator;
    [SerializeField] LayerMask groundLayer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //animator.SetBool("IsMoving", false);

        input = new CustomInput();
        AssignInputs();
    }
    void Update()
    {
        SetAnimations();
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

    void SetAnimations()

    {
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);
    }

}
