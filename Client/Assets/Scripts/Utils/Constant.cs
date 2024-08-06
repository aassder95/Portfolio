using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    public static class Stage
    {
        public static readonly Vector2 START_POS = new Vector2(2.6f, 2.5f);

        public static class Waypoint
        {
            public static readonly Vector2Int CNT = new Vector2Int(2, 2);
            public static readonly Vector2 OFFSET = new Vector2(4.4f, 3.0f);
            public static readonly Vector2 WORLD_OFFSET = new Vector2(-0.1f, 1.0f);
        }

        public static class Slot
        {
            public static readonly Vector2Int CNT = new Vector2Int(3, 2);
            public static readonly Vector2 OFFSET = new Vector2(1.3f, 1.4f);
            public static readonly Vector2 WORLD_OFFSET = new Vector2(-0.1f, 1.0f);
        }

        public static class Fish
        {
            public static readonly Vector2Int CNT = new Vector2Int(2, 2);
            public static readonly Vector2 OFFSET = new Vector2(0.6f, 0.6f);
        }
    }

    public static class Game
    {
        public const int MAX_STAR = 4;
        public const int MAX_FISH_CNT = 24;
        public const int MAX_ENEMY_CNT = 50;
        public const float FAST_GAME_SPEED = 2.0f;
        public const float NORMAL_GAME_SPEED = 1.0f;
        public const float FIXED_DELTA_TIME = 0.02f;

        public static class Test
        {
            public const int START_PEARL = 2000;
            public const int COST_SPAWN = 5;
            public const int COST_REPOS = 5;
            public const int START_UPGRADE = 10;
            public const int COST_UPGRADE = 10;
            public const int UPGRADE_BONUS_RATE = 10;
            public const int MAX_UPGRADE = 10;
            public const int MAX_BONUS_PEARL = 20;
        }
    }

    public static class UI
    {
        public const float REMAIN_TIME_WARNING_THRESHOLD = 5.0f;
        public const float ENEMY_CNT_SPEED = 10.0f;

        public static class Combat
        {
            public const float STAY_DURATION = 0.15f;
            public const float FADE_DURATION = 0.1f;
            public const float DAMAGE_SIZE = 20.0f;
            public const float CRI_DAMAGE_SIZE = 25.0f;
            public const float DAMAGE_OFFSET_Y = 0.1f;
            public const float PEARL_SIZE = 20.0f;
        }
    }

    public static class Template
    {
        public const bool AES = true;
        public const bool GZIP = true;
    }

    public static class Deck
    {
        public const int MAX_FISH_CNT = 5;
    }

    public static class Path
    {
        public static class Name
        {
            public const string SETTING_DATA = "SettingData";
        }
    }

    public static class URL
    {
        //public const string SERVER_URL = "localhost";
        public const string SERVER_URL = "ec2-3-38-96-103.ap-northeast-2.compute.amazonaws.com";
        public const string SERVER_PORT = "3000";
    }
}
