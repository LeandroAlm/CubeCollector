// file=""SettingsController.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 17/09/2021

#region usings
using UnityEngine;
#endregion usings

namespace Game.Controller.Settings
{
    public class SettingsController
    {
        #region internal variables
        private int _Coin = 0;
        private int _Sound = 1;
        private int _Music = 1;
        private int _Vibration = 1;
        private int _Level = 1;
        private int _MaxLevel = 1;
        private int _BoxID = 0;
        private string _Shop = "";
        public enum settingsTrigger
        {
            On = 1,
            Off = 2,
        }
        #endregion internal variables

        #region get/set variabels
        /// <summary> sound On/Off </summary>
        public int Coins
        {
            get { return _Coin; }
            set
            {
                _Coin = value;
                PlayerPrefs.SetInt("COIN", _Coin);
            }
        }
        /// <summary> sound On/Off </summary>
        public int soundTrigger
        {
            get { return _Sound; }
            set
            {
                _Sound = value;
                PlayerPrefs.SetInt("SOUND", _Sound);
            }
        }
        /// <summary> music On/Off </summary>
        public int musicTrigger
        {
            get { return _Music; }
            set
            {
                _Music = value;
                PlayerPrefs.SetInt("MUSIC", _MaxLevel);
            }
        }
        /// <summary> vibartion On/Off </summary>
        public int vibrationTrigger
        {
            get { return _Vibration; }
            set
            {
                _Vibration = value;
                PlayerPrefs.SetInt("VIBRATION", _Vibration);
            }
        }
        /// <summary> vibartion On/Off </summary>
        public int maxLevel
        {
            get { return _MaxLevel; }
            set 
            {
                _MaxLevel = value;
                PlayerPrefs.SetInt("LEVEL", _MaxLevel);
            }
        }
        /// <summary> vibartion On/Off </summary>
        public int currentLevel
        {
            get { return _Level; }
            set { _Level = value; }
        }
        /// <summary> vibartion On/Off </summary>
        public int currentBoxID
        {
            get { return _BoxID; }
            set 
            {
                _BoxID = value;
                PlayerPrefs.SetInt("BOX", _BoxID);
            }
        }
        /// <summary> shop Materials owned </summary>
        public string currentShop
        {
            get { return _Shop; }
            set 
            { 
                _Shop = value;
                PlayerPrefs.SetString("SHOP", _Shop);
            }
        }
        #endregion get/set variables

        #region custom methods
        public void InitSettings()
        {
            // if possible try load
            // else sety default

            if (PlayerPrefs.GetInt("COIN") > 0)
                _Coin = PlayerPrefs.GetInt("COIN");
            else
                Coins = 0;

            if (PlayerPrefs.GetInt("SOUND") > 0)
                _Sound = PlayerPrefs.GetInt("SOUND");
            else
                soundTrigger = (int)settingsTrigger.On;

            if (PlayerPrefs.GetInt("MUSIC") > 0)
                _Music = PlayerPrefs.GetInt("MUSIC");
            else
                musicTrigger = (int)settingsTrigger.On;

            if (PlayerPrefs.GetInt("VIBRATION") > 0)
                _Vibration = PlayerPrefs.GetInt("VIBRATION");
            else
                vibrationTrigger = (int)settingsTrigger.On;

            if (PlayerPrefs.GetInt("LEVEL") > 0)
                _MaxLevel = PlayerPrefs.GetInt("LEVEL");
            else
                maxLevel = 1;

            if (PlayerPrefs.GetInt("BOX") > 0)
                _BoxID = PlayerPrefs.GetInt("BOX");
            else
                currentBoxID = 0;

            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("SHOP")))
                _Shop = PlayerPrefs.GetString("SHOP");
            else
                currentShop = "0;";
        }
        #endregion custom methods
    }
}
