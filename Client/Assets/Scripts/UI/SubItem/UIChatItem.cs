using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIChatItem : UIBase
{
    #region Fields
    ChatData m_data;
    #endregion
    #region Properties
    public ChatData Data { set => m_data = value; }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        GetComponent<TextMeshProUGUI>().SetText($"{m_data.Name}:{m_data.Message}");

        return true;
    }
    #endregion
}
