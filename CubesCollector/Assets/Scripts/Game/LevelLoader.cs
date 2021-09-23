// file=""LevelLoader.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 17/09/2021

#region usings
using Game.Design.Juntion;
using Game.Design.Level;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#endregion usings

namespace Game.Loader.Level
{
    public class LevelLoader : MonoBehaviour
    {
        #region vars
        public Vector3 currentPosition;
        public Vector3 currentForward;
        public int plataformsLoad;
        #endregion vars

        #region methods
        /// <summary>
        /// Set start values to load 1st Juntion
        /// </summary>
        public void setStartValues()
        {
            currentForward = Vector3.forward;
            currentPosition = Vector3.zero;
            plataformsLoad = 0;
        }

        /// <summary>
        /// Parse information on LevelDesign into gamescene
        /// </summary>
        /// <param name="a_level">level to load</param>
        /// <param name="a_BoxMaterial">material to use in win boxes</param>
        public void LoadLevel(int a_level, Material a_BoxMaterial)
        {
            setStartValues();

            Transform mapParent = GameObject.Find("Map").transform;

            LevelDesign level = Resources.Load<LevelDesign>("Levels/" + (a_level < 10 ? "0" : "") + a_level) as LevelDesign;
            
            foreach (Junction junction in level.junctions)
            {
                LoadAJunction(junction, mapParent, a_BoxMaterial);
            }

            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Finish"));
            go.transform.position = currentPosition + currentForward * 4.5f;
            go.transform.forward = currentForward;
            go.name = "Finish";
            go.transform.parent = mapParent;
        }

        /// <summary>
        /// Load Junction
        /// </summary>
        /// <param name="a_junction">junction top load</param>
        /// <param name="a_mapParent">Map transform reference</param>
        /// <param name="a_BoxMaterial">Material to add to boxes (in case junction have boxes)</param>
        public void LoadAJunction(Junction a_junction, Transform a_mapParent, Material a_BoxMaterial)
        {
            GameObject go = null;

            if (a_junction.JuntionType == LevelDesign.JunctionType.Straight)
                go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Straight"));
            else if (a_junction.JuntionType == LevelDesign.JunctionType.Left)
            {
                go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Left"));
                currentPosition += currentForward * 2.25f;
            }
            else if (a_junction.JuntionType == LevelDesign.JunctionType.Right)
            {
                go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Right"));
                currentPosition += currentForward * 2.25f;
            }

            go.transform.position = currentPosition;
            go.transform.forward = currentForward;
            go.name = "Junction_" + plataformsLoad;

            int x = -2;
            int z = 1;
            GameObject tempGO = null;
            for (int i = 0; i < 15; i++)
            {
                if (a_junction.BlockWinPosition != null && a_junction.BlockWinPosition.Length > 0 && a_junction.BlockWinPosition[i] > 0) // Boxes to pickup
                {
                    for (int j = 0; j < a_junction.BlockWinPosition[i]; j++)
                    {
                        tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Box"));
                        tempGO.name = "Box_" + i;
                        tempGO.transform.forward = currentForward;
                        SetPositionByCurrentForward(go.transform, tempGO.transform, x, z, 0.6f + j * 1);
                        tempGO.transform.parent = a_mapParent;
                        tempGO.GetComponent<Renderer>().material = a_BoxMaterial;
                    }
                }
                else if ((i >= 5 && i <= 9)
                    && a_junction.BlockLosePosition != null && a_junction.BlockLosePosition.Length > 0 && a_junction.BlockLosePosition[i - 5] > 0) // Boxes to lose
                {
                    for (int j = 0; j < a_junction.BlockLosePosition[i - 5]; j++)
                    {
                        tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/BoxLose"));
                        tempGO.name = "BoxLose_" + i;
                        tempGO.transform.forward = currentForward;
                        SetPositionByCurrentForward(go.transform, tempGO.transform, x, z, 0.6f + j * 1);
                        tempGO.transform.parent = a_mapParent;
                    }
                }
                else if (a_junction.CoinsPosition != null && a_junction.CoinsPosition.Length > 0 && a_junction.CoinsPosition[i]) // Coins
                {
                    tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Coin"));
                    tempGO.name = "Coin_" + i;
                    tempGO.transform.forward = currentForward;
                    tempGO.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                    SetPositionByCurrentForward(go.transform, tempGO.transform, x, z, 0.8f);
                    tempGO.transform.parent = a_mapParent;
                }

                x++;

                if ((i + 1) % 5 == 0)
                    z--;
                if (x >= 3)
                    x = -2;
            }

            go.transform.parent = a_mapParent;

            // rotate forward to new direction and add curves offset
            if (a_junction.JuntionType == LevelDesign.JunctionType.Left)
            {
                currentPosition += currentForward * 1.25f;
                currentForward = -(new Vector3(currentForward.z, 0, -currentForward.x));
                currentPosition += currentForward * 3.5f;
            }
            else if (a_junction.JuntionType == LevelDesign.JunctionType.Right)
            {
                currentPosition += currentForward * 1.25f;
                currentForward = new Vector3(currentForward.z, 0, -currentForward.x);
                currentPosition += currentForward * 3.5f;
            }

            currentPosition += currentForward * 3;
            plataformsLoad++;
        }

