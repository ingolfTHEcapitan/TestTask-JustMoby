using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Services.Audio
{
    public interface ISoundService
    {
        void PlayOneShot(AudioClip clip, AudioSource audioSource);
        void PlayOneShotRandom(List<AudioClip> clips, AudioSource audioSource);
        void PlayDelayed(AudioClip clip, AudioSource audioSource, float delay);
        void Stop(AudioSource audioSource);
    }
}