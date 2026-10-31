using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Logic.Common
{
    public class DissolveShader : MonoBehaviour
    {
        private const float MaxThreshold = 1f;
        private const string DissolveAmountKey = "_DissolveAmount";
        
        [SerializeField] private float _speed = 0.25f;
        
        private readonly List<Material> _dissolveMaterials = new List<Material>();
        private UniTask _dissolveTask;
        private CancellationTokenSource _dissolveTokenSource;

        private void Awake()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers) 
                _dissolveMaterials.Add(renderer.material);
        }
        
        private void OnDestroy() => 
            StopTask();

        public void PlayDissolveFx()
        {
            StopTask();
            _dissolveTokenSource = new CancellationTokenSource();
            DissolveAsync(_dissolveTokenSource.Token).Forget();
        }

        private async UniTask DissolveAsync(CancellationToken token)
        {
            float threshold = 0;

            while (threshold < MaxThreshold)
            {
                foreach (Material material in _dissolveMaterials) 
                    material.SetFloat(DissolveAmountKey, threshold);
                
                threshold += Time.deltaTime * _speed;
                await UniTask.Yield(token);
            }
        }

        private void StopTask()
        {
            _dissolveTokenSource?.Cancel();
            _dissolveTokenSource?.Dispose();
            _dissolveTokenSource = null;
        }
    }
}