using System;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public class PlayerData : INamed
    {
        public int Coins;
        public int Health = 6;
        public Vector2 Position;
        public Vector2 Velocity;
        public List<string> Items = new List<string>();
        [field: SerializeField] public string Name { get; set; }

    }

    [Serializable]
    public class GameData
    {
        public List<PlayerData> PlayerDatas = new List<PlayerData>();

        public string GameName;

        public string CurrentLevelName;
        public List<LevelData> LevelDatas = new List<LevelData>();
    }

    [Serializable]
    public class LevelData
    {
        public string LevelName;
        public List<CoinData> CoinDatas = new List<CoinData>();
        public List<LaserSwitchData> LaserSwitchDatas = new List<LaserSwitchData>();
    }
    [Serializable]
    public class LaserSwitchData : INamed
    {
        public bool IsOn;
        [field: SerializeField] public string Name { get; set; }
    }

    [Serializable]
    public class CoinData : INamed
    {
        public bool IsCollected;
        [field: SerializeField] public string Name { get; set; }
    }

