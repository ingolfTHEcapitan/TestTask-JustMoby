using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.Ads
{
    public interface IAdsService
    {
        event Action OnRewardedAdLoaded;
        event Action OnInterstitialAdLoaded;
        bool IsRewardedAdLoaded { get; }
        bool IsInterstitialAdLoaded { get; }
        void Initialize();
        UniTask<bool> TryShowRewardedAd();
        UniTask<bool> TryShowInterstitialAd();
    }
}