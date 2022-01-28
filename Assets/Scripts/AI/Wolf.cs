﻿using System.Collections;
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
    public bool isBeingWatched = false;

    [Header("Wolf Settings")]
    public SheepPack m_targetPack;
    public Sheep m_targetSheep;

    public float m_blendSpeed = 2.5f;
    public float m_unblendSpeed = 4.5f;
    public float m_roamDistance = 6.0f;

    public float m_rangeTillBlend = 2f;
    public float m_timeTillKill = 15.0f;
    public float m_KillTimeMin = 15.0f;
    public float m_KillTimeMax = 30.0f;
    public float m_killRange = 2.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        m_targetPack = GameManager.Instance.GetNearestPack(transform.position);

        m_target = Vector3.zero;
        m_myLegs.speed = m_unblendSpeed;
        m_myLegs.SetDestination(m_targetPack.GetAveragePosition());
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector3 moveTarget = m_targetPack.GetAveragePosition();
        isBeingWatched = IsVisibleByCamera();

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
                if(m_isWaitingForDestination || m_myLegs.IsNearDestination(m_targetPack.m_roamRangeMax))
                {
                    m_targetPack.GenerateRoamLocation(this);
                }
                if(!isBeingWatched)
                {
                    m_timeTillKill -= Time.deltaTime;
                    if(m_timeTillKill <= 0)
                    {
                        TransitionTo(AIState.Hunt);
                    }
                }
                break;
            case AIState.Hunt:
                m_myLegs.SetDestination(m_targetSheep.transform.position);

                if(!isBeingWatched)
                    TransitionTo(AIState.Blend);

                if (m_myLegs.IsNearDestination(m_killRange))
                {
                    if(m_targetPack.m_sheepList.Count > 1)
                    {
                        m_targetSheep.Kill(false);
                        TransitionTo(AIState.Blend);
                    }
                    else
                    {
                        m_targetSheep.Kill(false);
                        TransitionTo(AIState.MovingToPack);
                    }
                }
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
                m_myLegs.speed = m_unblendSpeed;
                m_targetPack = GameManager.Instance.GetNearestPack(transform.position);
                m_target = Vector3.zero;
                m_myLegs.SetDestination(m_targetPack.GetAveragePosition());
                break;
            case AIState.Blend:
                if(m_targetPack == null)
                {
                    SetTarget(GetRoamLocation());
                }
                else
                {
                    m_targetPack.GenerateRoamLocation(this);
                    m_timeTillKill = Random.Range(m_KillTimeMin, m_KillTimeMax);
                }
                m_myLegs.speed = m_blendSpeed;
                break;
            case AIState.Hunt:
                m_targetSheep = m_targetPack.GetClosestSheep(transform.position);
                m_myLegs.speed = m_unblendSpeed;
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

    public override void Kill(bool fromShot = false)
    {
        GameManager.Instance.m_wolfList.Remove(this);

        Destroy(gameObject);
    }
}
