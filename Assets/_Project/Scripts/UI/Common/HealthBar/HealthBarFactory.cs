using _Project.Scripts.Logic.Common.Health;
using Zenject;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarFactory
    {
        private readonly IInstantiator _container;
        
        public HealthBarFactory(IInstantiator container) => 
            _container = container;

        public HealthBarPresenter Create(HealthBarView healthBarView, IHealth healthModel)
        {
            HealthBarPresenter presenter = _container.Instantiate<HealthBarPresenter>(new object[] { healthBarView, healthModel });
            presenter.Initialize();
            return presenter;
        }
    }
}