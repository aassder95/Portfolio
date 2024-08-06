using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    #region Fields
    static Managers m_instance;
    CardManager m_cardManager = new CardManager();
    ChatManager m_chatManager = new ChatManager();
    DataManager m_dataManager = new DataManager();
    EnemyManager m_enemyManager = new EnemyManager();
    FishManager m_fishManager = new FishManager();
    GameManager m_gameManager = new GameManager();
    GridManager m_gridManager = new GridManager();
    InputManager m_inputManager = new InputManager();
    NetworkManager m_networkManager = new NetworkManager();
    PoolManager m_poolManager = new PoolManager();
    RankManager m_rankManager = new RankManager();
    ResourceManager m_resourceManager = new ResourceManager();
    SceneManagerEx m_sceneManager = new SceneManagerEx();
    ShopManager m_shopManager = new ShopManager();
    SoundManager m_soundManager = new SoundManager();
    StageManager m_stageManager = new StageManager();
    TemplateManager m_templateManager = new TemplateManager();
    UIManager m_uiManager = new UIManager();
    #endregion
    #region Properties
    static Managers Instance => m_instance;
    public static CardManager Card => Instance.m_cardManager;
    public static ChatManager Chat => Instance.m_chatManager;
    public static DataManager Data => Instance.m_dataManager;
    public static EnemyManager Enemy => Instance.m_enemyManager;
    public static FishManager Fish => Instance.m_fishManager;
    public static GameManager Game => Instance.m_gameManager;
    public static GridManager Grid => Instance.m_gridManager;
    public static InputManager Input => Instance.m_inputManager;
    public static NetworkManager Network => Instance.m_networkManager;
    public static PoolManager Pool => Instance.m_poolManager;
    public static RankManager Rank => Instance.m_rankManager;
    public static ResourceManager Resource => Instance.m_resourceManager;
    public static SceneManagerEx Scene => Instance.m_sceneManager;
    public static ShopManager Shop => Instance.m_shopManager;
    public static SoundManager Sound => Instance.m_soundManager;
    public static StageManager Stage => Instance.m_stageManager;
    public static TemplateManager Template => Instance.m_templateManager;
    public static UIManager UI => Instance.m_uiManager;
    #endregion
    #region Unity
    void Update()
    {
        if (Scene.CurScene.Type == Define.Scene.Game)
        {
            Input.Update();
            Stage.Update();
        }
    }
    #endregion
    #region Init
    public static void Init()
    {
        GameObject go = new GameObject("@Managers");
        m_instance = go.GetOrAddComponent<Managers>();
        DontDestroyOnLoad(go);

        m_instance.InitManagers();
        m_instance.SetResolution();
        Application.targetFrameRate = 60;
    }
    #endregion
    #region InitSub
    void InitManagers()
    {
        Data.Init();
        Sound.Init();
    }
    #endregion
    #region Set
    public void SetResolution()
    {
        Vector2 targetRes = new Vector2(1080.0f, 1920.0f);
        Vector2 deviceRes = new Vector2(Screen.width, Screen.height);
        Screen.SetResolution((int)targetRes.x, (int)((deviceRes.y / deviceRes.x) * targetRes.x), true);

        float targetAspect = targetRes.x / targetRes.y;
        float deviceAspect = deviceRes.x / deviceRes.y;
        if (targetAspect < deviceAspect)
        {
            float newWidth = targetAspect / deviceAspect;
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = deviceAspect / targetAspect;
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
    #endregion
}
