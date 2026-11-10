using _Project.Scripts.Configs;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.PlayerStats;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Windows.PlayerStats.Stat
{
    public class PlayerStatViewFactory
    {
        private readonly IInstantiator _container;
        private readonly IAssetProvider _assetProvider;
        
        public PlayerStatViewFactory(IInstantiator container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }

        public async UniTask<PlayerStatView> CreateAsync(Transform parent, PlayerStatData data, PlayerStatConfig config, AudioSource audioSource)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.PlayerStatItem);
            PlayerStatView view = _container.InstantiatePrefabForComponent<PlayerStatView>(prefab, parent);
            Sprite iconFrame = await _assetProvider.LoadAsync<Sprite>(config.IconFrameAddress);
            Sprite icon = await _assetProvider.LoadAsync<Sprite>(config.IconAddress);
            
            view.Initialize(data, iconFrame, icon, audioSource);
            return view;
        }
    }
}