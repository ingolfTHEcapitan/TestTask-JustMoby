using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Services.Audio
{
    public interface IMusicService
    {
        void PlayMusic(AudioClip clip, bool loop = true);
        void StopMusic();
    }
}