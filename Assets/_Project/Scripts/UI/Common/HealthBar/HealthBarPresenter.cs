using System;
using System.Threading;
using _Project.Scripts.Logic.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarPresenter
    {
        private readonly Health _model;
        private readonly HealthBarView _view;
        
        private readonly bool _hideOnZeroHealth;

        private UniTask _currentLerpTask;
        private CancellationTokenSource _cts;
        
        public HealthBarPresenter(HealthBarView view, Health model, bool hideOnZeroHealth = true)
        {
            _view = view;
            _model = model;
            _hideOnZeroHealth = hideOnZeroHealth;
        }
        
        public void Initialize()
        {
            _view.OnHealthBarDestroy += CleanUp;
            _model.OnHealthChanged += UpdateHealthBar;
            
            if (_hideOnZeroHealth) 
                _model.OnZeroHealth += HideHealthBar;
            
            _view.UpdateHealthText(_model.CurrentHealth, _model.MaxHealth);
            _view.UpdateHealthSliderValue(_model.NormalizedHealth);
        }

        private void CleanUp()
        {
            _view.OnHealthBarDestroy -= CleanUp;
            _model.OnHealthChanged -= UpdateHealthBar;
            
            if (_hideOnZeroHealth) 
                _model.OnZeroHealth -= HideHealthBar;
            
            _cts?.Cancel();
            _cts?.Dispose();
        }
        
        private async void HideHealthBar()
        {
            try
            {
                await HideAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        private async UniTask HideAsync()
        {
            if (_currentLerpTask.Status == UniTaskStatus.Pending) 
                await _currentLerpTask;
            
            _view.Hide();
        }
        
        private void UpdateHealthBar()
        {
            _view.UpdateHealthText(_model.CurrentHealth, _model.MaxHealth);
            
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            _currentLerpTask = LerpHealthSliderAsync(_cts.Token);
        }
        
        private async UniTask LerpHealthSliderAsync(CancellationToken token)
        {
            float startValue = _view.HealthSliderValue;
            float targetValue = _model.NormalizedHealth;
            float elapsedTime = 0;
            
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
                
                _view.UpdateHealthSliderValue(Mathf.Lerp(startValue, targetValue, time));
                
                await UniTask.Yield(token).SuppressCancellationThrow();
            }
        }
    }
}