// file=""GameController.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 17/09/2021

#region usings
using Game.Controller.Player;
using Game.Design.Juntion;
using Game.Design.Level;
using Game.Loader.Level;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#endregion usings

namespace Game.Controller.Game
{
    public class GameController : MonoBehaviour
    {
        #region vars
        [SerializeField]
        [Tooltip("Player object reference")]
        public GameObject player;
        [SerializeField]
        [Tooltip("Settings button object reference")]
        private GameObject settingsBtt;
        [SerializeField]
        [Tooltip("Win Panel object reference")]
        private GameObject winPanel;
        [SerializeField]
        [Tooltip("Lose Panel object reference")]
        private GameObject losePanel;
        #endregion vars

        #region internal vars
        /// <summary>
        /// levels amount in Resources/Levels folder
        /// </summary>
        private int levelAmount = 0;
        /// <summary>
        /// Game background music reference
        /// </summary>
        private AudioClip music_GameBackground;
        /// <summary>
        /// Level loader reference
        /// </summary>
        private LevelLoader levelLoader;
        #endregion internal vars

        #region custom methods
        public void InitLevel()
        {
            if (levelAmount <= 0)
                levelAmount = Resources.LoadAll("Levels/", typeof(LevelDesign)).Length;

            if (music_GameBackground == null)
                music_GameBackground = Resources.Load<AudioClip>("Audio/musicGame") as AudioClip;

            // Clear map
            Transform map = GameObject.Find("Map").transform;
            for (int i = 3; i < map.childCount; i++)
            {
                Destroy(map.GetChild(i).gameObject);
            }

            // need load every enter because could change material
            Material playersMat = Resources.Load<Material>("Materials/Mat_Box_" + Menu.MenuController.settingsController.currentBoxID);

            if (levelLoader == null)
                levelLoader = gameObject.GetComponent<LevelLoader>();

            levelLoader.LoadLevel(Menu.MenuController.settingsController.currentLevel, playersMat);

            if (Menu.MenuController.settingsController.musicTrigger == (int)Settings.SettingsController.settingsTrigger.On)
            {
                AudioSource audio = GetComponent<AudioSource>();
                audio.clip = music_GameBackground;
                audio.volume = 0.025f;
                audio.loop = true;
                audio.Play();
            }

            settingsBtt.SetActive(false);
            player.GetComponent<PlayerController>().PlayerInit(playersMat);
        }

        /// <summary>
        /// Open Win panel
        /// </summary>
        public void FinishLevel()
        {
            if (Menu.MenuController.settingsController.maxLevel + 1 <= levelAmount)
            {
                if (Menu.MenuController.settingsController.maxLevel <= Menu.MenuController.settingsController.currentLevel)
                    Menu.MenuController.settingsController.maxLevel++;
            }
            else if(Menu.MenuController.settingsController.currentLevel == levelAmount)
            {
                winPanel.transform.Find("Next").gameObject.SetActive(false);
                winPanel.transform.Find("Warning").gameObject.SetActive(true);
            }
            else
            {
                winPanel.transform.Find("Next").gameObject.SetActive(true);
                winPanel.transform.Find("Warning").gameObject.SetActive(false);
            }

            settingsBtt.SetActive(false);
            winPanel.SetActive(true);
        }

        /// <summary>
        /// Open Lose panel
        /// </summary>
        public void LoseLevel()
        {
            losePanel.SetActive(true);
            settingsBtt.SetActive(false);
        }
        
        public void DisableSettingsButton()
        {
            settingsBtt.SetActive(false);
        }
        #endregion custom methods
    }
}

