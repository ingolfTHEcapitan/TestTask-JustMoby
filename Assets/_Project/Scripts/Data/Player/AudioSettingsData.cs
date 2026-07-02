using System;

namespace _Project.Scripts.Data.Player
{
    [Serializable]
    public class AudioSettingsData
    {
        public float MasterVolume = 1.0f;
        public float EffectsVolume = 1.0f;
        public float MusicVolume = 0.25f;
        public float UIVolume = 0.75f;
    }
}