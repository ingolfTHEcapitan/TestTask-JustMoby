using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.Effects
{
    public interface IEffectsService
    {
        UniTask WarmUp();
        UniTask PlayHitFx(Vector3 position, Transform parent);
        UniTask PlayEnemyDeathFx(Vector3 position, Transform parent);
    }
}