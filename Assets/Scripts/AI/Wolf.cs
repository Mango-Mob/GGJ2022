using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : MonoBehaviour
{
    public enum AIState
    {
        Blend,
        Hunt,
    }

    private NavMeshAgent m_myLegs;

    public SheepPack m_target;

    public void Awake()
    {
        m_myLegs = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_target = GameManager.Instance.GetNearestPack(transform.position);

        m_myLegs.SetDestination(m_target.GetAveragePosition());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if(m_target != null)
        {
            Vector3 pos = m_target.GetAveragePosition();
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
}
