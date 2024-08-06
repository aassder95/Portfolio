using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChatData
{
    #region Fields
    int m_index;
    string m_name;
    string m_message;
    #endregion
    #region Properties
    public int Index => m_index;
    public string Name => m_name;
    public string Message => m_message;
    #endregion
    #region Init
    public ChatData(int index, string name, string message)
    {
        m_index = index;
        m_name = name;
        m_message = message;
    }
    #endregion
}

public class ChatManager
{
    #region Fields
    List<ChatData> m_chatDatas = new List<ChatData>();
    #endregion
    #region Init
    public void Clear()
    {
        m_chatDatas.Clear();
    }
    #endregion
    #region Set
    public void SetData(JArray chatDatas)
    {
        foreach (JToken chatData in chatDatas)
        {
            int index = chatData.ToInt("idx");

            if (m_chatDatas.Any(data => data.Index == index))
                continue;

            ChatData data = new ChatData(index, chatData.ToStringValue("name"), chatData.ToStringValue("message"));
            m_chatDatas.Add(data);

            Managers.UI.CreateUIChat(data);
        }
    }
    #endregion
}
