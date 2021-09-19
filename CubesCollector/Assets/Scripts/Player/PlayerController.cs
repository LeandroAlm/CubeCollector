// file=""PlayerController.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 19/09/2021

#region usings
using Game.Controller.Game;
using Game.Design.Juntion;
using TMPro;
using UnityEngine;
#endregion usings

namespace Game.Controller.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region vars
        [SerializeField]
        [Tooltip("Coin amout gameobject reference")]
        private GameObject CoinsTextGO;
        [SerializeField]
        [Tooltip("Speed multiplier")]
        private float speed = 1.0f;
        [SerializeField]
        [Tooltip("Speed multiplier for side move")]
        private float speed_side = 1.0f;
        #endregion vars

        #region internal vars
        private enum playerStatus
        {
            Play,
            Pause,
            Finish,
        }

        private playerStatus currentStatus = playerStatus.Pause;
        private int coinsCollect = 0;

        private int currentJunctionID = -1;
        private GameObject currentJuntion;
        private int currentBoxesCollected;
        #endregion internal vars

        #region methods
        private void Awake()
        {
            coinsCollect = 0;
            CoinsTextGO.GetComponent<TextMeshProUGUI>().text = coinsCollect.ToString();
            currentBoxesCollected = 0;
        }

        private void Update()
        {
            if (currentStatus == playerStatus.Play)
            {
                transform.position += transform.forward * speed;

                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        transform.position += (touch.deltaPosition.x > 0 ? 1 : -1) * transform.right * speed_side;

                        if (transform.position.x < (currentJuntion.transform.position.x - 2))
                            transform.position = new Vector3((currentJuntion.transform.position.x - 2), transform.position.y, transform.position.z);
                        if (transform.position.x > (currentJuntion.transform.position.x + 2))
                            transform.position = new Vector3((currentJuntion.transform.position.x + 2), transform.position.y, transform.position.z);
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("BoxLose"))
            {
                Transform t1 = transform.GetChild(0);
                Transform t2 = t1.GetChild(t1.childCount - 1);
                t2.tag = "Untagged";
                t2.parent = GameObject.Find("Boxes").transform;
                collision.gameObject.tag = "Untagged";
            }
            else if (collision.gameObject.CompareTag("BoxCollect"))
            {
                foreach (Transform child in transform.GetChild(0))
                {
                    child.position += Vector3.up;
                }
                collision.transform.SetParent(transform.GetChild(0));
                collision.transform.localPosition = new Vector3(0, 0.6f, 0);
                currentBoxesCollected++;
            }
            else if (collision.gameObject.CompareTag("Coin"))
            {
                coinsCollect++;
                CoinsTextGO.GetComponent<TextMeshProUGUI>().text = coinsCollect.ToString();
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("Map"))
            {
                int ID = int.Parse(collision.gameObject.name) - 1;
                if (currentJunctionID != ID) // Detect new Junction
                {
                    currentJuntion = collision.gameObject;
                    currentJunctionID = ID;
                }
            }
            else if (collision.gameObject.CompareTag("Finish"))
            {
                Invoke("playerFinish", 0.25f);
            }
        }
        #endregion methods

        #region custom methods
        public void playerMove()
        {
            currentStatus = playerStatus.Play;
        }
        
        public void playerPause()
        {
            currentStatus = playerStatus.Pause;
        }

        public void playerFinish()
        {
            int coinsSaved = 0;

            if (PlayerPrefs.GetInt("COINS") > 0)
                coinsSaved += PlayerPrefs.GetInt("COINS");

            PlayerPrefs.SetInt("COINS", coinsSaved + coinsCollect);
            currentStatus = playerStatus.Finish;
            // Animate end level
        }
        #endregion custom methods
    }
}

