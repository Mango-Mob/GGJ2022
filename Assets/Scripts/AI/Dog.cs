using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dog : MonoBehaviour
{
    public static void CreateDogToLoc(Vector3 spawnLoc, Vector3 scoutLocation)
    {
        scoutLocation.y = 0;
        GameObject dogObject = GameObject.Instantiate(GameManager.Instance.m_dogPrefab, spawnLoc, Quaternion.LookRotation((scoutLocation - spawnLoc).normalized, Vector3.up));
        dogObject.GetComponent<Dog>().Awake();
        dogObject.GetComponent<Dog>().SetTargetDestination(scoutLocation);
    }

    protected NavMeshAgent m_myLegs;

    public float m_stopRange = 0.25f;
    public float m_waitTime = 5.0f;
    private Vector3 m_spawnLoc;

    private bool m_isReturning = false;
    protected virtual void Awake()
    {
        GameManager.Instance.m_dog = this;
        m_myLegs = GetComponent<NavMeshAgent>();
        m_spawnLoc = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_myLegs.IsNearDestination(m_stopRange))
        {
            if(m_isReturning)
            {
                GameManager.Instance.m_dog = null;
                Destroy(gameObject);
                return;
            }

            m_waitTime -= Time.deltaTime;
            if(m_waitTime <= 0)
            {
                m_myLegs.SetDestination(m_spawnLoc);
                m_isReturning = true;
            }
        }
    }

    public void SetTargetDestination(Vector3 pos)
    {
        m_myLegs.SetDestination(pos);

        if (m_myLegs == null)
            GetComponent<NavMeshAgent>().SetDestination(pos);
    }
    protected virtual void OnDrawGizmosSelected()
    {
        if(m_myLegs != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_myLegs.destination, 0.25f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        }
    }
}
