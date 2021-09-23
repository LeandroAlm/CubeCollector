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
        [Tooltip("Player gameobject reference")]
        public GameObject player;
        [SerializeField]
        [Tooltip("Settings button reference")]
        private GameObject settingsBtt;
        [SerializeField]
        [Tooltip("Settings Panel reference")]
        public GameObject settingsPanel;
        [SerializeField]
        [Tooltip("Win Panel reference")]
        private GameObject winPanel;
        [SerializeField]
        [Tooltip("Lose Panel reference")]
        private GameObject losePanel;
        [SerializeField]
        [Tooltip("Play button reference")]
        private GameObject playBtt;
        #endregion vars

        #region internal vars
        private LevelDesign currentLevelDesign;
        #endregion internal vars

        #region custom methods
        public void InitLevel()
        {
            // Clear map
            Transform map = GameObject.Find("Map").transform;
            for (int i = 3; i < map.childCount; i++)
            {
                Destroy(map.GetChild(i).gameObject);
            }

            // UI start layout
            playBtt.SetActive(true);
            losePanel.SetActive(false);
            winPanel.SetActive(false);
            settingsBtt.SetActive(false);
            settingsPanel.SetActive(false);

            //Menu.MenuController.settingsController.currentLevel = 2; // Force Level in Game Scene
            Material playersMat = Resources.Load<Material>("Materials/Mat_Box_" + Menu.MenuController.settingsController.currentBoxID);
            LevelLoader levelLoader = gameObject.AddComponent<LevelLoader>();
            levelLoader.loadLevel(Menu.MenuController.settingsController.currentLevel, playersMat);
            Destroy(levelLoader);


            if (Menu.MenuController.settingsController.musicTrigger == (int)Settings.SettingsController.settingsTrigger.On)
            {
                AudioSource audio = GetComponent<AudioSource>();
                audio.clip = Resources.Load<AudioClip>("Audio/musicGame") as AudioClip;
                audio.volume = 0.025f;
                audio.loop = true;
                audio.Play();
            }

            player.GetComponent<PlayerController>().PlayerInit(playersMat);
            // Skybox change color
            //skyboxMaterial.SetColor("_TintColor", Color.red);
            //RenderSettings.skybox = skyboxMaterial;
        }

        /// <summary>
        /// Open Win panel
        /// </summary>
        public void FinishLevel()
        {
            var allLevels = Resources.LoadAll("Levels/", typeof(LevelDesign));

            if (Menu.MenuController.settingsController.maxLevel + 1 <= allLevels.Length)
            {
                if (Menu.MenuController.settingsController.maxLevel <= Menu.MenuController.settingsController.currentLevel)
                    Menu.MenuController.settingsController.maxLevel++;
            }
            else if(Menu.MenuController.settingsController.currentLevel == allLevels.Length)
            {
                winPanel.transform.Find("Next").gameObject.SetActive(false);
                winPanel.transform.Find("Warning").gameObject.SetActive(true);
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

