using System;
using _Project.Scripts.Data.Player;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.SaveLoad.SaveCoordinator
{
    public interface ISaveLoadCoordinator : ISaveLoadService
    {
        event Func<PlayerProgress, PlayerProgress, UniTask<SaveType>> OnSaveConflictHappened;
    }
}