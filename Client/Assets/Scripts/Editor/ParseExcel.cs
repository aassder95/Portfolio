using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using ExcelDataReader;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class ParseExcel : EditorWindow
{
    #region Fields
    static string m_excelPath = $"{Application.dataPath}/Resources/Templates/Excel";
    static string m_serverPath = Path.GetFullPath($"{Application.dataPath}/../../PixelFishDefendersServer/template");
    static string m_clientPath = $"{Application.dataPath}/Resources/Templates";
    #endregion
    #region Control
    [MenuItem("Tools/ParseExcel")]
    public static async UniTaskVoid Parse()
    {
        await ClearDir(m_clientPath);
        await ParseTemplates(m_clientPath, Constant.Template.AES, Constant.Template.GZIP);

        await ClearDir(m_serverPath);
        await ParseTemplates(m_serverPath);
    }
    #endregion
    #region Utility
    static async UniTask ClearDir(string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo info = new DirectoryInfo(path);
            foreach (FileInfo file in info.GetFiles())
                file.Delete();
        }

        await UniTask.Yield();
    }

    static async UniTask ParseTemplates(string path, bool isAes = false, bool isGZip = false)
    {
        await ParseTemplate<EnemyTemplate>(path, isAes, isGZip);
        await ParseTemplate<FishLevelTemplate>(path, isAes, isGZip);
        await ParseTemplate<FishTemplate>(path, isAes, isGZip);
        await ParseTemplate<ShopTemplate>(path, isAes, isGZip);
        await ParseTemplate<StageTemplate>(path, isAes, isGZip);
        await ParseTemplate<TextTemplate>(path, isAes, isGZip);
    }

    static async UniTask ParseTemplate<T>(string path, bool isAes = false, bool isGZip = false) where T : ITemplate, new()
    {
        DataTable data = await LoadExcel(typeof(T).Name);

        List<T> templates = data.AsEnumerable().Select(row =>
        {
            T template = new T();
            template.Load(row);
            return template;
        }).ToList();

        await WriteJson(templates, path, isAes, isGZip);
    }

    static async UniTask<DataTable> LoadExcel(string name)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (FileStream stream = File.Open($"{m_excelPath}/{name}.xlsx", FileMode.Open, FileAccess.Read))
        using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
        {
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
            });

            return await UniTask.FromResult(result.Tables[0]);
        }
    }

    static async UniTask WriteJson<T>(List<T> templates, string path, bool isAes, bool isGZip) where T : ITemplate
    {
        string name = typeof(T).Name;
        string json = JsonConvert.SerializeObject(templates, Formatting.None);

        if (isAes)
            json = await AesEncryption.Encrypt(json);

        if (isGZip)
            json = await GZipCompression.Compress(json);

        string fileExtension = isAes || isGZip ? "pft" : "json";
        File.WriteAllText($"{path}/{name}.{fileExtension}", json);
        AssetDatabase.Refresh();

        Debug.Log($"[ParseExcel:WriteJson] {name} parse success!");
        await UniTask.Yield();
    }
    #endregion
}
