using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPingVFX : MonoBehaviour
{
    private SpriteRenderer m_sprite;
    private VFXTimerScript m_vfxTimer;
    public GameObject m_dogModel;

    public float m_rotateSpeed = 10.0f;
    public float m_bounceFrequency = 1.0f;
    public float m_bounceAmplitude = 1.0f;
    public float m_spawnSpeed = 3.0f;
    public float m_dogMoveRate = 3.0f;

    private Vector3 m_startLocation;
    private float m_targetScale;
    private float m_startingTime;

    private bool m_moveDog = false;

    // Start is called before the first frame update
    void Start()
    {
        m_sprite = GetComponentInChildren<SpriteRenderer>();
        m_vfxTimer = GetComponentInChildren<VFXTimerScript>();
        m_startingTime = m_vfxTimer.m_timer;
        m_startLocation = m_sprite.transform.position;

        m_targetScale = transform.localScale.y;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_vfxTimer.m_timer < 0.5f)
            m_moveDog = true;

        if (m_moveDog)
        {
            m_dogModel.transform.position -= transform.up * m_dogMoveRate;
        }

        if (transform.localScale.x < m_targetScale)
        {
            transform.localScale += Vector3.one * Time.deltaTime * m_spawnSpeed;
        }
        else
        {
            transform.localScale = Vector3.one * m_targetScale;
        }

        m_sprite.transform.position = m_startLocation + m_bounceAmplitude * Vector3.up * Mathf.Cos(m_bounceFrequency * Time.realtimeSinceStartup);

        transform.Rotate(Vector3.up * Time.deltaTime * m_rotateSpeed);

        Color color = m_sprite.color;
        color.a -= Time.deltaTime / m_startingTime;
        m_sprite.color = color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Dog>())
        {
            m_moveDog = true;
            Debug.Log("Dog Detected");
        }
    }
}
