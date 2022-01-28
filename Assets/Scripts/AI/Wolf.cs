using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : Sheep
{
    public enum AIState
    {
        MovingToPack,
        Blend,
        Hunt,
    }

    public AIState m_currentState = AIState.MovingToPack;

    [Header("Wolf Settings")]
    public SheepPack m_targetPack;
    public Sheep m_targetSheep;

    public float m_rangeTillBlend = 2f;
    public float m_timeTillKill = 15.0f;
    public float m_KillTimeMin = 15.0f;
    public float m_KillTimeMax = 30.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        m_targetPack = GameManager.Instance.GetNearestPack(transform.position);

        m_myLegs.SetDestination(m_targetPack.GetAveragePosition());
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector3 moveTarget = m_targetPack.GetAveragePosition();

        switch (m_currentState)
        {
            case AIState.MovingToPack:
                m_myLegs.SetDestination(moveTarget);
                if (Vector3.Distance(moveTarget, transform.position) < m_rangeTillBlend)
                {
                    TransitionTo(AIState.Blend);
                }
                break;
            case AIState.Blend:
                base.Update();
                if(m_isWaitingForDestination || m_targetPack.m_roamRangeMax < Vector3.Distance(moveTarget, transform.position))
                {
                    m_targetPack.GenerateRoamLocation(this);
                }
                if(!IsVisibleByCamera())
                {
                    m_timeTillKill -= Time.deltaTime;
                    if(m_timeTillKill <= 0)
                    {
                        TransitionTo(AIState.Hunt);
                    }
                }
                break;
            case AIState.Hunt:
                m_myLegs.SetDestination(moveTarget);
                break;
            default:
                break;
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
                break;
            case AIState.Blend:
                m_targetPack.GenerateRoamLocation(this);
                m_timeTillKill = Random.Range(m_KillTimeMin, m_KillTimeMax);
                break;
            case AIState.Hunt:
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

    public override void Kill(bool fromShot = false)
    {
        GameManager.Instance.m_wolfList.Remove(this);

        Destroy(gameObject);
    }
}
