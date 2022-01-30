using AudioSystem.Agents;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Dog : MonoBehaviour
{
    public static bool CreateDogToLoc(Transform player, Vector3 scoutLocation)
    {
        NavMeshPath temp = new NavMeshPath();
        if(NavMesh.CalculatePath(player.position, scoutLocation, NavMesh.AllAreas, temp))
        {
            GameObject dogObject = GameObject.Instantiate(GameManager.Instance.m_dogPrefab, player.position, Quaternion.LookRotation((scoutLocation - player.position).normalized, Vector3.up));
            dogObject.GetComponent<Dog>().Awake();
            dogObject.GetComponent<Dog>().owner = player;
            dogObject.GetComponent<Dog>().SetTargetDestination(scoutLocation);
            return true;
        }
        return false;
    }

    public Transform owner;
    protected NavMeshAgent m_myLegs;
    protected Animator m_myAnimator;
    [SerializeField] private MultiAudioAgent m_audioAgent;

    [SerializeField] private GameObject m_detectVFX;

    public enum DogState
    {
        Scout,
        Wait,
        Follow,
        Return,
    }

    public DogState m_currentState = DogState.Scout;

    public float m_stopRange = 0.25f;
    public float m_waitTime = 5.0f;
    public float m_detectRange = 7f;
    public float m_wolfTrackTime = 7f;
    public float m_rotationSpeed = 120.0f;

    private Quaternion m_targetRotation = Quaternion.identity;

    private Vector3 m_scoutLoc;
    private float m_timer = 0;
    private bool m_hasFoundWolf = false;
    private Wolf m_wolfBody = null;

    protected virtual void Awake()
    {
        GameManager.Instance.m_dog = this;
        m_myLegs = GetComponent<NavMeshAgent>();
        m_myAnimator = GetComponentInChildren<Animator>();
        m_myLegs.updateRotation = false;
        TransitionTo(DogState.Scout);
    }
    
    protected virtual void Start()
    {
        m_audioAgent.Play("DogSqueak", true);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
        
        //Note to self: This will fk up with slopes
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        m_myAnimator.SetBool("IsMoving", m_myLegs.velocity.magnitude > 0.25f);
        m_myAnimator.SetBool("IsBarking", m_currentState == DogState.Follow);
        m_detectVFX?.SetActive(m_currentState == DogState.Wait);

        if (m_timer > 0)
            m_timer -= Time.deltaTime;

        switch (m_currentState)
        {
            case DogState.Scout:
                m_myLegs.SetDestination(m_scoutLoc);
                SetTargetRotationTo(m_scoutLoc);
                if (m_myLegs.IsNearDestination(m_stopRange))
                {
                    TransitionTo(DogState.Wait);
                }
                break;
            case DogState.Wait:
                if (!m_hasFoundWolf)
                {
                    List<Wolf> localWolves = GetWolvesWithinRange();
                    if (localWolves.Count > 0)
                    {
                        foreach (var wolf in localWolves)
                        {
                            wolf.Reveal();
                            m_waitTime = m_wolfTrackTime;
                        }

                        m_wolfBody = this.GetClosest<Wolf>(localWolves);
                        TransitionTo(DogState.Follow);
                    }
                }
                if (m_timer <= 0)
                {
                    TransitionTo(DogState.Return);
                }
                break;
            case DogState.Follow:
                if(!GameManager.Instance.m_wolfList.Contains(m_wolfBody))
                    TransitionTo(DogState.Return);

                m_myLegs.SetDestinationNear(m_wolfBody.transform.position, 0.75f, 1.25f);
                SetTargetRotationTo(m_wolfBody.transform.position);
                if (m_timer <= 0)
                {
                    TransitionTo(DogState.Return);
                }
                break;
            case DogState.Return:
                m_myLegs.SetDestination(owner.transform.position);
                SetTargetRotationTo(owner.transform.position);
                if (m_myLegs.IsNearDestination(m_stopRange))
                {
                    GameManager.Instance.m_dog = null;
                    Destroy(gameObject);
                    return;
                }
                break;
            default:
                break;
        }
    }

    public List<Wolf> GetWolvesWithinRange()
    {
        List<Wolf> withinRange = new List<Wolf>();
        foreach (var wolf in GameManager.Instance.m_wolfList)
        {
            if(Vector3.Distance(transform.position, wolf.transform.position) <= m_detectRange)
            {
                withinRange.Add(wolf);
            }
        }
        return withinRange;
    }

    public void SetTargetRotation(Quaternion rotation)
    {
        m_targetRotation = rotation;
    }

    private void SetTargetRotationTo(Vector3 target)
    {
        m_targetRotation = Quaternion.LookRotation((target - transform.position).normalized, Vector3.up);
    }

    public void SetTargetDestination(Vector3 pos)
    {
        m_scoutLoc = pos; 
    }

    private void TransitionTo(DogState state)
    {
        if (m_currentState == state)
            return;

        m_currentState = state;

        switch (m_currentState)
        {
            case DogState.Scout:
                break;
            case DogState.Wait:
                m_timer = m_waitTime;
                break;
            case DogState.Follow:
                m_timer = m_wolfTrackTime;
                break;
            case DogState.Return:
                m_timer = 0;
                break;
            default:
                break;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if(m_myLegs != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_myLegs.destination, 0.25f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);

#if UNITY_EDITOR
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, Vector3.up, m_detectRange);
#endif
        }
    }

    public void Bark()
    {
        m_audioAgent.Play($"DogBark{Random.Range(1, 5)}", false, Random.Range(0.85f, 1.25f));
    }
}
