using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletVFX : MonoBehaviour
{
    [SerializeField] private float m_speed = 20.0f;
    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] private GameObject m_trail;
    public LayerMask m_collisionMask;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody.velocity = transform.forward * m_speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & m_collisionMask) != 0)
        {
            m_trail.transform.SetParent(null);
            m_trail.GetComponent<VFXTimerScript>();
            Destroy(this);
        }
    }
}
