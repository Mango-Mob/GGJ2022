using AudioSystem.Agents;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    protected NavMeshAgent m_myLegs;

    [Header("Sheep Stats")]
    public float m_roamDist = 5f;
    public float m_stoppingDistance = 0.5f;
    public float m_waitMin = 1.0f;
    public float m_waitMax = 2.0f;
    public float m_shotReactRange = 5.0f;
    public float m_maxReactDist = 5.0f;

    private Coroutine m_currentScaredRoutine;
    private float m_scaredDuration;
    protected Vector3 m_target;
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
        if(m_myLegs.IsNearDestination(m_stoppingDistance))
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

    public virtual void Kill(bool fromShot = false)
    {
        GetComponentInParent<SheepPack>().Destroy(this);
    }

    public virtual void ReactToGunFire(Vector3 shotLoc)
    {
        Vector3 direct = shotLoc.GetDirectionTo(transform.position, true);

        float dist = Vector3.Distance(shotLoc, transform.position);
        float mag = 1.0f - (dist/m_shotReactRange);

        if(mag > 0.0f && mag <= 1.0f)
        {
            m_target = transform.position + direct * (mag * m_maxReactDist);
            m_myLegs.SetDestination(m_target);
            m_myLegs.velocity = Vector3.zero;

            if (!(this is Wolf))
                StartScaredRoutine(mag);

            m_isWaitingForDestination = false;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_target, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }

    public void StartScaredRoutine(float magnitude)
    {
        if (m_scaredDuration > 0)
            StopCoroutine(m_currentScaredRoutine);

        GetComponentInParent<MultiAudioAgent>().PlayOnce("SheepAlert", false, Random.Range(0.85f, 1.25f));
        m_currentScaredRoutine = StartCoroutine(ScaredRoutine(m_myLegs.acceleration * (2.5f * magnitude), m_myLegs.acceleration, 10.0f * magnitude));
    }

    public IEnumerator ScaredRoutine(float startAccel, float endAccel, float duration)
    {
        if (m_myLegs != null)
        {
            if(m_scaredDuration <= 0)
            {
                m_myLegs.angularSpeed *= 1.5f;
                m_myLegs.speed *= 1.5f;
            }

            m_scaredDuration = duration;

            m_myLegs.acceleration = startAccel;

            while (m_scaredDuration > 0)
            {
                m_myLegs.acceleration = Mathf.Lerp(startAccel, endAccel, 1.0f - (m_scaredDuration / duration));

                yield return new WaitForEndOfFrame();
                m_scaredDuration -= Time.deltaTime;
            }

            m_scaredDuration = 0.0f;
            m_myLegs.angularSpeed /= 1.5f;
            m_myLegs.speed /= 1.5f;
            m_myLegs.acceleration = endAccel;
        }
    }
}
