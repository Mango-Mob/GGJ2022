using AudioSystem.Agents;
using AudioSystem.Listeners;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AudioSystem.Managers
{
    ///
    /// <author> Michael Jordan </author> 
    /// <year> 2021 </year>
    /// 
    /// <summary>
    /// A global singleton used to handle all listeners and audio agents within the current scene, by
    /// containing the global volume settings.
    /// 
    /// Note: Agents/Listeners are incharge of being added/removed when they are awake/destroyed. 
    /// </summary>
    /// 
    public class AudioManager : SingletonPersistent<AudioManager>
    {

        //Agent and listener lists:
        public List<AudioAgent> agents { get; private set; }
        public List<ListenerAgent> listeners { get; private set; }

        //private array of volumes
        public float[] volumes;
        public float m_globalPitch = 1.0f;
        public float m_globalVolume = 1.0f;

        private bool m_isMutating = false;
        //Volume types: 
        //(Add more to dynamically expand the above array)
        public enum VolumeChannel
        {
            MASTER,
            SOUND_EFFECT,
            MUSIC,
        }

        /// <summary>
        /// Called imediately after creation in the constructor
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            volumes = new float[Enum.GetNames(typeof(AudioManager.VolumeChannel)).Length];
            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i] = PlayerPrefs.GetFloat($"volume{i}", 1.0f);
            }

            agents = new List<AudioAgent>();
            listeners = new List<ListenerAgent>();
        }

        public void SaveData()
        {
            for (int i = 0; i < volumes.Length; i++)
            {
                PlayerPrefs.SetFloat($"volume{i}", volumes[i]);
            }
        }

        private void OnDestroy()
        {
            SaveData();
        }

        /// <summary>
        /// Gets the volume of the type additionally based on the agent's location
        /// </summary>
        /// <param name="type">Volume type to accurately base calculate the volume on.</param>
        /// <param name="agent">The agent to base the 3d volume on. Use NULL instead for gobal volume.</param>
        /// <returns> volume data between 0.0f and 1.0f </returns>
        public float GetVolume(VolumeChannel type, AudioAgent agent)
        {
            if (type == VolumeChannel.MASTER)
                return m_globalVolume * volumes[(int)VolumeChannel.MASTER] * CalculateHearingVolume(agent);

            return m_globalVolume * volumes[(int)VolumeChannel.MASTER] * volumes[(int)type] * CalculateHearingVolume(agent);
        }

        /// <summary>
        /// Makes the agent the only one playing with volume, the others will be muted. 
        /// </summary>
        /// <param name="_agent">Agent to prioritise.</param>
        public void MakeSolo(AudioAgent _agent)
        {
            if (_agent == null) //Edge case
            {
                Debug.LogError($"Agent attempting to be solo is null, ignored function call");
                return;
            }

            //Mute all other agents
            foreach (var agent in agents)
            {
                agent.SetMute(true);
            }

            //Unmute param agent
            _agent.SetMute(false);
        }

        /// <summary>
        /// Unmutes all agents within the scene.
        /// </summary>
        public void UnMuteAll()
        {
            foreach (var agent in agents)
            {
                agent.SetMute(false);
            }
        }

        /// <summary>
        /// Calculates the 3D volume based on the distance from all listeners.
        /// </summary>
        /// <param name="agent">Agent to get the volume of. use NULL for global volume.</param>
        /// <returns>Largest volume returned from all listener calculations.</returns>
        private float CalculateHearingVolume(AudioAgent agent)
        {
            if (listeners.Count == 0) //Edge case
                return 1.0f;

            if (agent == null) //Edge case
                return 1.0f;

            if (agent.IsGlobal)
                return 1.0f;
            
            float max = 0.0f;
            foreach (var listener in listeners)
            {
                max = Mathf.Max(listener.CalculateHearingVol(agent.transform.position), max);
            }
            return max;
        }

        public void PlayAudioTemporary(Vector3 _position, AudioClip _clip, VolumeChannel _channel = VolumeChannel.SOUND_EFFECT)
        {
            GameObject temp = new GameObject();
            temp.transform.position = _position;

            VFXAudioAgent agent = temp.AddComponent<VFXAudioAgent>();
            agent.channel = _channel;
            agent.Play(_clip);
        }

        public async void FadeGlobalVolume(float duration, float finalVal)
        {
            if (!m_isMutating)
            {
                m_isMutating = true;

                float time = 0;
                float startValue = m_globalVolume;

                while (time < duration)
                {
                    m_globalVolume = Mathf.Clamp(Mathf.Lerp(startValue, finalVal, time / duration), 0f, 1f);
                    time += Time.unscaledDeltaTime;
                    await Task.Yield(); //return yield null aka wait till next frame
                }

                m_globalVolume = finalVal;

                m_isMutating = false;
            }
        }
    }

}
