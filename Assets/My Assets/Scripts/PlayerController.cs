using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Runtime.CompilerServices;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    private CustomInput input;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Point and Click Settings")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] ParticleSystem clickEffect;

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
            if (clickEffect != null)
            {
                //Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                StartCoroutine(PlayClickEffect(hit.point + new Vector3(0, 0.1f, 0)));
            }
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

    IEnumerator PlayClickEffect(Vector3 position)
    {
        ParticleSystem effect = Instantiate(clickEffect, position, clickEffect.transform.rotation);
        effect.Play();
        yield return new WaitForSeconds(effect.main.duration);
        Destroy(effect.gameObject);
    }

}
