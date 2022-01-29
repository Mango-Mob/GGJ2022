using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletVFX : MonoBehaviour
{
    [SerializeField] private float m_speed = 20.0f;
    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] private GameObject m_trail;
    [SerializeField] private VFXTimerScript m_timer;

    public LayerMask m_collisionMask;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody.velocity = transform.forward * m_speed;
    }

    public void SetEndPoint(Vector3 _position)
    {
        float distanceToTravel = (transform.position - _position).magnitude;
        m_timer.m_timer = distanceToTravel / m_speed;
    }

    private void OnDestroy()
    {
        if (m_trail == null)
            return;

        m_trail.transform.SetParent(null);
        m_trail.GetComponent<VFXTimerScript>().m_startedTimer = true;
        m_trail.GetComponent<VFXTimerScript>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_collisionMask == (m_collisionMask | (1 << other.gameObject.layer)))
        {
            m_trail.transform.SetParent(null);
            m_trail.GetComponent<VFXTimerScript>().m_startedTimer = true;
            m_trail.GetComponent<VFXTimerScript>().enabled = true;
        }
    }
}
