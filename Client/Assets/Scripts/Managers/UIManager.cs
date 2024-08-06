using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class UIManager
{
    #region Fields
    int m_order = 10;
    GameObject m_root;
    UIScene m_uiScene;
    Stack<UIPopup> m_uiPopups = new Stack<UIPopup>();
    Subject<Unit> m_onNewMessage = new Subject<Unit>();
    #endregion
    #region Properties
    public IObservable<Unit> OnNewMessage => m_onNewMessage;
    #endregion
    #region Init
    public void Init()
    {
        m_root = new GameObject("@UIRoot");
    }

    public void RefreshUI()
    {
        m_uiScene?.RefreshUI();

        foreach (UIPopup uiPopup in m_uiPopups)
            uiPopup?.RefreshUI();
    }

    public void Clear()
    {
        CloseAllUIPopup();
        m_uiScene.Clear();
        m_uiScene = null;
        Managers.Resource.Destroy(m_root);
        m_root = null;
        m_order = 10;
    }
    #endregion
    #region Control
    public T ShowUIScene<T>() where T : UIScene
    {
        T uiScene = InstantiateUI<T>($"Scene/{typeof(T).Name}", m_root);
        m_uiScene = uiScene;
        return uiScene;
    }

    public T ShowUIPopup<T>() where T : UIPopup
    {
        T uiPopup = InstantiateUI<T>($"Popup/{typeof(T).Name}", m_root);
        m_uiPopups.Push(uiPopup);
        return uiPopup;
    }

    public UIConfirm ShowUIComfirmPopup(Define.UIConfirm type)
    {
        UIConfirm uiPopup = ShowUIPopup<UIConfirm>();
        uiPopup.Type = (int)type;
        return uiPopup;
    }

    public void ShowCombatText(Define.UICombatText type, Vector3 pos, int value)
    {
        if (!Managers.Data.Setting.IsDamage.Value)
            return;

        UICombatText uiCombatTxt = InstantiateUI<UICombatText>("World/UICombatText", m_root);
        uiCombatTxt.Type = (int)type;
        uiCombatTxt.SetData(pos, value);
        SetCanvas(uiCombatTxt.gameObject, RenderMode.WorldSpace);
        uiCombatTxt.ShowCombatText();
    }

    public void CreateUISubItems<T>(GameObject parent, int cnt) where T : UIBase
    {
        for (int i = 0; i < cnt; i++)
        {
            T uiSubItem = InstantiateUI<T>($"SubItem/{typeof(T).Name}", parent);
            uiSubItem.Type = i;
        }
    }

    public T CreateUIWorld<T>(GameObject parent) where T : UIBase
    {
        T uiWorld = InstantiateUI<T>($"World/{typeof(T).Name}", parent);
        SetCanvas(uiWorld.gameObject, RenderMode.WorldSpace);
        return uiWorld;
    }

    public void CreateUIChat(ChatData data)
    {
        UIHome uiHome = FindUIScene<UIHome>();
        if (uiHome == null)
            return;

        UIChatItem uiChatItem = InstantiateUI<UIChatItem>($"SubItem/UIChatItem", uiHome.ChatContent);
        uiChatItem.Data = data;

        m_onNewMessage.OnNext(Unit.Default);
    }

    public T InstantiateUI<T>(string path, GameObject parent) where T : UIBase
    {
        GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/{path}");
        GameObject go = Managers.Resource.Instantiate(prefab, parent.transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = prefab.transform.position;
        return go.GetOrAddComponent<T>();
    }

    public T FindUIScene<T>() where T : UIScene
    {
        if (m_uiScene == null || m_uiScene.GetType() != typeof(T))
            return null;

        return m_uiScene as T;
    }

    public T FindUIPopup<T>() where T : UIPopup
    {
        return m_uiPopups.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
    }

    public T PeekUIPopup<T>() where T : UIPopup
    {
        return m_uiPopups.Count > 0 ? m_uiPopups.Peek() as T : null;
    }

    public void CloseUIPopup()
    {
        if (m_uiPopups.Count == 0)
            return;

        UIPopup uiPopup = m_uiPopups.Pop();
        uiPopup.Clear();

        Managers.Resource.Destroy(uiPopup.gameObject);

        m_order--;
    }

    public void CloseUIPopup(UIPopup uiPopup)
    {
        if (m_uiPopups.Count == 0 || m_uiPopups.Peek() != uiPopup)
            return;

        CloseUIPopup();
    }

    public void CloseAllUIPopup()
    {
        while (m_uiPopups.Count > 0)
            CloseUIPopup();
    }
    #endregion
    #region Is
    public bool IsUIPopup()
    {
        return m_uiPopups.Count > 0;
    }
    #endregion
    #region Set
    public void SetCanvas(GameObject go, RenderMode renderMode)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = renderMode;

        if (renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = Camera.main;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 0;
        }
        else if (renderMode == RenderMode.ScreenSpaceOverlay)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = m_order++;
        }
    }
    #endregion
}
