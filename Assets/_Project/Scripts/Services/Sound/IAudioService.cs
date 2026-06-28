using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Services.Sound
{
    public interface IAudioService
    {
        void PlayOneShot(AudioClip clip, AudioSource audioSource);
        void PlayOneShotRandom(List<AudioClip> clips, AudioSource audioSource);
        void Play(AudioClip clip, AudioSource audioSource);
        void Stop(AudioSource audioSource);
        void PlayDelayed(AudioClip clip, AudioSource audioSource, float delay);
    }
}