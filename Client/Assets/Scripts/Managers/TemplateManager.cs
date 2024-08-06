using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public interface ITemplate
{
    #region Properties
    int Id { get; set; }
    #endregion
    #region Control
    void Load(DataRow row);
    #endregion
}

[AttributeUsage(AttributeTargets.Field)]
public class TemplateCoreAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field)]
public class TemplateAttribute : Attribute { }

public class TemplateManager
{
    #region Fields
    int m_loadTemplateCnt = 0;
    int m_totalLoadTemplateCnt;
    string m_path = $"{Application.dataPath}/Resources/Templates";
    [TemplateCore] Dictionary<int, TextTemplate> m_textTemplates;
    [Template] Dictionary<int, EnemyTemplate> m_enemyTemplates;
    [Template] Dictionary<int, FishLevelTemplate> m_fishLevelTemplates;
    [Template] Dictionary<int, FishTemplate> m_fishTemplates;
    [Template] Dictionary<int, ShopTemplate> m_shopTemplates;
    [Template] Dictionary<int, StageTemplate> m_stageTemplates;
    Subject<float> m_onLoadTemplate = new Subject<float>();
    #endregion
    #region Properties
    public Dictionary<int, FishTemplate> FishTemplates => m_fishTemplates;
    public IObservable<float> OnLoadTemplate => m_onLoadTemplate;
    #endregion
    #region Control
    public async UniTask LoadCoreTemplates()
    {
        m_textTemplates = await LoadJsonTemplate<TextTemplate>();
    }

    public async UniTask LoadTemplates()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        m_totalLoadTemplateCnt = fields.Count(field => field.GetCustomAttribute<TemplateAttribute>() != null);

        m_enemyTemplates = await LoadJsonTemplate<EnemyTemplate>();
        m_fishLevelTemplates = await LoadJsonTemplate<FishLevelTemplate>();
        m_fishTemplates = await LoadJsonTemplate<FishTemplate>();
        m_shopTemplates = await LoadJsonTemplate<ShopTemplate>();
        m_stageTemplates = await LoadJsonTemplate<StageTemplate>();
    }
    #endregion
    #region Utility
    async UniTask<Dictionary<int, T>> LoadJsonTemplate<T>() where T : ITemplate
    {
        string json = await ReadTemplateFile<T>();

        List<T> templates = JsonConvert.DeserializeObject<List<T>>(json);
        m_onLoadTemplate.OnNext((float)++m_loadTemplateCnt / m_totalLoadTemplateCnt);

        Debug.Log($"[TemplateManager:LoadJsonTemplate] Loaded {typeof(T).Name}");
        return templates.ToDictionary(template => template.Id);
    }

    async UniTask<string> ReadTemplateFile<T>()
    {
        string fileExtension = Constant.Template.AES || Constant.Template.GZIP ? "pft" : "json";
        string json = await File.ReadAllTextAsync($"{m_path}/{typeof(T).Name}.{fileExtension}");

        if (Constant.Template.GZIP)
            json = await GZipCompression.Decompress(json);

        if (Constant.Template.AES)
            json = await AesEncryption.Decrypt(json);

        return json;
    }
    #endregion
    #region Get
    T GetTemplate<T>(int id, Dictionary<int, T> templates) where T : ITemplate
    {
        return templates.TryGetValue(id, out T template) ? template : default;
    }

    public EnemyTemplate GetEnemyTemplate(int id)
    {
        return GetTemplate(id, m_enemyTemplates);
    }

    public FishTemplate GetFishTemplate(int id)
    {
        return GetTemplate(id, m_fishTemplates);
    }

    public ShopTemplate GetShopTemplate(int id)
    {
        return GetTemplate(id, m_shopTemplates);
    }

    public string GetText(int id)
    {
        TextTemplate template = GetTemplate(id, m_textTemplates);
        return Managers.Data.Setting.Language.Value == Define.Language.Kor ? template?.Kor : template?.Eng;
    }

    public FishLevelTemplate GetFishLevelTemplate(int level)
    {
        return m_fishLevelTemplates.Values.FirstOrDefault(template => template.Level == level);
    }

    public FishTemplate GetNextStarFishTemplate(FishTemplate template)
    {
        return m_fishTemplates.Values.FirstOrDefault(nextTemplate =>
            nextTemplate.Type == template.Type && nextTemplate.Kind == template.Kind &&
            nextTemplate.Star == template.Star + 1);
    }

    public List<StageTemplate> GetSingleStageTemplates()
    {
        return m_stageTemplates.Values.Where(template => template.Type == 1).ToList();
    }

    public StageTemplate GetSingleStageTemplate(int stage)
    {
        return m_stageTemplates.Values.FirstOrDefault(template =>
        template.Type == 1 && template.Stage == stage);
    }

    public StageTemplate GetStageTemplate(Define.StageType type, int stage = 0)
    {
        return m_stageTemplates.Values.FirstOrDefault(template =>
        template.Type == (int)type && template.Stage == stage);
    }
    #endregion
}
