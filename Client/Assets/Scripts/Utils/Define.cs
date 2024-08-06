using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum Scene
    {
        None,
        Title,
        Home,
        Game,
    }

    public enum UIConfirm
    {
        WinNext,
        WinEnd,
        Lose,
        Result,
        GiveUp,
        LevelUp,
        NameIsNull,
        NameIsLonger,
        NotFoundTemplate,
        NotEnoughGold,
        NotEnoughGem,
        NotEnoughCard,
        MaxCardLevel,
        BuyCard,
    }

    public enum UIHomeMenuItem
    {
        None = -1,
        Deck,
        Home,
        Shop,
    }

    public enum UICombatText
    {
        AttDamage,
        CriDamage,
        Pearl,
    }

    public enum Sound
    {
        Bgm,
        Sfx,
        Max,
    }

    public enum Language
    {
        Eng,
        Kor,
        Max,
    }

    public enum Protocol
    {
        REQ_LOGIN = 100000010,
        RES_LOGIN,
        REQ_CREATE_USER = 100000020,
        RES_CREATE_USER,
        REQ_CARD_LEVEL_UP = 100000030,
        RES_CARD_LEVEL_UP,
        REQ_STAGE_END = 100000040,
        RES_STAGE_END,
        REQ_RANK = 100000050,
        RES_RANK,
        REQ_CHAT = 100000060,
        RES_CHAT,
        REQ_CHAT_SEND = 100000070,
        RES_CHAT_SEND,
        REQ_DECK_CHANGE = 100000080,
        RES_DECK_CHANGE,
        REQ_SHOP_RAND_CARD = 100000090,
        RES_SHOP_RAND_CARD,
        REQ_SHOP_BUY_ITEM = 100000100,
        RES_SHOP_BUY_ITEM,
    }

    public enum ErrorCode
    {
        OK = 200,
        BAD_REQUEST = 400,
        UNAUTHORIZED = 401,
        FORBIDDEN = 403,
        NOT_FOUND = 404,
        INTERNAL_SERVER_ERROR = 500,

        NOT_FOUND_TEMPLATE = 1001,
        NOT_FOUND_USER = 1002,
        NOT_FOUND_STAGE = 1003,
        NOT_FOUND_CARD = 1004,
        NOT_FOUND_DECK = 1005,
        NOT_FOUND_FILE = 1006,
        NOT_FOUND_USER_PURCHASES = 1007,

        CREATE_USER = 1201,

        NOT_ENOUGH_GOLD = 1401,
        NOT_ENOUGH_GEM = 1402,
        NOT_ENOUGH_CARD = 1403,

        MAX_CARD_LEVEL = 1601,

        ALREADY_PURCHASED = 1801,
    }

    public enum Tier
    {
        None = -1,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythology
    }

    public enum StageType
    {
        Single = 1,
        Infinite,
    }
}
