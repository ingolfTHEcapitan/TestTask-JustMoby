using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Services.Sound
{
    public class AudioService : IAudioService
    {
        private const float MinPitchValue = 0.9f;
        private const float MaxPitchValue = 1.2f;

        public void PlayOneShot(AudioClip clip, AudioSource audioSource)
        {
            audioSource.PlayOneShot(clip);
            audioSource.pitch = Random.Range(MinPitchValue, MaxPitchValue);
        }

        public void PlayOneShotRandom(List<AudioClip> clips, AudioSource audioSource)
        {
            AudioClip randomClip = clips[Random.Range(0, clips.Count)];
            PlayOneShot(randomClip, audioSource);
        }
        
        public void PlayDelayed(AudioClip clip, AudioSource audioSource, float delay)
        {
            audioSource.clip = clip;
            audioSource.PlayDelayed(delay);
            audioSource.pitch = Random.Range(MinPitchValue, MaxPitchValue);
        }

        public void Play(AudioClip clip, AudioSource audioSource)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void Stop(AudioSource audioSource) => 
            audioSource.Stop();
    }
}