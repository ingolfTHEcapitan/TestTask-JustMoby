using _Project.Scripts.Services.GamePause;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Common.PauseHandlers
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorPauser : MonoBehaviour, IPauseHandler
    {
        [SerializeField] private Animator _animator;
        
        private IGamePauseService _pauseService;

        [Inject]
        public void Construct(IGamePauseService pauseService) => 
            _pauseService = pauseService;
        
        private void Awake()
        {
            _pauseService.Register(this);
            SetAnimatorSpeed(_pauseService.IsPaused);
        }

        private void OnDestroy() => 
            _pauseService.UnRegister(this);
        
        public void SetPaused(bool isPaused) => 
            SetAnimatorSpeed(isPaused);

        private void SetAnimatorSpeed(bool isPaused) => 
            _animator.speed = isPaused ? 0f : 1f;
    }
}