using System;
using _Project.Scripts.Logic.Common;
using UnityEngine;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarPresenter
    {
        private readonly Health _model;
        private readonly HealthBarView _view;
        
        private readonly bool _hideOnZeroHealth;
        
        public HealthBarPresenter(HealthBarView view, Health model, bool hideOnZeroHealth = true)
        {
            _view = view;
            _model = model;
            _hideOnZeroHealth = hideOnZeroHealth;
        }
        
        public void Initialize()
        {
            _view.OnHealthBarDestroy += CleanUp;
            _model.OnHealthChanged += UpdateHealthView;
            
            if (_hideOnZeroHealth) 
                _model.OnZeroHealth += HideHealthView;
            
            _view.Initialize(_model.CurrentHealth, _model.MaxHealth);
        }

        private void CleanUp()
        {
            _view.OnHealthBarDestroy -= CleanUp;
            _model.OnHealthChanged -= UpdateHealthView;
            
            if (_hideOnZeroHealth) 
                _model.OnZeroHealth -= HideHealthView;
        }

        private void UpdateHealthView() => 
            _view.UpdateHealthBar(_model.CurrentHealth, _model.MaxHealth);

        private async void HideHealthView()
        {
            try
            {
                await _view.HideAsync();
            }
            catch (OperationCanceledException)
            {
               
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}