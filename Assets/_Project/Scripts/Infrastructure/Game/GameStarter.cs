using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.UI.Common;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameStarter
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IAudioService _audioService;
        private readonly ILoadingCurtainService _loadingCurtain;
        
        private readonly AudioSource _audioSource;
        private readonly AudioClip _dungeonMusic;
        private readonly CursorController _cursorController;
        
        public GameStarter(IAnalyticsService analyticsService, IAudioService audioService,ILoadingCurtainService loadingCurtain, 
            AudioSource audioSource, AudioClip dungeonMusic, CursorController cursorController)
        {
            _analyticsService = analyticsService;
            _audioService = audioService;
            _loadingCurtain = loadingCurtain;
            _audioSource = audioSource;
            _dungeonMusic = dungeonMusic;
            _cursorController = cursorController;
        }

        public void StartGame()
        {
            _analyticsService.LogGameStart();
            _cursorController.SetCursorVisible(visible: false);
            _audioService.Play(_dungeonMusic, _audioSource);
            _loadingCurtain.HideLoading();
        }
    }
}