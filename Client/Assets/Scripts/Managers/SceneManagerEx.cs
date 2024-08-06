using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    #region Properties
    public SceneBase CurScene => GameObject.FindObjectOfType<SceneBase>();
    #endregion
    #region Init
    public void Clear()
    {
        CurScene?.Clear();
    }
    #endregion
    #region Control
    public void LoadScene(Define.Scene type)
    {
        Clear();

        SceneManager.LoadScene(Enum.GetName(typeof(Define.Scene), type));
    }

    public void LoadGameScene(Define.StageType type)
    {
        Managers.Stage.Type = type;
        LoadScene(Define.Scene.Game);
    }
    #endregion
}
