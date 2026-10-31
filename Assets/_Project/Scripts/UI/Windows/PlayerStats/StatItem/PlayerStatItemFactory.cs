using _Project.Scripts.Configs;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.PlayerStats;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Windows.PlayerStats.StatItem
{
    public class PlayerStatItemFactory
    {
        private readonly IInstantiator _container;
        private readonly IAssetProvider _assetProvider;
        
        public PlayerStatItemFactory(IInstantiator container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }

        public async UniTask<PlayerStatItemView> CreateAsync(Transform parent, PlayerStatData stat, PlayerStatConfig statConfig, AudioSource audioSource)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.PlayerStatItem);
            PlayerStatItemView statItem = _container.InstantiatePrefabForComponent<PlayerStatItemView>(prefab, parent);
            Sprite iconFrame = await _assetProvider.LoadAsync<Sprite>(statConfig.IconFrameAddress);
            Sprite icon = await _assetProvider.LoadAsync<Sprite>(statConfig.IconAddress);
            
            statItem.Initialize(stat, iconFrame, icon, audioSource);
            return statItem;
        }
    }
}