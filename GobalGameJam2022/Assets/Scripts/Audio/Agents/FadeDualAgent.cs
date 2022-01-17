using AudioSystem.Managers;
using UnityEngine;

namespace AudioSystem.Agents
{
    public class FadeDualAgent : AudioAgent
    {
        [Header("AudioSettings")]
        public AudioClip audioClipA;
        public AudioClip audioClipB;
        public AudioManager.VolumeChannel channel;
        public bool playOnAwake = true;

        [Header("FadeSettings")]
        public AnimationCurve audioACurve;
        public AnimationCurve audioBCurve;

        protected AudioPlayer playerA = null;
        protected AudioPlayer playerB = null;

        protected float timeA, timeB;

        protected override void Awake()
        {
            base.Awake();
            playerA = new AudioPlayer(gameObject, audioClipA);
            playerB = new AudioPlayer(gameObject, audioClipB);

            if (playOnAwake)
            {
                Play();
            }
        }

        protected override void Update()
        {
            AudioManager managerRef = (AudioManager.Instance as AudioManager);

            if (isMuted)
            {
                playerA.SetVolume(0.0f);
                playerB.SetVolume(0.0f);
            }
            else if (managerRef != null)
            {
                playerA.SetVolume(managerRef.GetVolume(channel, this) * localVolume * audioACurve.Evaluate(timeA));
                playerB.SetVolume(managerRef.GetVolume(channel, this) * localVolume * audioBCurve.Evaluate(timeB));
            }

            if (audioACurve.Evaluate(timeA) == 0)
            {
                playerA.Pause();
            }
            else if (!playerA.IsPlaying())
            {
                playerA.Play();
            }

            if (audioBCurve.Evaluate(timeB) == 0)
            {
                playerB.Pause();
            }
            else if (!playerB.IsPlaying())
            {
                playerB.Play();
            }
        }

        protected void Play()
        {
            playerA.SetLooping(true);
            playerB.SetLooping(true);

            if (playerA.GetVolume() == 0)
            {
                playerA.Pause();
            }
            else if (!playerA.IsPlaying())
            {
                playerA.Play();
            }

            if (playerB.GetVolume() == 0)
            {
                playerB.Pause();
            }
            else if (!playerB.IsPlaying())
            {
                playerB.Play();
            }
        }

        protected void Stop()
        {
            playerA.Stop();
            playerB.Stop();
        }

        public void SetFadeValues(float _timeA, float _timeB)
        {
            timeA = _timeA;
            timeB = _timeB;
        }
    }
}
