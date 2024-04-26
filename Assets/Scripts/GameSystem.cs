using Cysharp.Threading.Tasks;
using RoachFramework;
using UnityEngine;

public class GameSystem : BaseSingleton<GameSystem> {

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeSceneLoad() {
        Instance.AsyncLoad();
    }
    
    public BroadcastService Broadcast { get; private set; }
    public GameDataService GameData { get; private set; }
    
    protected override void OnInitialized() {
        Broadcast = ServiceProvider.Instance.ProvideService<BroadcastService>(BroadcastService.SERVICE_NAME);
        GameData = ServiceProvider.Instance.ProvideService<GameDataService>(GameDataService.ServiceName);
        // --- 预加载放在这里
        AnimaManager.Instance.LoadAnimaData();
        SpriteManager.Instance.LoadSpriteData();
        PrefabManager.Instance.LoadPrefabData();
        TextManager.Instance.LoadTextData();
        MaterialManager.Instance.LoadMaterialData();
        TilesManager.Instance.LoadTilesData();
        LogUtils.LogNotice("System Initialized Before Scene Load!");
    }

    private async void AsyncLoad() {
        await UniTask.Yield();
        // --- 异步加载放在这里
        LogUtils.LogNotice("System Asynchronous Load!");
    }
}
