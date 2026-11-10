using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Logic.Common
{
    public class DissolveShader : MonoBehaviour
    {
        private static readonly int DissolveAmountId = Shader.PropertyToID("_DissolveAmount");
        private const float MaxThreshold = 1f;
        
        [SerializeField] private float _speed = 0.25f;

        private Renderer[] _dissolveRenderers;
        private MaterialPropertyBlock _propertyBlock;
        
        private CancellationTokenSource _dissolveTokenSource;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            _dissolveRenderers = GetComponentsInChildren<Renderer>();
        }
        
        private void OnDestroy()
        {
            StopTask();
            ClearPropertyBlock();
        }

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
                ApplyThreshold(threshold);
                threshold += Time.deltaTime * _speed;
                await UniTask.Yield(token);
            }
            
            ApplyThreshold(MaxThreshold);
        }

        private void ApplyThreshold(float threshold)
        {
            foreach (Renderer renderer in _dissolveRenderers)
            {
                renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(DissolveAmountId, threshold);
                renderer.SetPropertyBlock(_propertyBlock);
            }
        }

        private void StopTask()
        {
            _dissolveTokenSource?.Cancel();
            _dissolveTokenSource?.Dispose();
            _dissolveTokenSource = null;
        }

        private void ClearPropertyBlock()
        {
            _propertyBlock?.Clear();
            foreach (Renderer renderer in _dissolveRenderers) 
                renderer.SetPropertyBlock(null);
        }
    }
}