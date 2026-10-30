using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Services.Audio
{
    public class AudioService : IMusicService, ISoundService
    {
        private const float MinPitchValue = 0.9f;
        private const float MaxPitchValue = 1.2f;

        private readonly AudioSource _musicAudioSource;

        public AudioService(AudioSource musicAudioSource) => 
            _musicAudioSource = musicAudioSource;

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

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            _musicAudioSource.clip = clip;
            _musicAudioSource.loop = loop;
            _musicAudioSource.Play();
        }

        public void Stop(AudioSource audioSource) => 
            audioSource.Stop();
            
    }
}