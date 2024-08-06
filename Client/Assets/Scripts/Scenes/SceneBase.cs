using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBase : MonoBehaviour
{
    #region Fields
    protected Define.Scene m_type = Define.Scene.None;
    protected bool m_isInit = false;
    #endregion
    #region Properties
    public Define.Scene Type => m_type;
    #endregion
    #region Unity
    void Start()
    {
        Init();
    }
    #endregion
    #region Init
    protected virtual bool Init()
    {
        if (m_isInit)
            return false;

        Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
        Managers.UI.Init();

        return m_isInit = true;
    }

    public virtual void Clear()
    {
        Managers.UI.Clear();
    }
    #endregion
}
