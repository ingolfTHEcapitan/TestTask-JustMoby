using _Project.Scripts.Services.Sound;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.LoadingCurtain.UI
{
    public class LoadingCurtain : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private float _indicatorRotationSpeed = 100f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _loadingMusic;
        
        private IAudioService _audioService;

        [Inject]
        private void Construct(IAudioService audioService) => 
            _audioService = audioService;

        private void Awake() => 
            DontDestroyOnLoad(gameObject);

        public void Open()
        {
            gameObject.SetActive(true);
            _audioService.Play(_loadingMusic, _audioSource);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            _audioService.Stop(_audioSource);
        }

        private void Update()
        {
            _loadingIndicator.transform.Rotate(Vector3.forward, Time.deltaTime * _indicatorRotationSpeed);
        }
    }
}