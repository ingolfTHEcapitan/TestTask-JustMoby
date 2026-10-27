using _Project.Scripts.Logic.Common;
using Zenject;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarFactory
    {
        private readonly IInstantiator _container;
        
        public HealthBarFactory(IInstantiator container) => 
            _container = container;

        public void Create(HealthBarView healthBarView, Health healthModel, bool hideOnZeroHealth = true)
        {
            HealthBarPresenter presenter = 
                _container.Instantiate<HealthBarPresenter>(new object[] { healthBarView, healthModel, hideOnZeroHealth });
            presenter.Initialize();
        }
    }
}