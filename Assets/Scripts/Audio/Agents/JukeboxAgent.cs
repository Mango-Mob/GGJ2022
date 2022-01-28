using System.Collections.Generic;
using AudioSystem.Managers;
using UnityEngine;
using Unity.Collections;

namespace AudioSystem.Agents
{
    public class JukeboxAgent : AudioAgent
    {
        public List<AudioClip> audioClips;
        public bool isShuffled = false;
        public bool isPlayingOnAwake = false;
        public bool isLoopingQueue = false;

        [Header("Fade Controls:")]
        public bool hasFadeTransitions = false;
        [ShowIf("hasFadeTransitions", false)]
        public float fadeTime = 5.0f;

        [ReadOnly]
        public AudioClip currentlyPlaying;

        private List<AudioClip> currentList;
        private bool isCurrentlyShuffled = false;
        private int currentIndex;
        private bool isPlaying = false;


        protected AudioManager.VolumeChannel type = AudioManager.VolumeChannel.MUSIC;
        protected AudioPlayer player;
        protected AudioPlayer backPlayer;

        protected override void Awake()
        {
            base.Awake();
            player = new AudioPlayer(gameObject, null);
            player.SetVolume(AudioManager.Instance .GetVolume(type, this) * localVolume);
            backPlayer = new AudioPlayer(gameObject, null);
            currentList = new List<AudioClip>();
            foreach (var clip in audioClips)
            {
                currentList.Add(clip);
            }

            currentlyPlaying = audioClips[0];

            if (isShuffled)
            {
                Shuffle();
            }
            if (isPlayingOnAwake)
            {
                Play();
            }
        }

        protected override void Update()
        {
            CheckAudioPlayer();

            if (isShuffled && !isCurrentlyShuffled)
                Shuffle();
            else if (!isShuffled && isCurrentlyShuffled)
                ResetOrder();

            currentlyPlaying = player.currentClip;

            if (isMuted)
            {
                player.SetVolume(0.0f);
                backPlayer.SetVolume(0.0f);
            }
            else
            {
                player.SetVolume(AudioManager.Instance .GetVolume(type, this) * localVolume);
                backPlayer.SetVolume(AudioManager.Instance .GetVolume(type, this) * localVolume);
            }

        }
        public bool IsPlaying()
        {
            return player.IsPlaying() || backPlayer.IsPlaying();
        }

        private void CheckAudioPlayer()
        {
            if (hasFadeTransitions && player.TimeLeft() < 5.0f && player.IsPlaying())
            {
                float time = player.TimeLeft();
                //Start Fade out:
                player.FadeOut(time);

                //Switch to backPlayer
                var temp = backPlayer;
                backPlayer = player;
                player = temp;

                //Start Fade in:
                LoadNextAudio();
                player.FadeIn(time);
            }
            if (!player.IsPlaying() && isPlaying)
                LoadNextAudio();
        }

        private void LoadNextAudio()
        {
            uint nextIndex = (uint)currentIndex + 1;
            if (currentIndex + 1 >= currentList.Count && isLoopingQueue)
            {
                if (isLoopingQueue)
                    nextIndex = 0;
                else
                    return;
            }
            Play(nextIndex);
        }

        public void Shuffle()
        {
            //Fisher-Yates shuffle Algorithm:
            var rng = new System.Random();
            int n = currentList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(0, n + 1);
                var temp = currentList[k];
                currentList[k] = currentList[n];
                currentList[n] = temp;
            }
            currentIndex = GetIndexOf(currentlyPlaying);
            isCurrentlyShuffled = true;
            player.Stop();
        }

        public void ResetOrder()
        {
            for (int i = 0; i < currentList.Count; i++)
            {
                currentList[i] = audioClips[i];
            }
            currentIndex = GetIndexOf(currentlyPlaying);
            isCurrentlyShuffled = false;
        }

        private int GetIndexOf(AudioClip clip)
        {
            for (int i = 0; i < currentList.Count; i++)
            {
                if (currentList[i].name == clip.name)
                {
                    return i;
                }
            }
            Debug.LogError("Audio clip provided does not exist (in GetIndexOf function).");
            return -1;
        }

        public bool Play(uint index = 0)
        {
            //Wrap arround:
            index += (uint)currentList.Count;
            index %= (uint)currentList.Count;

            player.SetLooping(false);
            player.SetClip(currentList[(int)index]);

            if (hasFadeTransitions)
            {
                player.FadeIn(fadeTime);
            }


            player.Play();
            isPlaying = true;
            currentIndex = (int)index;
            return true;
        }

        public bool Play(string clipName, bool isSolo = false, bool isLooping = false)
        {
            for (int i = 0; i < currentList.Count; i++)
            {
                if (currentList[i].name == clipName)
                {
                    player.SetClip(currentList[i]);
                    player.SetLooping(isSolo && isLooping);
                    player.SetPitch(1.0f);

                    if (hasFadeTransitions)
                        player.FadeIn(fadeTime);

                    player.Play();
                    currentIndex = i;
                    isLoopingQueue = isLooping;
                    return true;
                }
            }
            isPlaying = true;
            Debug.LogError($"MultiAudioAgent on gameObject: \"{gameObject.name}\" doesn't contain \"{clipName}\".");
            return false;
        }

        public void Stop()
        {
            if (hasFadeTransitions)
                player.FadeOut(fadeTime);
            else
            {
                player.Stop();
            }
            isPlaying = false;
            player.SetPitch(1.0f);
        }
    }
}