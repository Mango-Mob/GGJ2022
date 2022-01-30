using UnityEngine;
using AudioSystem.Managers;
///
/// <author> Michael Jordan </author> 
/// <year> 2021 </year>
/// 
/// <summary>
/// An abstract parent class for audio agents.
/// </summary>
/// 

namespace AudioSystem.Agents
{
    public abstract class AudioAgent : MonoBehaviour
    {
        [Header("Parent Settings:")] //Local settings
        [Range(0.0f, 1.0f)]
        public float localVolume = 1f;
        [Tooltip("Mutes this agent completely.")]
        public bool isMuted = false;

        public bool IsGlobal = false;

        protected virtual void Awake()
        {
            AudioManager.Instance.agents.Add(this);

            if (isMuted)
                Debug.LogWarning($"Audio agent is muted on awake, location: {gameObject.name}.");
        }

        protected abstract void Update();

        public virtual void SetMute(bool status) { isMuted = status; }

        protected virtual void OnDestroy()
        {
            AudioManager.Instance ?.agents.Remove(this);
        }
    }
}

