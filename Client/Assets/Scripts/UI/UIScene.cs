using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScene : UIBase
{
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceCamera);

        return true;
    }
    #endregion
}
