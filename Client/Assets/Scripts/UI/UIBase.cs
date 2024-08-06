using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    #region Fields
    protected bool m_isInit = false;
    protected int m_type;
    protected Dictionary<Type, UnityEngine.Object[]> m_objects = new Dictionary<Type, UnityEngine.Object[]>();
    #endregion
    #region Properties
    public int Type { get => m_type; set => m_type = value; }
    #endregion
    #region Unity
    protected virtual void Start()
    {
        Init();
    }
    #endregion
    #region Init
    protected virtual bool Init()
    {
        if (m_isInit)
            return false;

        return m_isInit = true;
    }

    public virtual void RefreshUI()
    {

    }

    public virtual void Clear()
    {

    }
    #endregion
    #region Control
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        m_objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            objects[i] = typeof(T) == typeof(GameObject)
                ? gameObject.FindChild(names[i], true)
                : gameObject.FindChild<T>(names[i], true);

            if (objects[i] == null)
                Debug.LogWarning($"[UIBase:Bind] Failed to bind({names[i]})");
        }
    }

    protected void BindObject(Type type)
    {
        Bind<GameObject>(type);
    }

    protected void BindText(Type type)
    {
        Bind<TextMeshProUGUI>(type);
    }

    protected void BindImage(Type type)
    {
        Bind<Image>(type);
    }

    protected void BindButton(Type type)
    {
        Bind<Button>(type);
    }

    protected void BindToggle(Type type)
    {
        Bind<Toggle>(type);
    }
    #endregion
    #region Get
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        return m_objects.TryGetValue(typeof(T), out UnityEngine.Object[] objects) ? objects[idx] as T : null;
    }

    protected GameObject GetObject(int idx)
    {
        return Get<GameObject>(idx);
    }

    protected TextMeshProUGUI GetText(int idx)
    {
        return Get<TextMeshProUGUI>(idx);
    }

    protected Image GetImage(int idx)
    {
        return Get<Image>(idx);
    }

    protected Button GetButton(int idx)
    {
        return Get<Button>(idx);
    }

    protected Toggle GetToggle(int idx)
    {
        return Get<Toggle>(idx);
    }
    #endregion
}