        /// <summary>
        /// Set postion by rotation of current Junction
        /// </summary>
        /// <param name="a_reference">Junction to be reference of positions</param>
        /// <param name="a_transform">Transform that will be set position</param>
        /// <param name="a_right">postion gap to right</param>
        /// <param name="a_forward">postion gap to forward</param>
        private void SetPositionByCurrentForward(Transform a_reference, Transform a_transform, int a_right, int a_forward, float height)
        {
            if (currentForward.z > 0)
                a_transform.position = new Vector3(a_reference.position.x + a_right, height, a_reference.position.z + a_forward);
            else if (currentForward.x < 0)
                a_transform.position = new Vector3(a_reference.position.x - a_forward, height, a_reference.position.z + a_right);
            else if (currentForward.x > 0)
                a_transform.position = new Vector3(a_reference.position.x + a_forward, height, a_reference.position.z - a_right);
            else
            {
                if (Application.isEditor)
                    DestroyImmediate(a_transform.gameObject);
                else
                    Destroy(a_transform.gameObject);
            }
        }

        /// <summary>
        /// Destroy all level Generated, JUST LEVEL MAKER
        /// </summary>
        public void DestroyCurrentLevel()
        {
            var tempList = GameObject.Find("Map").transform.Cast<Transform>().ToList();

            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    
        /// <summary>
        /// Remove last Junction, JUST LEVEL MAKER
        /// </summary>
        /// <param name="a_junctionType"></param>
        public void UndoJunction(LevelDesign.JunctionType a_junctionType)
        {
            currentPosition -= currentForward * 3;

            // rotate forward for new position and add offset off curves
            if (a_junctionType == LevelDesign.JunctionType.Left)
            {
                currentPosition -= currentForward * 3.5f;
                currentForward = (new Vector3(currentForward.z, 0, -currentForward.x));
                currentPosition -= currentForward * 1.25f;
                currentPosition -= currentForward * 2.25f;
            }
            else if (a_junctionType == LevelDesign.JunctionType.Right)
            {
                currentPosition -= currentForward * 3.5f;
                currentForward = -(new Vector3(currentForward.z, 0, -currentForward.x));
                currentPosition -= currentForward * 1.25f;
                currentPosition -= currentForward * 2.25f;
            }

            Transform map = GameObject.Find("Map").transform;

            DestroyImmediate(map.GetChild(map.childCount - 1).gameObject);

            while (!map.GetChild(map.childCount - 1).name.StartsWith("Junction_"))
            {
                DestroyImmediate(map.GetChild(map.childCount - 1).gameObject);
            }

            plataformsLoad--;
        }
        #endregion methods
    }
}
