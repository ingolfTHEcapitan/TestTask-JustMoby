using _Project.Scripts.Services.Sound;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.LoadingCurtain.UI
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private float _indicatorRotationSpeed = 100f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _loadingMusic;
        
        private IAudioService _audioService;

        [Inject]
        public void Construct(IAudioService audioService) => 
            _audioService = audioService;

        private void Awake() => 
            DontDestroyOnLoad(gameObject);

        public void Show()
        {
            gameObject.SetActive(true);
            _audioService.Play(_loadingMusic, _audioSource);
        }

        public void Hide()
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