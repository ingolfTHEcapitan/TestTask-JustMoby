using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameStarter
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IAudioService _audioService;
        private readonly LoadingCurtainPresenter _loadingCurtainPresenter;
        
        private readonly AudioSource _audioSource;
        private readonly AudioClip _dungeonMusic;
        private readonly CursorController _cursorController;
        
        public GameStarter(IAnalyticsService analyticsService, IAudioService audioService, LoadingCurtainPresenter loadingCurtainPresenter, 
            AudioSource audioSource, AudioClip dungeonMusic, CursorController cursorController)
        {
            _analyticsService = analyticsService;
            _audioService = audioService;
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _audioSource = audioSource;
            _dungeonMusic = dungeonMusic;
            _cursorController = cursorController;
        }

        public void StartGame()
        {
            _analyticsService.LogGameStart();
            _cursorController.SetCursorVisible(visible: false);
            _audioService.Play(_dungeonMusic, _audioSource);
            _loadingCurtainPresenter.HideLoading();
        }
    }
}