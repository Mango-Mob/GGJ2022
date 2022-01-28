﻿using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    protected NavMeshAgent m_myLegs;
    [Header("Sheep Stats")]
    public float m_roamDist = 5f;
    public float m_stoppingDistance = 0.5f;
    public float m_waitMin = 1.0f;
    public float m_waitMax = 2.0f;

    private Vector3 m_target;
    private float m_waitTime;

    public bool m_isWaitingForDestination = true;

    private float m_timer;
    protected virtual void Awake()
    {
        m_myLegs = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(Vector3.Distance(transform.position, m_target) < m_stoppingDistance)
        {
            if(m_timer < m_waitTime)
            {
                m_timer += Time.deltaTime;
            }
            else if(m_timer >= m_waitTime)
            {
                m_isWaitingForDestination = true;
            }
        }
    }

    public void SetTarget(Vector3 target)
    {
        m_target = target;
        m_myLegs.SetDestination(m_target);

        m_timer = 0f;
        m_waitTime = Random.Range(m_waitMin, m_waitMax);
        m_isWaitingForDestination = false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_target, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }

    public virtual void Kill(bool fromShot = false)
    {
        GetComponentInParent<SheepPack>().Destroy(this);
    }
}
