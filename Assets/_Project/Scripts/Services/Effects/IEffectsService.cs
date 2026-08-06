using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.Effects
{
    public interface IEffectsService
    {
        UniTask WarmUpAsync();
        UniTask PlayHitFxAsync(Vector3 position, Transform parent);
        UniTask PlayEnemyDeathFxAsync(Vector3 position, Transform parent);
    }
}