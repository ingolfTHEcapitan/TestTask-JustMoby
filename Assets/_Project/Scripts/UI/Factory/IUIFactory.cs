using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows.GameOver;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using _Project.Scripts.UI.Windows.MainMenu;
using _Project.Scripts.UI.Windows.PlayerStats;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
using _Project.Scripts.UI.Windows.Settings;
using _Project.Scripts.UI.Windows.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Factory
{
    public interface IUIFactory
    {
        UniTask<HeadUpDisplayView> CreateHudViewAsync(Transform uiParent);
        UniTask<GameOverWindowView> CreateGameOverWindowViewAsync(Transform uiParent);
        UniTask<LoadingCurtainView> CreateLoadingCurtainViewAsync();
        UniTask<MainMenuWindowView> CreateMainMenuWindowViewAsync(Transform uiParent);
        UniTask<PlayerStatsWindowView> CreatePlayerStatsViewAsync(Transform uiParent);
        UniTask<PlayerStatItemView> CreatePlayerStatItemViewAsync(Transform uiParent);
        UniTask<SaveConflictResolveWindow> CreateSaveConflictResolveWindowAsync(Transform uiParent);
        UniTask<SettingsWindowView> CreateSettingsViewAsync(Transform uiParent);
        UniTask<ShopWindowView> CreateShopWindowViewAsync(Transform uiParent);
        UniTask<Sprite> LoadSpriteAsync(string assetAddress);
        HeadUpDisplayView GetHudView();
        LoadingCurtainView GetLoadingWindowView();
        SettingsWindowView GetSettingsWindowView();
        GameOverWindowView GetGameOverWindowView();
        ShopWindowView GetShopWindowView();
        PlayerStatsWindowView GetPlayerStatsWindowView();
        MainMenuWindowView GetMainMenuWindowView();
    }
}