// file=""MenuController.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 17/09/2021

#region usings
using Game.Controller.Settings;
using Game.Design.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion usings

namespace Game.Controller.Menu
{
    public class MenuController : MonoBehaviour
    {
        #region variables
        [SerializeField]
        [Tooltip("Reference of Menu object")]
        public GameObject menuUIObject;
        [SerializeField]
        [Tooltip("Reference of Menu object")]
        public Camera gameCamera;
        [SerializeField]
        [Tooltip("Reference of coin text gameobject")]
        public GameObject[] textCoin;
        [SerializeField]
        [Tooltip("Reference of levels container gameobject")]
        private GameObject levelContainerGO;
        [SerializeField]
        [Tooltip("Reference of shop container gameobject")]
        public GameObject shopContainerGO;
        [SerializeField]
        [Tooltip("Reference of settings panel")]
        private GameObject settingsPanel;
        [SerializeField]
        [Range(1, 1000)]
        [Tooltip("Cost of new materials")]
        public int shopCost;
        #endregion variables

        #region internal variables
        private static SettingsController _settingsController;
        public static SettingsController settingsController
        {
            get 
            { 
                if (_settingsController == null)
                    SettingsInit();

                return _settingsController;
            }
            set { _settingsController = new SettingsController(); }
        }
        #endregion internal variables

        #region base methods
        private void Awake()
        {
            gameCamera.gameObject.SetActive(false);
            SettingsInit();
            SettingPanelInit();
            LevelsInit();
            ShopLoad();
            foreach (GameObject cointText in textCoin)
            {
                cointText.GetComponent<TextMeshProUGUI>().text = settingsController.Coins.ToString();
            }

            for (int i = 0; i < menuUIObject.transform.childCount; i++)
            {
                if (i <= 1)
                    menuUIObject.transform.GetChild(i).gameObject.SetActive(true);
                else
                    menuUIObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        #endregion base methods

        #region custom methods
        #region SETTINGS
        private static void SettingsInit()
        {
            settingsController = new SettingsController();
            settingsController.InitSettings();
        }

        private void SettingPanelInit()
        {
            Sprite soundOn = Resources.Load("UI/soundOn", typeof(Sprite)) as Sprite;
            Sprite soundOff = Resources.Load("UI/soundOff", typeof(Sprite)) as Sprite;

            if (settingsController.soundTrigger == (int)SettingsController.settingsTrigger.On)
                settingsPanel.transform.Find("Sound").GetChild(0).GetComponent<Image>().sprite = soundOn;
            else
                settingsPanel.transform.Find("Sound").GetChild(0).GetComponent<Image>().sprite = soundOff;

            if (settingsController.musicTrigger == (int)SettingsController.settingsTrigger.On)
                settingsPanel.transform.Find("Music").GetChild(0).GetComponent<Image>().sprite = soundOn;
            else
                settingsPanel.transform.Find("Music").GetChild(0).GetComponent<Image>().sprite = soundOff;

            if (settingsController.vibrationTrigger == (int)SettingsController.settingsTrigger.On)
                settingsPanel.transform.Find("Vibration").GetChild(0).GetComponent<Image>().sprite = Resources.Load("UI/vibrationOn", typeof(Sprite)) as Sprite;
            else
                settingsPanel.transform.Find("Vibration").GetChild(0).GetComponent<Image>().sprite = Resources.Load("UI/vibrationOff", typeof(Sprite)) as Sprite;
        }
        #endregion SETTINGS

        #region LEVELS
        /// <summary>
        /// Loads all buttons for possible levels
        /// </summary>
        public void LevelsInit()
        {
            int x = 1;
            int current_Y = -100;
            var allLevels = Resources.LoadAll("Levels/", typeof(LevelDesign));

            GameObject LevelBtt = Resources.Load<GameObject>("Prefabs/UI/LevelBtt");
            GameObject LevelBttBlock = Resources.Load<GameObject>("Prefabs/UI/LevelBttBlocked");

            if (levelContainerGO.transform.childCount > 0)
                foreach (Transform child in levelContainerGO.transform)
                    Destroy(child.gameObject);

            for (int i = 1; i < allLevels.Length+1; i++)
            {
                GameObject levelBtt = null;

                if (i <= settingsController.maxLevel)
                    levelBtt = Instantiate(LevelBtt, levelContainerGO.transform) as GameObject;
                else
                    levelBtt = Instantiate(LevelBttBlock, levelContainerGO.transform) as GameObject;

                levelBtt.GetComponent<RectTransform>().anchoredPosition = new Vector2(125 + 210 * (x - 1), current_Y);
                levelBtt.name = "Btt_" + (i < 10 ? "0" : "") + i;
                levelBtt.transform.GetComponentInChildren<TextMeshProUGUI>().text = (i < 10 ? "0" : "") + i;

                // Need to create an int to fix i value, solved in stackOverflow
                int i_value = i;
                levelBtt.GetComponent<Button>()?.onClick.AddListener(delegate { GetComponent<UI.UIController>().OnLoadLevelClick(i_value); });

                x++;
                if (x >= 4)
                {
                    x = 1;
                    current_Y -= 210;
                }
            }
        }
        #endregion LEVELS

        #region SHOP
        /// <summary>
        /// Loads all material for possible acquisition
        /// </summary>
        public void ShopLoad()
        {
            int x = 1;
            int current_Y = -100;

            string current_shop = settingsController.currentShop;
            string[] allIDNames = current_shop.Split(';');
            List<int> allIDs = new List<int>();
            GameObject shopBtt = Resources.Load<GameObject>("Prefabs/UI/ShopBtt");
            var allTextures = Resources.LoadAll("UI/Box", typeof(Sprite));

            for (int i = 0; i < allIDNames.Length; i++)
                if (!string.IsNullOrEmpty(allIDNames[i]))
                    allIDs.Add(int.Parse(allIDNames[i]));

            RectTransform tempGO = null;
            for (int i = 0; i < allTextures.Length; i++)
            {
                tempGO = Instantiate(shopBtt, shopContainerGO.transform).GetComponent<RectTransform>();
                tempGO.anchoredPosition = new Vector2(162 + 238 * (x - 1), current_Y);
                tempGO.GetComponent<Image>().sprite = (Sprite)allTextures[i];
                tempGO.name = i.ToString();

                // Need to create an int to fix i value, solved in stackOverflow
                GameObject spcialGO = tempGO.gameObject;
                if (!allIDs.Contains(i))
                {
                    tempGO.transform.GetChild(0).gameObject.SetActive(true);
                    tempGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = shopCost.ToString();


                    tempGO.GetComponent<Image>().color = Color.grey;

                    int i_value = i;
                    tempGO.GetComponent<Button>()?.onClick.AddListener(() => { GetComponent<UI.UIController>().OnShopItemBuyClick(spcialGO); });
                }
                else
                    tempGO.GetComponent<Button>()?.onClick.AddListener(() => { GetComponent<UI.UIController>().OnShopItemChangeClick(spcialGO); });

                if (settingsController.currentBoxID == i)
                    shopContainerGO.transform.Find("Glow").GetComponent<RectTransform>().anchoredPosition = tempGO.anchoredPosition;

                x++;
                if (x >= 4)
                {
                    x = 1;
                    current_Y -= 225;
                }
            }
        }
        #endregion SHOP
        #endregion custom methods
    }
}
