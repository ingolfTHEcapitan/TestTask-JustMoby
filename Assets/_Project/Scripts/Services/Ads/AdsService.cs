using System;
using System.Threading.Tasks;
using _Project.Scripts.Services.Progress;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;
using Application = UnityEngine.Device.Application;

namespace _Project.Scripts.Services.Ads
{
    public class AdsService: IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener, IAdsService
    {
        private const string AndroidGameId = "6036504";
        private const string IOSGameId = "6036505";
        private const string AndroidRewardedAdId = "Rewarded_Android";
        private const string AndroidInterstitialAdId = "Interstitial_Android";
        private const bool TestMode = true;
        
        public event Action OnRewardedAdLoaded;
        public event Action OnInterstitialAdLoaded;

        private Action _onRewardedAdFinished;
        private Action _onInterstitialAdFinished;
        
        private readonly IProgressService _progressService;

        public bool IsRewardedAdLoaded { get; private set; }
        public bool IsInterstitialAdLoaded { get; private set; }
        public bool IsAdsRemoved => _progressService.PlayerProgress.PurchaseData.IsAdsRemoved;
        
        public AdsService(IProgressService progressService)
        {
            _progressService = progressService;
            Advertisement.Initialize(GetGameId(), TestMode, this);
        }

        public async void  OnInitializationComplete()
        {
            Debug.Log("Unity Ads Initialization Complete!");

            UniTask rewardedAd = LoadAd(AndroidRewardedAdId);
            UniTask interstitialAd = LoadAd(AndroidInterstitialAdId);
            await UniTask.WhenAll(rewardedAd, interstitialAd);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message) => 
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"Unity Ads Ad Loaded: {placementId}");

            if (placementId == AndroidRewardedAdId)
            {
                IsRewardedAdLoaded = true;
                OnRewardedAdLoaded?.Invoke();
            }
            else if (placementId == AndroidInterstitialAdId)
            {
                IsInterstitialAdLoaded = true;
                OnInterstitialAdLoaded?.Invoke();
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) => 
            Debug.Log($"Unity Ads Failed To Load: {placementId} {error.ToString()} - {message}");

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) => 
            Debug.Log($"Unity Ads Failed To Load: {placementId} {error.ToString()} - {message}");

        public async void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log($"On Unity Ads Show Complete: {showCompletionState.ToString()}");

            UniTask rewardedAd = LoadAd(AndroidRewardedAdId);
            UniTask interstitialAd = LoadAd(AndroidInterstitialAdId);
            await UniTask.WhenAll(rewardedAd, interstitialAd);
            
            if (placementId == AndroidRewardedAdId)
            {
                _onRewardedAdFinished?.Invoke();
                _onRewardedAdFinished = null;
            }
            else if (placementId == AndroidInterstitialAdId)
            {
                _onInterstitialAdFinished?.Invoke();
                _onInterstitialAdFinished = null;
            }
        }

        public void OnUnityAdsShowStart(string placementId) { }

        public void OnUnityAdsShowClick(string placementId) { }

        public void ShowRewardedAd(Action onRewardedAdFinished)
        {
            Advertisement.Show(AndroidRewardedAdId, this);
            _onRewardedAdFinished = onRewardedAdFinished;
        }
        
        public void ShowInterstitialAd(Action onInterstitialAdFinished)
        {
            if (IsAdsRemoved)
            {
                onInterstitialAdFinished?.Invoke();
                return;
            }
            
            Advertisement.Show(AndroidInterstitialAdId, this);
            _onInterstitialAdFinished = onInterstitialAdFinished;
        }

        private string GetGameId()
        {
            string gameId = string.Empty;

            if (Application.platform == RuntimePlatform.Android) 
                gameId = AndroidGameId;
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                gameId = IOSGameId;
            else if (Application.platform == RuntimePlatform.WindowsEditor)
                gameId = AndroidGameId;
            else
                Debug.LogError("Unsupported platform for ads ");
            
            return gameId;
        }

        private UniTask LoadAd(string placementId)
        {
            Debug.Log($"Loading {placementId} Ad");
            Advertisement.Load(placementId, this);
            return Task.CompletedTask.AsUniTask();
        }

        
    }
}