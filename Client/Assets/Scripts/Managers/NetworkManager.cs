using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager
{
    #region Control
    public async UniTask<string> PostRequest(string jsonData)
    {
        Debug.Log($"[NetworkManager:PostRequest] {jsonData}");

        using (UnityWebRequest www = new UnityWebRequest($"http://{Constant.URL.SERVER_URL}:{Constant.URL.SERVER_PORT}", "POST"))
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            www.SendWebRequest().completed += _ => completionSource.TrySetResult();

            await completionSource.Task;

            if (www.result == UnityWebRequest.Result.Success)
            {
                string res = www.downloadHandler.text;
                Debug.Log("[NetworkManager:PostRequest] Response: " + res);
                return res;
            }
            else
            {
                Debug.LogError($"[NetworkManager:PostRequest] POST request error: {www.error}");
                return null;
            }
        }
    }

    async UniTask Request(Define.Protocol protocol, Dictionary<string, object> data = null)
    {
        JObject packet = new JObject
        {
            { "protocol", (int)protocol },
            { "uuid", Managers.Data.Setting.UUID }
        };

        if (data != null)
            packet.Add("data", JObject.FromObject(data));

        string response = await PostRequest(packet.ToString());
        Response(response);
    }

    void Response(string res)
    {
        if (res == null)
        {
            Debug.LogError("[NetworkManager:Response] Response is null");
            return;
        }

        JObject resData = JsonConvert.DeserializeObject<JObject>(res);

        Define.ErrorCode errorCode = (Define.ErrorCode)resData.ToInt("errorCode");
        ProcessError(errorCode);

        if (errorCode != Define.ErrorCode.OK)
            return;

        ProcessProtocol((Define.Protocol)resData.ToInt("protocol"), resData.ToJObject("data"));
    }

    void ProcessError(Define.ErrorCode errorCode)
    {
        switch (errorCode)
        {
            case Define.ErrorCode.NOT_FOUND_TEMPLATE:
                Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.NotFoundTemplate);
                break;
            case Define.ErrorCode.CREATE_USER:
                Managers.UI.ShowUIPopup<UIName>();
                break;
            case Define.ErrorCode.NOT_ENOUGH_GOLD:
                Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.NotEnoughGold);
                break;
            case Define.ErrorCode.NOT_ENOUGH_GEM:
                Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.NotEnoughGem);
                break;
            case Define.ErrorCode.NOT_ENOUGH_CARD:
                Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.NotEnoughCard);
                break;
            case Define.ErrorCode.MAX_CARD_LEVEL:
                Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.MaxCardLevel);
                break;
        }
    }

    void ProcessProtocol(Define.Protocol protocol, JObject data)
    {
        switch (protocol)
        {
            case Define.Protocol.RES_LOGIN:
            case Define.Protocol.RES_CREATE_USER:
                ResponseLogin(data);
                break;
            case Define.Protocol.RES_DECK_CHANGE:
                ResponseDeckChange(data);
                break;
            case Define.Protocol.RES_CARD_LEVEL_UP:
                ResponseCardLevelUp(data);
                break;
            case Define.Protocol.RES_STAGE_END:
                ResponseStageEnd(data);
                break;
            case Define.Protocol.RES_RANK:
                ResponseRank(data);
                break;
            case Define.Protocol.RES_CHAT:
            case Define.Protocol.RES_CHAT_SEND:
                ResponseChat(data);
                break;
            case Define.Protocol.RES_SHOP_RAND_CARD:
                ResponseShopRandCard(data);
                break;
            case Define.Protocol.RES_SHOP_BUY_ITEM:
                ResponseShopBuyItem(data);
                break;
            default:
                Debug.LogWarning($"[NetworkManager:Response] Unhandled protocol: {protocol}");
                break;
        }
    }
    #endregion
    #region Request
    public async void RequestLogin()
    {
        Debug.Log("[NetworkManager:RequestLogin]");

        await Request(Define.Protocol.REQ_LOGIN);
    }

    public async void RequestCreateUser(string name)
    {
        Debug.Log("[NetworkManager:RequestCreateUser]");

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["name"] = name;

        await Request(Define.Protocol.REQ_CREATE_USER, data);
    }

    public async void RequestCardLevelUp(int id)
    {
        Debug.Log("[NetworkManager:RequestLevelUpCard]");

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["id"] = id;

        await Request(Define.Protocol.REQ_CARD_LEVEL_UP, data);
    }

    public async void RequestStageEnd(Define.StageType type, int clearStage, bool isWin)
    {
        Debug.Log("[NetworkManager:RequestStageEnd]");

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["type"] = (int)type;
        data["clearStage"] = clearStage;
        data["isWin"] = isWin;

        await Request(Define.Protocol.REQ_STAGE_END, data);
    }

    public async void RequestRank(Define.StageType type)
    {
        Debug.Log("[NetworkManager:RequestRank]");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["type"] = (int)type;

        await Request(Define.Protocol.REQ_RANK, data);
    }

    public async void RequestChat()
    {
        Debug.Log("[NetworkManager:RequestChat]");

        await Request(Define.Protocol.REQ_CHAT);
    }

    public async void RequestChatSend(string message)
    {
        Debug.Log("[NetworkManager:RequestChatSend]");

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["message"] = message;

        await Request(Define.Protocol.REQ_CHAT_SEND, data);
    }

    public async void RequestDeckChange(int deckId, int selectCardId)
    {
        Debug.Log("[NetworkManager:RequestChangeDeck]");

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["deckId"] = deckId;
        data["selectCardId"] = selectCardId;

        await Request(Define.Protocol.REQ_DECK_CHANGE, data);
    }

    public async void RequestShopRandCard()
    {
        Debug.Log("[NetworkManager:RequestShopRandCard]");

        await Request(Define.Protocol.REQ_SHOP_RAND_CARD);
    }

    public async void RequestShopBuyItem(int id)
    {
        Debug.Log("[NetworkManager:RequestShopBuyItem]");

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["id"] = id;

        await Request(Define.Protocol.REQ_SHOP_BUY_ITEM, data);
    }
    #endregion
    #region Response
    void ResponseLogin(JObject data)
    {
        Debug.Log("[NetworkManager:ResponseLogin]");

        Managers.Shop.SetData(data);
        Managers.Data.SetData(data);
        Managers.Card.SetData(data);

        Managers.Scene.LoadScene(Define.Scene.Home);
    }

    void ResponseCardLevelUp(JObject data)
    {
        Debug.Log("[NetworkManager:ResponseLevelUpCard]");

        Managers.Data.Game.Gold.Value = data.ToInt("gold");
        Managers.Card.SetCardData(data.ToJObject("card"));

        Managers.UI.FindUIPopup<UICardInfo>().RefreshUI();
        Managers.UI.FindUIScene<UIHome>().RefreshUICardItem();
        Managers.UI.CloseUIPopup();
    }

    void ResponseStageEnd(JObject data)
    {
        Debug.Log("[NetworkManager:ResponseStageEnd]");

        if (data != null)
        {
            Managers.Data.Game.Gold.Value = data.ToInt("gold");

            if (Managers.Stage.Type == Define.StageType.Single)
                Managers.Data.Game.ClearStage = data.ToInt("clearStage");
        }

        Managers.Stage.ShowUI();
    }

    void ResponseRank(JObject data)
    {
        Debug.Log("[NetworkManager:ResponseRank]");

        Managers.Rank.SetData(data);

        Managers.UI.ShowUIPopup<UIRank>();
    }

    void ResponseChat(JObject data)
    {
        Debug.Log("[NetworkManager:ResponseChat]");

        Managers.Chat.SetData(data.ToJArray("chats"));
    }

    void ResponseDeckChange(JObject data)
    {
        Debug.Log("[NetworkManager:ResponseChangeDeck]");

        Managers.Card.SetDeckData(data.ToJArray("deck"));
        Managers.Card.RefreshDeck();
    }

    void ResponseShopRandCard(JObject data)
    {
        Debug.Log("[NetworkManager:ResponseShopRandCard]");

        Managers.Shop.SetData(data);
    }

    void ResponseShopBuyItem(JObject data)
    {
        Debug.Log("[NetworkManager:ResponseShopBuyItem]");

        Managers.Data.Game.Gold.Value = data.ToInt("gold");

        Managers.Card.SetCardData(data.ToJObject("card"));
        Managers.Shop.SetData(data);

        Managers.UI.CloseUIPopup();
    }
    #endregion
}
