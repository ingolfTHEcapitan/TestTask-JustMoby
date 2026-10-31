using _Project.Scripts.Services.GamePause;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Logic.Common.PauseHandlers
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentPauser : MonoBehaviour, IPauseHandler
    {
        [SerializeField] private NavMeshAgent _agent;
        
        private IGamePauseService _pauseService;

        [Inject]
        public void Construct(IGamePauseService pauseService) => 
            _pauseService = pauseService;
        
        private void Awake() => 
            _pauseService.Register(this);

        private void OnDestroy() => 
            _pauseService.UnRegister(this);
        
        public void SetPaused(bool isPaused)
        {
            if (!_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
                return;
            
            _agent.isStopped = isPaused;

            if (isPaused) 
                _agent.velocity = Vector3.zero;
        }
    }
}