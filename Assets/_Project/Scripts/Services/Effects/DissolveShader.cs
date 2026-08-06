using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Services.Effects
{
    public class DissolveShader: MonoBehaviour
    {
        private const float MaxThreshold = 1f;
        private const string DissolveAmountKey = "_DissolveAmount";
        
        [SerializeField] private float _speed = 0.25f;
        
        private readonly List<Material> _dissolveMaterials = new List<Material>();
        private Coroutine _dissolveCoroutine;

        private void Awake()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers) 
                _dissolveMaterials.Add(renderer.material);
        }
        
        public void PlayDissolveFx()
        {
            if (_dissolveCoroutine != null)
                StopCoroutine(_dissolveCoroutine);
            
            _dissolveCoroutine = StartCoroutine(DissolveRoutine());
        }

        private IEnumerator DissolveRoutine()
        {
            float threshold = 0;

            while (threshold < MaxThreshold)
            {
                foreach (Material material in _dissolveMaterials) 
                    material.SetFloat(DissolveAmountKey, threshold);
                
                threshold += Time.deltaTime * _speed;
                yield return null;
            }
        }
    }
}