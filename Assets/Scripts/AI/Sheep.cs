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

    public GameObject m_shotVFXPrefab;

    public GameObject m_namePlate;
    protected TextMesh m_namePlateText;
    public float m_softPlateDist = 10;
    public float m_hardPlateDist = 5;
    protected virtual void Awake()
    {
        m_myLegs = GetComponentInChildren<NavMeshAgent>();
        m_sheepAnimControl = GetComponentInChildren<Animator>();
        m_defaultAcc = m_myLegs.acceleration;
        m_namePlateText = m_namePlate.GetComponentInChildren<TextMesh>();
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
        if (!(this is Wolf))
        {
            if (PauseUpdate())
                return;
        }

        if (m_isDead)
        {
            m_sheepAnimControl.SetBool("IsDead", m_isDead);
            m_myLegs.isStopped = true;
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
            if (PauseUpdate())
                return;
            AnimationUpdate();
            UpdateNamePlate(m_name);
        }
    }

    protected void UpdateNamePlate(string name)
    {
        if(PlayerPrefs.GetInt("GameJamMode", 1) == 1)
        {
            m_namePlate.SetActive(false);
            return;
        }

        m_namePlateText.text = name;

        float playerDist = Vector3.Distance(transform.position, GameManager.Instance.m_playerCamera.transform.position);
        bool zoomed = GameManager.Instance.m_playerCamera.GetComponentInParent<Player_Camera>().m_isScoped;
        m_namePlate.SetActive(playerDist <= m_softPlateDist || zoomed);
        float alpha = 1.0f;

        if (m_namePlate.activeInHierarchy)
        {
            if (zoomed)
            {
                alpha = Mathf.Clamp((playerDist - m_hardPlateDist) / (m_softPlateDist - m_hardPlateDist), 0.0f, 1.0f);
            }
            else
            {
                alpha = Mathf.Clamp(1.0f - (playerDist - m_hardPlateDist) / (m_softPlateDist - m_hardPlateDist), 0.0f, 1.0f);
            }
                

            foreach (var item in m_namePlate.GetComponentsInChildren<Renderer>())
            {
                item.material.color = new Color(1, 1, 1, alpha);
            }
        }
    }

    protected virtual void AnimationUpdate()
    {
        m_sheepAnimControl.SetBool("IsDead", m_isDead);
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
            Instantiate(m_shotVFXPrefab, transform.position + transform.up * 0.5f, transform.rotation);
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

    protected virtual bool PauseUpdate()
    {
        m_myLegs.isStopped = PauseMenu.isPaused;

        m_sheepAnimControl.speed = (PauseMenu.isPaused) ? 0.0f : 1.0f;
        return PauseMenu.isPaused;
    }
}
