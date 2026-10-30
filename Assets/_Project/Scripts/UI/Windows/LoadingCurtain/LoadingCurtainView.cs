using System;
using _Project.Scripts.Services;
using _Project.Scripts.Services.Audio;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Windows.LoadingCurtain
{
    public class LoadingCurtainView : MonoBehaviour, IWindow
    {
        public event Action OnWindowDestroy;
        
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private float _indicatorRotationSpeed = 100f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip _loadingMusic;
        
        private IMusicService _audioService;

        [Inject]
        private void Construct(IMusicService audioService) => 
            _audioService = audioService;

        private void Awake() => 
            DontDestroyOnLoad(gameObject);

        private void OnDestroy() => 
            OnWindowDestroy?.Invoke();

        public void Open()
        {
            gameObject.SetActive(true);
            _audioService.PlayMusic(_loadingMusic);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            _audioService.StopMusic();
        }

        private void Update() => 
            AnimateLoadingIndicator();

        private void AnimateLoadingIndicator() => 
            _loadingIndicator.transform.Rotate(Vector3.forward, Time.deltaTime * _indicatorRotationSpeed);
    }
}