using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.UI.Common
{
    public class WindowPopupAnimation: MonoBehaviour
    {
        [Header("Scale Animation")]
        [SerializeField] private Transform _transform;
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private Vector3 _animationStartScale = Vector3.one * 0.5f;
        [SerializeField] private Ease _openEase = Ease.OutBack;
        [SerializeField] private Ease _closeEase = Ease.InBack;
        
        [Header("Fade Animation")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _openFadeDuration = 0.1f;

        private Sequence _animation;
        
        private void OnDestroy() => 
            KillAnimationIfActive();

        public void AnimateOpen()
        {
            KillAnimationIfActive();
            
            _animation = DOTween.Sequence();
            _animation
                .Join(_canvasGroup.DOFade(1f, _openFadeDuration).From(0))
                .Join(_transform.DOScale(Vector3.one, _animationDuration).From(_animationStartScale).SetEase(_openEase));
        }

        public async UniTask AnimateCloseAsync()
        {
            KillAnimationIfActive();
            
            _animation = DOTween.Sequence();
            _animation
                .Join(_transform.DOScale(_animationStartScale, _animationDuration).SetEase(_closeEase));
            await _animation.ToUniTask();
        }

        private void KillAnimationIfActive()
        {
            if (_animation != null && _animation.active)
            {
                _animation.Kill(complete: true);
                _animation = null;
            }
        }
    }
}