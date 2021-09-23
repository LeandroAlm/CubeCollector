// file=""UIController.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 22/09/2021

#region usings
using Game.Controller.Settings;
using Game.Controller.Menu;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Game.Controller.Game;
using Game.Controller.Player;
using System.Collections;
#endregion usings

namespace Game.Controller.UI
{
    public class UIController : MonoBehaviour
    {
        #region variables
        [SerializeField]
        [Tooltip("Menu UI object reference")]
        private GameObject menuUIObject;
        [SerializeField]
        [Tooltip("Game UI object reference")]
        public GameObject gameUIObject;
        [SerializeField]
        [Tooltip("Settings panel reference")]
        private GameObject settingsPanel;
        [SerializeField]
        [Tooltip("Lose panel reference")]
        private GameObject losePanel;
        [SerializeField]
        [Tooltip("Win panel reference")]
        private GameObject winPanel;
        [SerializeField]
        [Tooltip("Settings panel in Game reference")]
        private GameObject gameSettingsPanel;
        [SerializeField]
        [Tooltip("Shop coins object reference")]
        private GameObject shopCoinGO;
        [SerializeField]
        [Tooltip("Shop buttons container object reference")]
        private GameObject shopContainer;
        [SerializeField]
        [Tooltip("3D camera reference")]
        private Camera gameCamera;
        [SerializeField]
        [Tooltip("UI Camera reference")]
        public Camera uiCamera;
        [SerializeField]
        [Tooltip("Play button reference")]
        private GameObject gamePlayBtt;
        [SerializeField]
        [Tooltip("Sound button object, in settings, reference")]
        private GameObject soundGO;
        [SerializeField]
        [Tooltip("Music button object, in settings, reference")]
        private GameObject musicGO;
        [SerializeField]
        [Tooltip("Vibration button object, in settings, reference")]
        private GameObject vibraGO;
        [SerializeField]
        [Tooltip("Player object, reference")]
        private GameObject player;
        #endregion variables

