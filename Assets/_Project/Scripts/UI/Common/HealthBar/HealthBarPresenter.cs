using System.Threading;
using _Project.Scripts.Logic.Common.Health;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarPresenter
    {
        private readonly IHealth _model;
        private readonly HealthBarView _view;
        
        private UniTask _currentLerpTask;
        private CancellationTokenSource _cts;

        public HealthBarPresenter(HealthBarView view, IHealth model)
        {
            _view = view;
            _model = model;
        }
        
        public void Initialize()
        {
            _view.OnHealthBarDestroy += CleanUp;
            _model.OnHealthChanged += UpdateHealthBar;
            _model.OnTakeDamage += _view.PlayHitSound;
            _model.OnTakeHeal += _view.PlayHealSound;
            
            _view.UpdateHealthText(_model.CurrentHealth, _model.MaxHealth);
            _view.UpdateHealthSliderValue(_model.NormalizedHealth);
        }

        private void CleanUp()
        {
            _view.OnHealthBarDestroy -= CleanUp;
            _model.OnHealthChanged -= UpdateHealthBar;
            _model.OnTakeDamage -= _view.PlayHitSound;
            _model.OnTakeHeal -= _view.PlayHealSound;
            
            _cts?.Cancel();
            _cts?.Dispose();
        }
        
        //P+
        public async UniTask HideAsync()
        {
            if (_currentLerpTask.Status == UniTaskStatus.Pending) 
                await _currentLerpTask;
            
            //V+
            _view.Hide();
        }
        
        //V+ P
        private void UpdateHealthBar()
        {
            //V+
            _view.UpdateHealthText(_model.CurrentHealth, _model.MaxHealth);

            //P
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            //P
            _currentLerpTask = LerpHealthSliderAsync(_cts.Token);
        }
        
        //P
        private async UniTask LerpHealthSliderAsync(CancellationToken token)
        {
            //V
            float startValue = _view.HealthSliderValue;
            //M
            float targetValue = _model.NormalizedHealth;
            //P
            float elapsedTime = 0;
            //P
            float distance = Mathf.Abs(targetValue - startValue);
            float duration = distance / _view.SliderValuePerSecond;
            
            
            if (Mathf.Approximately(startValue, targetValue))
                return;
            
            while (!Mathf.Approximately(_view.HealthSliderValue , targetValue))
            {
                if(token.IsCancellationRequested)
                    break;
                
                elapsedTime += Time.deltaTime;
                float time = Mathf.Clamp01(elapsedTime / duration);
                //V
                _view.UpdateHealthSliderValue(Mathf.Lerp(startValue, targetValue, time));
                
                await UniTask.Yield(token);
            }
        }
    }
}