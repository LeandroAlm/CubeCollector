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
        private GameObject Player;
        [SerializeField]
        [Tooltip("Settings Panel reference")]
        private GameObject SettingsPanel;
        #endregion vars

        #region internal vars
        private LevelDesign currentLevelDesign;
        #endregion internal vars

        #region base method
        private void Awake()
        {
            LevelLoader levelLoader = gameObject.AddComponent<LevelLoader>();
            currentLevelDesign = levelLoader.loadLevel(Menu.MenuController.currentLevel > 0 ? Menu.MenuController.currentLevel : 2);

            Destroy(levelLoader);

        }
        #endregion base method

        #region custom methods
        public void OnStartClick()
        {
            Player.GetComponent<PlayerController>().playerMove();
        }

        public void OnSettingsClick()
        {
            if (!SettingsPanel.activeSelf)
            {
                SettingsPanel.SetActive(true);
                Player.GetComponent<PlayerController>().playerPause();
            }
            else
            {
                SettingsPanel.SetActive(false);
                Player.GetComponent<PlayerController>().playerMove();
            }
        }
    
        public void OnMenuClick()
        {
            // Back to Menu
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void OnExitClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        #endregion custom methods
    }
}

