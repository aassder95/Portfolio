using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHome : SceneBase
{
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        m_type = Define.Scene.Home;

        Managers.UI.ShowUIScene<UIHome>();

        Debug.Log("[SceneHome:Init]");

        return true;
    }

    public override void Clear()
    {
        base.Clear();

        Managers.Chat.Clear();
    }
    #endregion
}
