using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTitle : SceneBase
{
    #region Unity
    async void Start()
    {
        Managers.Init();
        await Managers.Template.LoadCoreTemplates();

        Init();
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        m_type = Define.Scene.Title;

        Managers.UI.ShowUIScene<UITitle>();

        Debug.Log("[SceneTitle:Init]");

        return true;
    }

    public override void Clear()
    {
        base.Clear();
    }
    #endregion
}