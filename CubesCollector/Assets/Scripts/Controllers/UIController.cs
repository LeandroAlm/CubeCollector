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
        [Tooltip("Reference of Menu UI object")]
        private GameObject menuUIObject;
        [SerializeField]
        [Tooltip("Reference of Game UI object")]
        public GameObject gameUIObject;
        [SerializeField]
        [Tooltip("Reference of shop coins object")]
        private GameObject shopCoinGO;
        [SerializeField]
        [Tooltip("Reference of shop glow object")]
        private GameObject shopGlowGO;
        [SerializeField]
        [Tooltip("Reference of Menu object")]
        private Camera gameCamera;
        [SerializeField]
        [Tooltip("Reference of Menu object")]
        public Camera uiCamera;
        [SerializeField]
        [Tooltip("Reference of sound button gameobject")]
        private GameObject soundGO;
        [SerializeField]
        [Tooltip("Reference of music button gameobject")]
        private GameObject musicGO;
        [SerializeField]
        [Tooltip("Reference of vibration button gameobject")]
        private GameObject vibraGO;
        [SerializeField]
        [Tooltip("Reference of player gameobject")]
        private GameObject player;
        #endregion variables

        #region base methods
        private void Awake()
        {
            StartBackgroundMusic();
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
                MenuController.settingsController.currentBoxID = int.Parse(a_BttReference.name);

                UpdateUICoinsAmout();

                MenuController.settingsController.Coins -= GetComponent<MenuController>().shopCost;
                foreach (GameObject cointText in GetComponent<MenuController>().textCoin)
                {
                    cointText.GetComponent<TextMeshProUGUI>().text = MenuController.settingsController.Coins.ToString();
                }

                a_BttReference.GetComponent<Image>().color = Color.white;
                a_BttReference.GetComponent<Button>().onClick.RemoveAllListeners();
                a_BttReference.GetComponent<Button>()?.onClick.AddListener(() => { GetComponent<UI.UIController>().OnShopItemChangeClick(a_BttReference); });
                a_BttReference.transform.GetChild(0).gameObject.SetActive(false);

                shopGlowGO.GetComponent<RectTransform>().anchoredPosition = a_BttReference.GetComponent<RectTransform>().anchoredPosition;
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
                    MenuController.settingsController.currentBoxID = int.Parse(a_BttReference.name);
                    GetComponent<MenuController>().shopContainerGO.transform.Find("Glow").GetComponent<RectTransform>().anchoredPosition = a_BttReference.GetComponent<RectTransform>().anchoredPosition;
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
            GetComponent<GameController>().InitLevel();
        }

        /// <summary>
        /// Start the level
        /// </summary>
        public void OnStartClick()
        {
            player.GetComponent<PlayerController>().PlayerStart();
        }

        /// <summary>
        /// Open settings panel
        /// </summary>
        public void OnSettingsClick()
        {
            if (!GetComponent<GameController>().settingsPanel.activeSelf)
            {
                GetComponent<GameController>().settingsPanel.SetActive(true);
                player.GetComponent<PlayerController>().PlayerPause();
            }
            else
            {
                GetComponent<GameController>().settingsPanel.SetActive(false);
                player.GetComponent<PlayerController>().PlayerStart();
            }
        }

        /// <summary>
        /// Go to next level
        /// </summary>
        public void OnNextClick()
        {
            Menu.MenuController.settingsController.currentLevel++;
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
            GetComponent<UIController>().gameUIObject.SetActive(false);
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
