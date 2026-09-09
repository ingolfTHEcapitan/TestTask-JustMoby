using System;
using UnityEngine;

namespace _Project.Scripts.Configs
{
    [Serializable]
    public class AdsConfig
    {
        [field: SerializeField] public string AndroidGameId { get; private set; }
        [field: SerializeField] public string IOSGameId { get; private set; }
        [field: SerializeField] public string AndroidRewardedAdId { get; private set; }
        [field: SerializeField] public string AndroidInterstitialAdId { get; private set; }
        [field: SerializeField] public bool TestMode { get; private set; }
    }
}