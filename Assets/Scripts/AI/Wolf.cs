using AudioSystem.Agents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : Sheep
{
    //Wolf names
    public enum AIState
    {
        MovingToPack,
        Blend,
        Hunt,
        Berserk,
        Idle,
    }

    public AIState m_currentState = AIState.MovingToPack;
    public bool isBeingWatched = false;

    [Header("Wolf Settings")]
    public GameObject m_sheepBody;
    public GameObject m_wolfBody;

    public SheepPack m_targetPack;
    public Sheep m_targetSheep;

    public float m_blendSpeed = 2.5f;
    public float m_unblendSpeed = 4.5f;
    public float m_roamDistance = 6.0f;

    public float m_rangeTillBlend = 2f;
    public float m_timeTillKill = 10.0f;
    public float m_KillTimeMin = 10.0f;
    public float m_KillTimeMax = 15.0f;
    public float m_killRange = 2.0f;

    public GameObject m_poofVFX;
    private MultiAudioAgent m_audioAgent;

    public string m_fakeName;
    protected override void Awake()
    {
        base.Awake();
        m_audioAgent = GetComponent<MultiAudioAgent>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        m_targetPack = this.GetClosest<SheepPack>(GameManager.Instance.m_packOfSheep);

        m_target = Vector3.zero;
        m_myLegs.speed = m_unblendSpeed;
        if (m_targetPack != null)
            m_myLegs.SetDestination(m_targetPack.GetAveragePosition());

        int selection = Random.Range(0, GameManager.Instance.m_wolfNames.Count);
        m_name = GameManager.Instance.m_wolfNames[selection];
        GameManager.Instance.m_wolfNames.RemoveAt(selection);

        selection = Random.Range(0, GameManager.Instance.m_sheepNames.Count);
        m_fakeName = GameManager.Instance.m_sheepNames[selection];
        GameManager.Instance.m_sheepNames.RemoveAt(selection);
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector3 moveTarget = m_targetPack != null ? m_targetPack.GetAveragePosition() : m_target;
        isBeingWatched = IsVisibleByCamera();

        if(GameManager.Instance.m_ammoCount <= 0 && (m_currentState != AIState.Berserk || m_currentState != AIState.Idle))
        {
            Reveal();
        }

        switch (m_currentState)
        {
            case AIState.MovingToPack:
                m_myLegs.speed = IsVisibleByCamera() ? m_blendSpeed : m_unblendSpeed;
                if (isBeingWatched)
                {
                    m_targetPack = null;
                    TransitionTo(AIState.Blend);
                }
                else
                {
                    m_myLegs.SetDestination(moveTarget);
                    if (m_myLegs.IsNearDestination(m_rangeTillBlend))
                    {
                        TransitionTo(AIState.Blend);
                    }
                }
                break;
            case AIState.Blend:
                base.Update();
                if(m_targetPack != null)
                {
                    if (m_isWaitingForDestination || m_myLegs.IsNearDestination(m_rangeTillBlend))
                    {
                        m_targetPack.GenerateRoamLocation(this);
                    }
                    if (!isBeingWatched)
                    {
                        m_timeTillKill -= Time.deltaTime;
                        if (m_timeTillKill <= 0)
                        {
                            TransitionTo(AIState.Hunt);
                        }
                    }
                }
                else
                {
                    if (m_isWaitingForDestination || m_myLegs.IsNearDestination(m_rangeTillBlend))
                    {
                        SetTarget(GetRoamLocation());
                    }
                    if (!isBeingWatched)
                    {
                        TransitionTo(AIState.MovingToPack);
                    }
                }
                break;
            case AIState.Hunt:
                if(m_targetSheep != null)
                    m_myLegs.SetDestination(m_targetSheep.transform.position);
                else if (m_targetPack != null)
                    m_targetSheep = m_targetPack.GetClosestSheep(transform.position);
                else
                    TransitionTo(AIState.MovingToPack);

                if (isBeingWatched)
                    TransitionTo(AIState.Blend);

                if (m_myLegs.IsNearDestination(m_killRange))
                {
                    if(m_targetPack.m_sheepList.Count > 1)
                    {
                        string name = m_targetSheep.Kill(false);
                        if (name != string.Empty)
                        {
                            KillFeedManager.Instance.DisplayWolfKill(m_name, name);
                        }
                        m_audioAgent.Play($"WolfAttack{Random.Range(1, 3)}", false, Random.Range(0.85f, 1.25f));
                        m_timeTillKill = Random.Range(m_KillTimeMin, m_KillTimeMax);
                        TransitionTo(AIState.Blend);
                    }
                    else
                    {
                        string name = m_targetSheep.Kill(false);
                        if (name != string.Empty)
                        {
                            KillFeedManager.Instance.DisplayWolfKill(m_name, name);
                        }
                        m_audioAgent.Play($"WolfAttack{Random.Range(1, 3)}", false, Random.Range(0.85f, 1.25f));
                        m_timeTillKill = Random.Range(m_KillTimeMin, m_KillTimeMax);
                        TransitionTo(AIState.MovingToPack);
                    }
                }
                break;
            case AIState.Berserk:
                if (m_targetSheep == null)
                {
                    Sheep nextTarget = GameManager.Instance.GetClosestSheep(transform.position);
                    
                    if(nextTarget != null)
                    {
                        m_targetSheep = nextTarget;
                    }
                    else
                    {
                        TransitionTo(AIState.Idle);
                        return;
                    }
                }
                m_myLegs.SetDestination(m_targetSheep.transform.position);

                m_timeTillKill -= Time.deltaTime;

                if (m_myLegs.IsNearDestination(m_killRange) && m_timeTillKill < 0)
                {
                    m_timeTillKill = 1.5f;
                    string name = m_targetSheep.Kill(false);
                    if(name != string.Empty)
                    {
                        KillFeedManager.Instance.DisplayWolfKill(m_name, name);
                    }

                    m_audioAgent.Play($"WolfAttack{Random.Range(1, 3)}", false, Random.Range(0.85f, 1.25f));
                    if (m_targetPack != null && m_targetPack.m_sheepList.Count == 0)
                    {
                        m_targetPack = null;
                    }
                }
                break;
            default:
                m_myLegs.isStopped = true;
                break;
        }
    }


    protected override void AnimationUpdate()
    {
        if(m_sheepBody.activeInHierarchy)
            base.AnimationUpdate();
        else
        {
            
        }
    }
    private void TransitionTo(AIState state)
    {
        if (m_currentState == state)
            return;

        m_currentState = state;

        switch (m_currentState)
        {
            case AIState.MovingToPack:
                m_myLegs.speed = m_unblendSpeed;
                m_targetPack = this.GetClosest<SheepPack>(GameManager.Instance.m_packOfSheep);
                m_target = Vector3.zero;

                if(m_targetPack != null)
                    m_myLegs.SetDestination(m_targetPack.GetAveragePosition());

                m_timeTillKill = Random.Range(m_KillTimeMin, m_KillTimeMax);
                break;
            case AIState.Blend:
                if(m_targetPack == null)
                {
                    SetTarget(GetRoamLocation());
                }
                else
                {
                    m_targetPack.GenerateRoamLocation(this);
                }
                m_myLegs.speed = m_blendSpeed;
                break;
            case AIState.Hunt:
                m_targetSheep = m_targetPack.GetClosestSheep(transform.position);
                m_myLegs.speed = m_unblendSpeed;
                break;
            case AIState.Berserk:
                m_audioAgent.Play("WolfAngry");
                m_myLegs.speed = m_unblendSpeed;
                m_myLegs.angularSpeed *= 1.5f;
                m_myLegs.acceleration *= 1.5f;
                m_timeTillKill = 0;
                if (m_targetPack == null)
                {
                    if (GameManager.Instance.m_packOfSheep.Count > 0)
                    {
                        TransitionTo(AIState.Idle);
                        return;
                    }
                    m_targetPack = this.GetClosest<SheepPack>(GameManager.Instance.m_packOfSheep);
                    
                }
                m_targetSheep = m_targetPack.GetClosestSheep(transform.position);
                break;
            default:
                break;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if(m_targetPack != null)
        {
            Vector3 pos = m_targetPack.GetAveragePosition();
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }

    private bool IsVisibleByCamera()
    {
        Vector3 viewPos = GameManager.Instance.m_playerCamera.WorldToViewportPoint(transform.position);
        
        return (viewPos.x <= 1.0f && viewPos.x >= 0.0f) && (viewPos.y <= 1.0f && viewPos.y >= 0.0f) && viewPos.z > 0;
    }

    private Vector3 GetRoamLocation()
    {
        Vector3 direction = Random.insideUnitSphere;
        direction.y = 0;

        return transform.position + direction.normalized * m_roamDistance;
    }

    public override string Kill(bool fromShot = false)
    {
        GameManager.Instance.m_wolfList.Remove(this);
        Reveal(false);
        TransitionTo(AIState.Idle);
        m_audioAgent.Play("WolfReveal");
        Destroy(gameObject, 5f);
        return m_name;
    }

    public void Reveal(bool berserk = true)
    {
        if(!m_wolfBody.activeInHierarchy)
        {
            if(berserk)
                TransitionTo(AIState.Berserk);
            m_audioAgent.Play("WolfReveal");
            m_poofVFX.SetActive(true);
            m_sheepBody.SetActive(false);
            m_wolfBody.SetActive(true);
        }
    }
}
