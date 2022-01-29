using AudioSystem.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem.Agents
{
    /// 
    /// <author> Michael Jordan </author> 
    /// <year> 2021 </year>
    /// 
    /// <summary>
    /// A single instance of the AudioAgent mainly used for multible soundeffects e.g. A player character.
    /// </summary>
    /// 
    public class MultiAudioAgent : AudioAgent
    {
        public AudioClip[] audioClips;
        public uint audioPlayersCount = 5;
        public AudioManager.VolumeChannel channel;

        protected Dictionary<string, AudioClip> audioLibrary;
        protected AudioPlayer[] players;

        protected override void Start()
        {
            base.Start();
            audioLibrary = new Dictionary<string, AudioClip>();

            foreach (var item in audioClips)
            {
                audioLibrary.Add(item.name, item);
            }

            if (audioPlayersCount != 0)
                players = new AudioPlayer[audioPlayersCount];

            for (int i = 0; i < audioPlayersCount; i++)
            {
                players[i] = new AudioPlayer(gameObject, null);
            }
        }

        protected override void Update()
        {
            foreach (var player in players)
            {
                if (isMuted)
                    player.SetVolume(0.0f);
                else
                    player.SetVolume(AudioManager.Instance .GetVolume(channel, this) * localVolume);

                player.Update();
            }
        }

        public bool Play(string clipName, bool isLooping = false, float pitch = 1.0f)
        {
            AudioClip clip;
            if (audioLibrary.TryGetValue(clipName, out clip))
            {
                AudioPlayer player = GetAvailablePlayer();
                if (player != null)
                {
                    player.SetClip(clip);
                    player.SetLooping(isLooping);
                    player.SetPitch(pitch);
                    player.Play();
                    return true;
                }
                Debug.LogWarning($"MultiAudioAgent on gameObject: \"{gameObject.name}\" doesn't have enough players to play: \"{clipName}\".");
                return false;
            }
            Debug.LogError($"MultiAudioAgent on gameObject: \"{gameObject.name}\" doesn't contain \"{clipName}\".");
            return false;
        }

        public bool PlayOnce(string clipName, bool isLooping = false, float pitch = 1.0f)
        {
            AudioClip clip;
            if (audioLibrary.TryGetValue(clipName, out clip))
            {
                AudioPlayer player = GetAvailablePlayer();
                if (player != null)
                {
                    if (!IsAudioPlaying(clipName))
                    {
                        player.SetClip(clip);
                        player.SetLooping(isLooping);
                        player.SetPitch(pitch);
                        player.Play();
                        return true;
                    }
                    return false;
                }
                Debug.LogWarning($"MultiAudioAgent on gameObject: \"{gameObject.name}\" doesn't have enough players to play: \"{clipName}\".");
                return false;
            }
            Debug.LogError($"MultiAudioAgent on gameObject: \"{gameObject.name}\" doesn't contain \"{clipName}\".");
            return false;
        }

        public void StopAudio(string clipName)
        {
            foreach (var player in players)
            {
                if (player.IsPlaying() && player.currentClip?.name == clipName)
                {
                    player.Stop();
                }
            }
        }
        public void StopAllAudio()
        {
            foreach (var player in players)
            {
                if (player.IsPlaying())
                {
                    player.Stop();
                }
            }
        }

        public bool IsAudioPlaying(string clipName)
        {
            foreach (var player in players)
            {
                if (player.IsPlaying() && player.currentClip?.name == clipName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAudioPlaying()
        {
            foreach (var player in players)
            {
                if (player.IsPlaying())
                {
                    return true;
                }
            }
            return false;
        }

        private AudioPlayer GetAvailablePlayer()
        {
            foreach (var player in players)
            {
                if (!player.IsPlaying())
                {
                    return player;
                }
            }
            return null;
        }

        public bool PlayDelayed(string clipName, float delay = 1.0f, bool isLooping = false, float pitch = 1.0f)
        {
            AudioClip clip;
            if (audioLibrary.TryGetValue(clipName, out clip))
            {
                AudioPlayer player = GetAvailablePlayer();
                if (player != null)
                {
                    player.SetClip(clip);
                    player.SetLooping(isLooping);
                    player.SetPitch(pitch);
                    player.PlayDelayed(delay);

                    return true;
                }
                Debug.LogWarning($"MultiAudioAgent on gameObject: \"{gameObject.name}\" doesn't have enough players to play: \"{clipName}\".");
                return false;
            }
            Debug.LogError($"MultiAudioAgent on gameObject: \"{gameObject.name}\" doesn't contain \"{clipName}\".");
            return false;
        }
    }

}