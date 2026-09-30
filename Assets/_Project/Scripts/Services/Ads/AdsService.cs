using System;
using System.Threading;
using System.Threading.Tasks;
using _Project.Scripts.Configs;
using _Project.Scripts.Services.Progress;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;
using Application = UnityEngine.Device.Application;

namespace _Project.Scripts.Services.Ads
{
    public class AdsService: IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener, IAdsService
    {
        public event Action OnRewardedAdLoaded;
        public event Action OnInterstitialAdLoaded;

        private Action OnRewardedAdFinished;
        private Action OnInterstitialAdFinished;
        
        private readonly IProgressService _progressService;
        private readonly AdsConfig _config;
        
        private UniTaskCompletionSource<bool> _rewardedAdTcs;
        private UniTaskCompletionSource<bool> _interstitialAdTcs;

        public bool IsRewardedAdLoaded { get; private set; }
        public bool IsInterstitialAdLoaded { get; private set; }
        public bool IsAdsRemoved => _progressService.PlayerProgress.PurchaseData.IsAdsRemoved;
        
        public AdsService(IProgressService progressService, AdsConfig config)
        { 
            _progressService = progressService;
            _config = config;
        }

        public void Initialize() => 
            Advertisement.Initialize(GetGameId(), _config.TestMode, initializationListener: this);

        public void OnInitializationComplete() => 
            LoadAdsAsync().Forget();

        public void OnInitializationFailed(UnityAdsInitializationError error, string message) => 
            Debug.LogError($"[ADS SERVICE] Initialization Failed: {error.ToString()} - {message}");

        public void OnUnityAdsAdLoaded(string placementId)
        {
            if (placementId == _config.AndroidRewardedAdId)
            {
                IsRewardedAdLoaded = true;
                _rewardedAdTcs.TrySetResult(true);
                OnRewardedAdLoaded?.Invoke();
            }
            else if (placementId == _config.AndroidInterstitialAdId)
            {
                IsInterstitialAdLoaded = true;
                _interstitialAdTcs.TrySetResult(true);
                OnInterstitialAdLoaded?.Invoke();
            }
            
            Debug.LogError($"[ADS SERVICE] ads loaded: {placementId}");
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"[ADS SERVICE] Failed To Load: {placementId} {error.ToString()} - {message}");
            
            if (placementId == _config.AndroidRewardedAdId)
            {
                IsRewardedAdLoaded = false;
                _rewardedAdTcs.TrySetResult(false);
            }
            else if (placementId == _config.AndroidInterstitialAdId)
            {
                IsInterstitialAdLoaded = false;
                _interstitialAdTcs.TrySetResult(false);
            }
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) => 
            Debug.LogError($"[ADS SERVICE] Failed To Show: {placementId} {error.ToString()} - {message}");

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) => 
            OnAdFinishedAsync(placementId).Forget();

        private async UniTask OnAdFinishedAsync(string placementId)
        {
            await LoadAdsAsync();

            if (placementId == _config.AndroidRewardedAdId)
            {
                OnRewardedAdFinished?.Invoke();
                OnRewardedAdFinished = null;
            }
            else if (placementId == _config.AndroidInterstitialAdId)
            {
                OnInterstitialAdFinished?.Invoke();
                OnInterstitialAdFinished = null;
            }
        }

        public void OnUnityAdsShowStart(string placementId) { }

        public void OnUnityAdsShowClick(string placementId) { }

        public void ShowRewardedAd(Action onRewardedAdFinished)
        {
            Advertisement.Show(_config.AndroidRewardedAdId, this);
            OnRewardedAdFinished = onRewardedAdFinished;
        }

        public void ShowInterstitialAd(Action onInterstitialAdFinished)
        {
            if (IsAdsRemoved)
            {
                onInterstitialAdFinished?.Invoke();
                return;
            }
            
            Advertisement.Show(_config.AndroidInterstitialAdId, this);
            OnInterstitialAdFinished = onInterstitialAdFinished;
        }

        private string GetGameId()
        {
            string gameId = string.Empty;

            if (Application.platform == RuntimePlatform.Android) 
                gameId = _config.AndroidGameId;
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                gameId = _config.IOSGameId;
            else if (Application.platform == RuntimePlatform.WindowsEditor)
                gameId = _config.AndroidGameId;
            else
                Debug.LogWarning($"[ADS SERVICE] Platform '{Application.platform}' is not supported. ");
            
            return gameId;
        }

        private async UniTask LoadAdsAsync()
        {
            try
            {
                UniTask rewardedAd = LoadAdAsync(_config.AndroidRewardedAdId);
                UniTask interstitialAd = LoadAdAsync(_config.AndroidInterstitialAdId);
                await UniTask.WhenAll(rewardedAd, interstitialAd);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ADS SERVICE] loading add error: {e}");
            }
        }

        private UniTask LoadAdAsync(string placementId)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            
            if (placementId == _config.AndroidRewardedAdId)
                _rewardedAdTcs = tcs;
            else if (placementId == _config.AndroidInterstitialAdId) 
                _interstitialAdTcs = tcs;
            
            Advertisement.Load(placementId, this);
            return tcs.Task.AsUniTask();
        }
    }
}