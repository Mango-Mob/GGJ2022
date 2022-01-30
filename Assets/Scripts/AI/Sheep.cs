using AudioSystem.Agents;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    protected NavMeshAgent m_myLegs;
    protected Animator m_sheepAnimControl;

    [Header("Sheep Stats")]
    public float m_roamDist = 5f;
    public float m_stoppingDistance = 0.5f;
    public float m_waitMin = 1.0f;
    public float m_waitMax = 2.0f;
    public float m_shotReactRange = 5.0f;
    public float m_maxReactDist = 5.0f;
    public float m_chanceToFidget = 0.05f;

    private Coroutine m_currentScaredRoutine;
    private float m_scaredDuration;
    protected Vector3 m_target;
    private float m_waitTime;

    public bool m_isDead = false;
    public bool m_isWaitingForDestination = true;

    public string m_name = "Sheep";

    private float m_timer;
    private float m_defaultAcc;

    protected virtual void Awake()
    {
        m_myLegs = GetComponentInChildren<NavMeshAgent>();
        m_sheepAnimControl = GetComponentInChildren<Animator>();
        m_defaultAcc = m_myLegs.acceleration;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        int selection = Random.Range(0, GameManager.Instance.m_sheepNames.Count);
        m_name = GameManager.Instance.m_sheepNames[selection];
        GameManager.Instance.m_sheepNames.RemoveAt(selection);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(m_isDead)
        {
            m_myLegs.isStopped = true;
            return;
        }
            
        if (m_myLegs.IsNearDestination(m_stoppingDistance))
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

        if(!(this is Wolf))
        {
            AnimationUpdate();
        }
    }

    protected virtual void AnimationUpdate()
    {
        m_sheepAnimControl.SetBool("IsMoving", m_myLegs.velocity.magnitude > 0.25f);
        float mod = (m_myLegs.acceleration - m_defaultAcc) / m_defaultAcc;
        m_sheepAnimControl.SetFloat("velocityMod", mod);

        if (m_timer < m_waitTime)
        {
            if (Random.Range(0, 1000f) < m_chanceToFidget * 1000)
            {
                m_sheepAnimControl.SetTrigger("IsFidget");
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

    public virtual string Kill(bool fromShot = false)
    {
        GetComponentInParent<SheepPack>().Destroy(this, fromShot);
        m_isDead = true;
        m_sheepAnimControl.SetBool("IsDead", m_isDead);
        if (fromShot)
        {
            m_sheepAnimControl.gameObject.SetActive(false);
        }
        else
        {
            GetComponentInChildren<Collider>().enabled = false;
        }
        return m_name;
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

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 5f);
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
