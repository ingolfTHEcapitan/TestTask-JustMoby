using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Logic.Common
{
    public class DissolveShader: MonoBehaviour
    {
        private readonly List<Material> dissolveMaterials = new List<Material>();

        private readonly float _maxThreshold = 1f;
        private readonly string _dissolveAmountKey = "_DissolveAmount";
        private readonly float _speed = 0.25f;
        private Coroutine _dissolveCoroutine;

        private void Awake()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers) 
                dissolveMaterials.Add(renderer.material);
        }
        
        public void Dissolve()
        {
            if (_dissolveCoroutine != null)
                StopCoroutine(_dissolveCoroutine);
            
            _dissolveCoroutine = StartCoroutine(DissolveRoutine());
        }

        private IEnumerator DissolveRoutine()
        {
            float threshold = 0;

            while (threshold < _maxThreshold)
            {
                foreach (Material material in dissolveMaterials) 
                    material.SetFloat(_dissolveAmountKey, threshold);
                
                threshold += Time.deltaTime * _speed;
                yield return null;
            }
        }
    }
}