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
        private bool _Sound = true;
        private bool _Music = true;
        private bool _Vibration = true;
        private int _Level = 1;

        /// <summary> sound On/Off </summary>
        public bool soundTrigger
        {
            get { return _Sound; }
            set { _Sound = value; }
        }
        /// <summary> music On/Off </summary>
        public bool musicTrigger
        {
            get { return _Music; }
            set { _Music = value; }
        }
        /// <summary> vibartion On/Off </summary>
        public bool vibrationTrigger
        {
            get { return _Vibration; }
            set { _Vibration = value; }
        }
        /// <summary> vibartion On/Off </summary>
        public int currentLevel
        {
            get { return _Level; }
            set { _Level = value; }
        }
    }
}
