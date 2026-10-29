using _Project.Scripts.Logic.Common;
using Zenject;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarFactory
    {
        private readonly IInstantiator _container;
        
        public HealthBarFactory(IInstantiator container) => 
            _container = container;

        public HealthBarPresenter Create(HealthBarView healthBarView, Health healthModel)
        {
            HealthBarPresenter presenter = _container.Instantiate<HealthBarPresenter>(new object[] { healthBarView, healthModel });
            presenter.Initialize();
            return presenter;
        }
    }
}