        #region base methods
        private void Awake()
        {
            SettingPanelInit();
            StartBackgroundMusic();

            for (int i = 0; i < menuUIObject.transform.childCount; i++)
            {
                if (i <= 1)
                    menuUIObject.transform.GetChild(i).gameObject.SetActive(true);
                else
                    menuUIObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            gameUIObject.SetActive(false);
        }
        #endregion base methods

        #region on click buttons
        #region SETTINGS
        /// <summary>
        /// Set sound settings
        /// </summary>
        public void OnSoundClick()
        {
            if (MenuController.settingsController.soundTrigger == (int)SettingsController.settingsTrigger.On)
            {
                soundGO.GetComponent<Image>().sprite = Resources.Load("UI/soundOff", typeof(Sprite)) as Sprite;
                MenuController.settingsController.soundTrigger = (int)SettingsController.settingsTrigger.Off;
            }
            else
            {
                soundGO.GetComponent<Image>().sprite = Resources.Load("UI/soundOn", typeof(Sprite)) as Sprite;
                MenuController.settingsController.soundTrigger = (int)SettingsController.settingsTrigger.On;
            }

        }

        /// <summary>
        /// Set muisc settings
        /// </summary>
        public void OnMusicClick()
        {
            if (MenuController.settingsController.musicTrigger == (int)SettingsController.settingsTrigger.On)
            {
                if (GetComponent<AudioSource>().isPlaying)
                    GetComponent<AudioSource>().Stop();

                musicGO.GetComponent<Image>().sprite = Resources.Load("UI/soundOff", typeof(Sprite)) as Sprite;
                MenuController.settingsController.musicTrigger = (int)SettingsController.settingsTrigger.Off;
            }
            else
            {
                musicGO.GetComponent<Image>().sprite = Resources.Load("UI/soundOn", typeof(Sprite)) as Sprite;
                MenuController.settingsController.musicTrigger = (int)SettingsController.settingsTrigger.On;
                StartBackgroundMusic();
            }
        }

        /// <summary>
        /// Set vibration settings
        /// </summary>
        public void OnVibrationClick()
        {
            if (MenuController.settingsController.vibrationTrigger == (int)SettingsController.settingsTrigger.On)
            {
                vibraGO.GetComponent<Image>().sprite = Resources.Load("UI/vibrationOff", typeof(Sprite)) as Sprite;
                MenuController.settingsController.vibrationTrigger = (int)SettingsController.settingsTrigger.Off;
            }
            else
            {
                vibraGO.GetComponent<Image>().sprite = Resources.Load("UI/vibrationOn", typeof(Sprite)) as Sprite;
                MenuController.settingsController.vibrationTrigger = (int)SettingsController.settingsTrigger.On;
            }
        }
        #endregion SETTINGS

        #region LEVEL
        /// <summary>
        /// Load level by ID
        /// </summary>
        /// <param name="a_level">Level ID</param>
        public void OnLoadLevelClick(int a_level)
        {
            MenuController.settingsController.currentLevel = a_level;
            menuUIObject.SetActive(false);
            gameUIObject.SetActive(true);
            gameCamera.gameObject.SetActive(true);
            uiCamera.clearFlags = CameraClearFlags.Depth;

            GameUILayoutInit();
            GetComponent<GameController>().InitLevel();
        }
        #endregion LEVEL

        #region SHOP
        /// <summary>
        /// Buy and save option of materials
        /// </summary>
        /// <param name="a_ID"></param>
        public void OnShopItemBuyClick(GameObject a_BttReference)
        {
            string current_shop = MenuController.settingsController.currentShop;

            if (MenuController.settingsController.Coins - GetComponent<MenuController>().shopCost >= 0)
            {
                current_shop += a_BttReference.name + ";";
                MenuController.settingsController.currentShop = current_shop;

                shopContainer.transform.Find(MenuController.settingsController.currentBoxID.ToString()).GetChild(0).gameObject.SetActive(false);
                MenuController.settingsController.currentBoxID = int.Parse(a_BttReference.name);

                UpdateUICoinsAmout();

                MenuController.settingsController.Coins -= GetComponent<MenuController>().shopCost;
                foreach (GameObject cointText in GetComponent<MenuController>().textCoin)
                {
                    cointText.GetComponent<TextMeshProUGUI>().text = MenuController.settingsController.Coins.ToString();
                }

                a_BttReference.transform.GetChild(1).GetComponent<Image>().color = Color.white;
                a_BttReference.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
                a_BttReference.transform.GetChild(1).GetComponent<Button>()?.onClick.AddListener(() => { GetComponent<UI.UIController>().OnShopItemChangeClick(a_BttReference); });
                a_BttReference.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);

                a_BttReference.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine("ShopCoinsError");
            }
        }

