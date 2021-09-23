// file=""PlayerController.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 19/09/2021

#region usings
using Game.Controller.Game;
using Game.Controller.Menu;
using Game.Design.Juntion;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion usings

namespace Game.Controller.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region vars
        [SerializeField]
        [Tooltip("Gamecontroller object reference")]
        private GameObject gameControllerGO;
        [SerializeField]
        [Tooltip("Game coin amout object reference")]
        private GameObject coinsTextGO;
        [SerializeField]
        [Tooltip("Extra coins object reference")]
        private GameObject extraGO;
        [SerializeField]
        [Tooltip("Map object reference")]
        private GameObject mapGO;
        [SerializeField]
        [Tooltip("Forward speed")]
        private float speed = 1.0f;
        [SerializeField]
        [Tooltip("Side speed")]
        private float speed_side = 1.0f;
        [SerializeField]
        [Tooltip("Acceleration multiplier for side move, when drag direction change Acceleration goes 0.0f - 1.0f with this multiplier. Created to smooth direction change")]
        private float accelerationMultiplier;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        [Tooltip("Collider offeet, when box collide with red box they should have same Y to block, this offset is used to create a gap")]
        private float colliderOffset;
        [SerializeField]
        [Tooltip("Extra coins earn for each extra box")]
        private int extraCoin;
        [SerializeField]
        [Tooltip("Player initial possition")]
        private Vector3 initialPosition;
        [SerializeField]
        [Tooltip("Box 0 initial possition")]
        private Vector3 boxInitialPosition;
        [SerializeField]
        [Tooltip("Guy initial possition")]
        private Vector3 guyInitialPosition;
        #endregion vars

        #region internal vars
        private enum playerStatus
        {
            Play,
            Pause,
            Finish,
        }
        private playerStatus currentStatus = playerStatus.Pause;

        /// <summary>
        /// Coins collected in current level
        /// </summary>
        private int coinsCollect = 0;
        /// <summary>
        /// Current straight Junction ID
        /// </summary>
        private int currentJunctionID = -1;
        /// <summary>
        /// Current straight Junction
        /// </summary>
        private GameObject currentJuntion;
        /// <summary>
        /// Referece to a curve "Rotation Point" Left
        /// </summary>
        private Transform currentCurveLeft;
        /// <summary>
        /// Referece to a curve "Rotation Point" Right
        /// </summary>
        private Transform currentCurveRight;
        /// <summary>
        /// Uses to update and limit max side position (keeping player inside of platform)
        /// </summary>
        private float currentLimitControl;
        /// <summary>
        /// flag if is falling, use to move down quicker
        /// </summary>
        private bool falling;
        /// <summary>
        /// Player accelaration when move for sides
        /// </summary>
        private float acceleration;
        /// <summary>
        /// Flag if touch is drag to right
        /// </summary>
        private bool isMovingRigth;
        /// <summary>
        /// Current direction of Player
        /// </summary>
        private Vector3 direction;
        /// <summary>
        /// reference to child "Boxes"
        /// </summary>
        private Transform boxesContainer;
        /// <summary>
        /// reference to child "Guy"
        /// </summary>
        private Transform guyTransform;
        #endregion internal vars

        #region base methods
        private void Awake()
        {
            boxesContainer = transform.GetChild(0);
            guyTransform = transform.GetChild(1);

            extraGO.GetComponent<TextMeshProUGUI>().text = "+" + extraCoin;
        }

        private void Update()
        {
            if (currentStatus == playerStatus.Play)
            {
                // Go forward
                transform.position += transform.forward * speed * Time.deltaTime;

                // Go sides
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        // Accelarate, reset on direction change, no easefunction, just Move_Linear to the 1.0f
                        if (touch.deltaPosition.x > 0 && !isMovingRigth)
                        {
                            isMovingRigth = true;
                            acceleration = 0.0f;
                        }
                        else if (touch.deltaPosition.x < 0 && isMovingRigth)
                        {
                            isMovingRigth = false;
                            acceleration = 0.0f;
                        }

                        if (acceleration < 1)
                            acceleration += Time.deltaTime * accelerationMultiplier;
                        else
                            acceleration = 1.0f;

                        //if (currentLimitControl + (isMovingRigth ? 1 : -1) * speed_side * Time.deltaTime * acceleration <= 2
                        //    && currentLimitControl + (isMovingRigth ? 1 : -1) * speed_side * Time.deltaTime * acceleration >= -2)
                        if (currentLimitControl >= -2 && currentLimitControl <= 2)
                        {
                            float move = (isMovingRigth ? 1 : -1) * speed_side * Time.deltaTime * acceleration;
                            float diff = move;

                            // I case o limite reach
                            if (currentLimitControl + move > 2)
                            {
                                diff = 2 - currentLimitControl;
                                transform.position += transform.right * diff;
                                currentLimitControl = 2;
                            }
                            else if(currentLimitControl + move < -2)
                            {
                                diff = -2 - currentLimitControl;
                                transform.position += transform.right * diff;
                                currentLimitControl = -2;
                            }
                            else
                            {
                                transform.position += transform.right * diff;
                                currentLimitControl += diff;
                            }

                        }
                    }
                }
                else
                    acceleration = 0.0f;

                // Force fall, when lose box forces go down quicker
                if (falling)
                    transform.position = new Vector3(transform.position.x, transform.position.y - (speed * Time.deltaTime), transform.position.z);

                // Fix rotation, this code was create to force rotate in curves
                if (currentCurveLeft != null) // is left
                {
                    transform.right = (transform.position - currentCurveLeft.position);
                    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                }
                else if (currentCurveRight != null) // is right
                {
                    transform.right = (currentCurveRight.position - transform.position);
                    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                }
                else if (transform.rotation.eulerAngles.y != currentJuntion.transform.rotation.eulerAngles.y)// force current junction forward
                    transform.forward = direction;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("BoxLose") && !falling)
            {
                Box.BoxControlller boxController = collision.gameObject.GetComponent<Box.BoxControlller>();
                if (boxesContainer.childCount > 0 && !boxController.firstCollide)
                {
                    bool boxRemmoved = false;
                    foreach (Transform child in boxesContainer)
                    {
                        if (collision.transform.position.y >= child.position.y - colliderOffset
                            && collision.transform.position.y <= child.position.y + colliderOffset)
                        {
                            child.tag = "Untagged";
                            child.parent = mapGO.transform;
                            collision.gameObject.tag = "Untagged";
                            boxRemmoved = true;
                            break;
                        }
                    }

                    if (!boxRemmoved)
                        return;

                    boxController.firstCollide = true;
                    if (boxesContainer.childCount <= 0)
                    {
                        // Lose level
                        Destroy(GetComponent<Rigidbody>());
                        guyTransform.GetComponent<Animator>().SetBool("isFalling", true);
                        //guyTransform.rotation = Quaternion.Euler(0, 0, 0);
                        guyTransform.Rotate(Vector3.up * -50);

                        gameControllerGO.GetComponent<GameController>().DisableSettingsButton();
                        Invoke("PlayerLose", 0.75f);
                    }
                    else
                    {
                        // Fix remaining boxes positions
                        for (int i = 0; i < boxesContainer.childCount; i++)
                        {
                            boxesContainer.GetChild(i).localPosition = boxInitialPosition + (Vector3.up * i);
                        }
                        guyTransform.localPosition = guyInitialPosition + (Vector3.up * (boxesContainer.childCount-1));
                        transform.position += Vector3.up;
                    }

                    if (MenuController.settingsController.vibrationTrigger == (int)Settings.SettingsController.settingsTrigger.On)
                        VibrationController.Vibrate(250);

                    PlayAudioByName("boxLose", 0.75f);
                }
            }
            else if (collision.gameObject.CompareTag("BoxCollect"))
            {
                collision.transform.SetParent(boxesContainer);
                collision.transform.localPosition = boxInitialPosition + Vector3.up * (boxesContainer.childCount-1);
                guyTransform.localPosition = guyInitialPosition + Vector3.up * (boxesContainer.childCount-1);
                collision.gameObject.tag = "Untagged";
                PlayAudioByName("boxWin", 1f);
            }
            else if (collision.gameObject.CompareTag("Coin"))
            {
                coinsCollect++;
                coinsTextGO.GetComponent<TextMeshProUGUI>().text = (MenuController.settingsController.Coins + coinsCollect).ToString();
                Destroy(collision.gameObject);

                PlayAudioByName("coin", 0.75f);
            }
            else if (collision.gameObject.CompareTag("MapLeft"))
            {
                currentCurveLeft = FetchJunction(collision);
            }
            else if (collision.gameObject.CompareTag("MapRight"))
            {
                currentCurveRight = FetchJunction(collision);
            }
            else if (collision.gameObject.CompareTag("Map"))
            {
                FetchJunction(collision);
            }
            else if (collision.gameObject.CompareTag("Finish"))
            {
                gameControllerGO.GetComponent<GameController>().DisableSettingsButton();
                Invoke("PlayerFinish", 0.1f);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("BoxLose"))
            {
                falling = true;
            }
        }
        #endregion base methods

        #region custom methods
        public void PlayerInit(Material a_material)
        {
            coinsCollect = 0;
            currentLimitControl = 0;
            direction = Vector3.forward;
            falling = false;

            transform.position = initialPosition;
            transform.rotation = Quaternion.Euler(Vector3.zero);

            if (boxesContainer.childCount > 1)
            {
                for (int i = 1; i < boxesContainer.childCount; i++)
                    Destroy(boxesContainer.GetChild(i).gameObject);
            }
            else if (boxesContainer.childCount <= 0) // In case lose and restart
            {
                GameObject tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Box"), boxesContainer);
                tempGO.GetComponent<Renderer>().material = a_material;
            }
            else
                boxesContainer.GetChild(0).GetComponent<Renderer>().material = a_material;

            boxesContainer.GetChild(0).localPosition = boxInitialPosition;
            guyTransform.rotation = Quaternion.Euler(0, 50, 0);
            guyTransform.localPosition = guyInitialPosition;
            guyTransform.GetComponent<Animator>().SetBool("isFalling", false);
            guyTransform.GetComponent<Animator>().SetBool("isDancing", false);

            coinsTextGO.GetComponent<TextMeshProUGUI>().text = MenuController.settingsController.Coins.ToString();
            currentJuntion = mapGO.transform.GetChild(0).gameObject;

            if (!GetComponent<Rigidbody>())
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.freezeRotation = true;
            }
        }
        
        public void PlayerStart()
        {
            currentStatus = playerStatus.Play;
        }
        
        public void PlayerPause()
        {
            currentStatus = playerStatus.Pause;
        }

        private void PlayerLose()
        {
            currentStatus = playerStatus.Finish;
            gameControllerGO.GetComponent<GameController>().LoseLevel();
        }

        public void PlayerFinish()
        {
            currentStatus = playerStatus.Finish;

            // Animate end level
            StartCoroutine("AnimateWinBox");
        }
        
        private void PlayAudioByName(string a_name, float a_volume)
        {
            if (MenuController.settingsController.soundTrigger == (int)Settings.SettingsController.settingsTrigger.Off)
                return;

            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = Resources.Load<AudioClip>("Audio/" + a_name) as AudioClip;
            audio.volume = a_volume;
            audio.Play();
        }
        
        private Transform FetchJunction(Collision a_collision)
        {
            if (currentCurveLeft != null)
            {
                // Update direction
                currentCurveLeft.tag = "Untagged";
                direction = -(new Vector3(direction.z, 0, -direction.x));
            }
            else if (currentCurveRight != null)
            {
                // Update direction
                direction = new Vector3(direction.z, 0, -direction.x);
                currentCurveRight.tag = "Untagged";
            }

            GameObject tempGO = a_collision.gameObject;

            if (a_collision.gameObject.name == "Curve")
                tempGO = a_collision.transform.parent.gameObject;
            else if (currentCurveLeft != null || currentCurveRight != null)
            {
                // Fix gap -2 to 2
                if (direction == Vector3.forward)
                    currentLimitControl = transform.position.x - tempGO.transform.position.x;
                else if (direction == Vector3.right)
                    currentLimitControl = tempGO.transform.position.z - transform.position.z;
                else if (direction == Vector3.left)
                    currentLimitControl = transform.position.z - tempGO.transform.position.z;

                float diff = 0;
                if (currentLimitControl < -2)
                {
                    diff = currentLimitControl + 2;

                    currentLimitControl += diff;
                    transform.position += transform.right * diff;
                }
                else
                if (currentLimitControl > 2)
                {
                    diff = currentLimitControl - 2;

                    currentLimitControl -= diff;
                    transform.position -= transform.right * diff;
                }
            }

            int ID = int.Parse(tempGO.name.Replace("Junction_", "")) - 1;
            if (currentJunctionID != ID) // Detect new Junction
            {
                currentJuntion = tempGO;
                currentJunctionID = ID;
            }

            currentCurveLeft = null;
            currentCurveRight = null;
            falling = false;

            if (tempGO.transform.childCount > 0) // Is a curve
                return tempGO.transform.GetChild(0);
            else
                return null;
        }

        private IEnumerator AnimateWinBox()
        {
            int initialBoxes = boxesContainer.childCount;

            guyTransform.GetComponent<Animator>().SetBool("isDancing", true);
            guyTransform.Rotate(Vector3.up * -50);
            MenuController.settingsController.Coins += coinsCollect;

            yield return new WaitForSeconds(0.5f);

            if (initialBoxes > 1)
            {
                extraGO.SetActive(true);
                for (int i = 1; i < initialBoxes; i++)
                {
                    guyTransform.transform.localPosition += Vector3.down;
                    Destroy(boxesContainer.GetChild(initialBoxes - i).gameObject);

                    MenuController.settingsController.Coins += extraCoin;
                    PlayAudioByName("coin", 0.5f);
                    coinsTextGO.GetComponent<TextMeshProUGUI>().text = MenuController.settingsController.Coins.ToString();

                    float angle = 0;
                    while (angle < 180)
                    {
                        float extraSize = (Mathf.Sin(angle * Mathf.Deg2Rad) * 30);
                        extraGO.GetComponent<TextMeshProUGUI>().fontSize = 70 + extraSize;

                        angle += speed/2;

                        yield return new WaitForEndOfFrame();
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);

            extraGO.SetActive(false);
            gameControllerGO.GetComponent<GameController>().FinishLevel();
        }
        #endregion custom methods
    }
}

