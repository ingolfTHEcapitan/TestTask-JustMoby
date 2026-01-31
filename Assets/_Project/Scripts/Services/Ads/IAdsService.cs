using System;

namespace _Project.Scripts.Services.Ads
{
    public interface IAdsService
    {
        event Action OnRewardedAdLoaded;
        event Action OnInterstitialAdLoaded;
        bool IsRewardedAdLoaded { get; }
        bool IsInterstitialAdLoaded { get; }
        void ShowRewardedAd(Action onRewardedAdFinished);
        void ShowInterstitialAd(Action onInterstitialAdFinished);
    }
}