        /// <summary>
        /// Change and save option of materials
        /// </summary>
        /// <param name="a_ID"></param>
        public void OnShopItemChangeClick(GameObject a_BttReference)
        {
            if (MenuController.settingsController.currentBoxID != int.Parse(a_BttReference.name))
            {
                string current_shop = MenuController.settingsController.currentShop;
                string[] allMaterials = current_shop.Split(';');

                if (allMaterials.ToList().Contains(a_BttReference.name))
                {
                    shopContainer.transform.Find(MenuController.settingsController.currentBoxID.ToString()).GetChild(0).gameObject.SetActive(false);
                    MenuController.settingsController.currentBoxID = int.Parse(a_BttReference.name);
                    a_BttReference.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        #endregion SHOP

        #region GAME
        /// <summary>
        /// Restart current level
        /// </summary>
        public void OnRestartClick()
        {
            GameUILayoutInit();
            GetComponent<GameController>().InitLevel();
        }

        /// <summary>
        /// Start the level
        /// </summary>
        public void OnStartClick()
        {
            Time.timeScale = 1.0f;
            player.GetComponent<PlayerController>().PlayerStart();
        }

        /// <summary>
        /// Open settings panel
        /// </summary>
        public void OnSettingsClick()
        {
            if (!gameSettingsPanel.activeSelf)
            {
                gameSettingsPanel.SetActive(true);
                player.GetComponent<PlayerController>().PlayerPause();
                Time.timeScale = 0.0f;
            }
            else
            {
                gameSettingsPanel.SetActive(false);
                player.GetComponent<PlayerController>().PlayerStart();
                Time.timeScale = 1.0f;
            }
        }

        /// <summary>
        /// Go to next level
        /// </summary>
        public void OnNextClick()
        {
            Menu.MenuController.settingsController.currentLevel++;
            GameUILayoutInit();
            GetComponent<GameController>().InitLevel();
        }

        /// <summary>
        /// Go to menu
        /// </summary>
        public void OnMenuClick()
        {
            // Back to Menu
            GetComponent<MenuController>().LevelsInit();
            GetComponent<MenuController>().menuUIObject.SetActive(true);
            gameUIObject.SetActive(false);
            GetComponent<MenuController>().gameCamera.gameObject.SetActive(false);
            GetComponent<UIController>().uiCamera.clearFlags = CameraClearFlags.Skybox;

            UpdateUICoinsAmout();

            for (int i = 0; i < GetComponent<Menu.MenuController>().menuUIObject.transform.childCount; i++)
            {
                if (i <= 1)
                    GetComponent<MenuController>().menuUIObject.transform.GetChild(i).gameObject.SetActive(true);
                else
                    GetComponent<MenuController>().menuUIObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            StartBackgroundMusic();
        }
        #endregion GAME

        /// <summary>
        /// Close the game
        /// </summary>
        public void OnExitClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        #endregion on click buttons

        #region custom methods
        /// <summary>
        /// Set and play background music
        /// </summary>
        public void StartBackgroundMusic()
        {
            if (MenuController.settingsController.musicTrigger == (int)SettingsController.settingsTrigger.On)
            {
                AudioSource audio = GetComponent<AudioSource>();
                audio.clip = Resources.Load<AudioClip>("Audio/musicMenu") as AudioClip;
                audio.volume = 0.5f;
                audio.loop = true;
                audio.Play();
            }
        }

        public void UpdateUICoinsAmout()
        {
            foreach (GameObject cointText in GetComponent<MenuController>().textCoin)
            {
                cointText.GetComponent<TextMeshProUGUI>().text = MenuController.settingsController.Coins.ToString();
            }
        }

        /// <summary>
        /// Update settings painel buttons with saves
        /// </summary>
        private void SettingPanelInit()
        {
            Sprite soundOn = Resources.Load("UI/soundOn", typeof(Sprite)) as Sprite;
            Sprite soundOff = Resources.Load("UI/soundOff", typeof(Sprite)) as Sprite;

            if (MenuController.settingsController.soundTrigger == (int)SettingsController.settingsTrigger.On)
                settingsPanel.transform.Find("Sound").GetChild(0).GetComponent<Image>().sprite = soundOn;
            else
                settingsPanel.transform.Find("Sound").GetChild(0).GetComponent<Image>().sprite = soundOff;

            if (MenuController.settingsController.musicTrigger == (int)SettingsController.settingsTrigger.On)
                settingsPanel.transform.Find("Music").GetChild(0).GetComponent<Image>().sprite = soundOn;
            else
                settingsPanel.transform.Find("Music").GetChild(0).GetComponent<Image>().sprite = soundOff;

            if (MenuController.settingsController.vibrationTrigger == (int)SettingsController.settingsTrigger.On)
                settingsPanel.transform.Find("Vibration").GetChild(0).GetComponent<Image>().sprite = Resources.Load("UI/vibrationOn", typeof(Sprite)) as Sprite;
            else
                settingsPanel.transform.Find("Vibration").GetChild(0).GetComponent<Image>().sprite = Resources.Load("UI/vibrationOff", typeof(Sprite)) as Sprite;
        }

        public void GameUILayoutInit()
        {
            // UI start layout
            gamePlayBtt.SetActive(true);
            losePanel.SetActive(false);
            winPanel.SetActive(false);
            gameSettingsPanel.SetActive(false);
        }

    private IEnumerator ShopCoinsError()
        {
            Color color = Color.red;
            while (color.g < 1.0f && color.b < 1.0f)
            {
                color.g += Time.deltaTime;
                color.b += Time.deltaTime;
                shopCoinGO.transform.GetComponent<TextMeshProUGUI>().color = color;
                yield return null;
            }

            shopCoinGO.transform.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        #endregion custom methods
    }
}
