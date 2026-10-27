using _Project.Scripts.Services;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameStarter
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly AudioService _audioService;
        private readonly LoadingCurtainPresenter _loadingCurtainPresenter;
        
        private readonly AudioSource _audioSource;
        private readonly AudioClip _dungeonMusic;
        
        public GameStarter(IAnalyticsService analyticsService, AudioService audioService, LoadingCurtainPresenter loadingCurtainPresenter, 
            AudioSource audioSource, AudioClip dungeonMusic)
        {
            _analyticsService = analyticsService;
            _audioService = audioService;
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _audioSource = audioSource;
            _dungeonMusic = dungeonMusic;
        }

        public void StartGame()
        {
            _analyticsService.LogGameStart();
            _audioService.Play(_dungeonMusic, _audioSource);
            _loadingCurtainPresenter.HideLoading();
        }
    }
}