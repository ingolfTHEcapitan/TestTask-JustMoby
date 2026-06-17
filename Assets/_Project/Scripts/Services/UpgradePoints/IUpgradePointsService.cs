using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.UpgradePoints
{
    public interface IUpgradePointsService
    {
        event Action OnChanged;
        int CurrentPoints { get; }
        UniTask AddPoint();
    }
}