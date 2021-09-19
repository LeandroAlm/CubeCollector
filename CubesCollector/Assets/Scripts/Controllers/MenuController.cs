// file=""MenuController.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 17/09/2021

#region usings
using Game.Controller.Settings;
using Game.Design.Level;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion usings

namespace Game.Controller.Menu
{
    public class MenuController : MonoBehaviour
    {
        #region variables
        [SerializeField][Tooltip("Reference of coin text gameobject")]
        private GameObject textCoin;
        [SerializeField][Tooltip("Reference of sound button gameobject")]
        private GameObject soundGO;
        [SerializeField][Tooltip("Reference of music button gameobject")]
        private GameObject musicGO;
        [SerializeField][Tooltip("Reference of vibration button gameobject")]
        private GameObject vibraGO;
        [SerializeField][Tooltip("Reference of levelscontainer gameobject")]
        private GameObject levelContainerGO;
        #endregion variables

        #region internal variables
        private int totalCoins = 0;
        private int maxLevel = 1;
        public static int currentLevel = 0;
        private SettingsController settingsController;
        private List<LevelDesign> allLevels;
        #endregion internal variables

        #region base methods
        private void Awake()
        {
            if (PlayerPrefs.GetInt("COINS") > 0)
                totalCoins = PlayerPrefs.GetInt("COINS");
            if (PlayerPrefs.GetInt("LEVEL") > 0)
                maxLevel = PlayerPrefs.GetInt("LEVEL");

            textCoin.GetComponent<TextMeshProUGUI>().text = totalCoins.ToString();

            settingsController = new SettingsController();
            settingsController.soundTrigger = true;
            settingsController.musicTrigger = true;
            settingsController.vibrationTrigger = true;
            settingsController.currentLevel = maxLevel;

            LevelsLoad();
        }
        #endregion base methods

        #region custom methods
        /// <summary>
        /// Close the game
        /// </summary>
        public void quitApplication()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }

        #region settings
        /// <summary>
        /// Set sound settings
        /// </summary>
        public void settingsSound()
        {
            if (settingsController.soundTrigger)
                soundGO.GetComponent<Image>().sprite = Resources.Load("UI/soundOff", typeof(Sprite)) as Sprite;
            else
                soundGO.GetComponent<Image>().sprite = Resources.Load("UI/soundOn", typeof(Sprite)) as Sprite;

            settingsController.soundTrigger = !settingsController.soundTrigger;
        }

        /// <summary>
        /// Set muisc settings
        /// </summary>
        public void settingsMusic()
        {
            if (settingsController.musicTrigger)
                musicGO.GetComponent<Image>().sprite = Resources.Load("UI/soundOff", typeof(Sprite)) as Sprite;
            else
                musicGO.GetComponent<Image>().sprite = Resources.Load("UI/soundOn", typeof(Sprite)) as Sprite;

            settingsController.musicTrigger = !settingsController.musicTrigger;
        }

        /// <summary>
        /// Set vibration settings
        /// </summary>
        public void settingsVibration()
        {
            if (settingsController.vibrationTrigger)
                vibraGO.GetComponent<Image>().sprite = Resources.Load("UI/vibrationOff", typeof(Sprite)) as Sprite;
            else
                vibraGO.GetComponent<Image>().sprite = Resources.Load("UI/vibrationOn", typeof(Sprite)) as Sprite;

            settingsController.vibrationTrigger = !settingsController.vibrationTrigger;
        }
        #endregion settings

        #region levels
        /// <summary>
        /// Loads all buttons for possible levels
        /// </summary>
        private void LevelsLoad()
        {
            int x = 1;
            int y = 1;
            int current_Y = -100;

            DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Levels");
            for (int i = 1; i < dir.GetFiles("*.asset").Length+1; i++)
            {
                GameObject levelBtt = Instantiate(Resources.Load<GameObject>("Prefabs/UI/LevelBtt"), levelContainerGO.transform) as GameObject;
                levelBtt.GetComponent<RectTransform>().anchoredPosition = new Vector2(125 + 210 * (x - 1), current_Y);
                levelBtt.name = "Btt_" + (i < 10 ? "0" : "") + i;
                levelBtt.transform.GetComponentInChildren<TextMeshProUGUI>().text = (i < 10 ? "0" : "") + i;

                // Need to create an int to fix i value, solved in stackOverflow
                int i_value = i;
                levelBtt.GetComponent<Button>().onClick.AddListener(delegate { OnClickLoadLevel(i_value); });

                x++;
                y++;
                if (y >= 5)
                {
                    x = 1;
                    y = 1;
                    current_Y -= 210;
                }
            }
        }
        
        private void OnClickLoadLevel(int a_level)
        {
            MenuController.currentLevel = a_level;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        #endregion levels
        #endregion custom methods
    }
}
