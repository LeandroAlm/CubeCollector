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
        private Vector3 currentPosition;
        private Vector3 currentFoward;
        private int plataformsLoad;

        public void setStartValues()
        {
            currentFoward = Vector3.forward;
            currentPosition = Vector3.zero;
            plataformsLoad = 0;
        }

        /// <summary>
        /// Parse information on LevelDesign into gamescene
        /// </summary>
        /// <param name="a_level">level to load</param>
        /// <param name="a_BoxMaterial">material to use in win boxes</param>
        public void loadLevel(int a_level, Material a_BoxMaterial)
        {
            setStartValues();

            Transform mapParent = GameObject.Find("Map").transform;

            LevelDesign level = Resources.Load<LevelDesign>("Levels/" + (a_level < 10 ? "0" : "") + a_level) as LevelDesign;
            
            foreach (Junction junction in level.junctions)
            {
                LoadAJunction(junction, mapParent, a_BoxMaterial);
            }

            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Finish"));
            go.transform.position = currentPosition + currentFoward * 4.5f;
            go.transform.forward = currentFoward;
            go.name = "Finish";
            go.transform.parent = mapParent;
        }

        public void LoadAJunction(Junction a_junction, Transform a_mapParent, Material a_BoxMaterial)
        {
            GameObject go = null;

            if (a_junction.JuntionType == LevelDesign.JunctionType.Straight)
                go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Straight"));
            else if (a_junction.JuntionType == LevelDesign.JunctionType.Left)
            {
                go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Left"));
                currentPosition += currentFoward * 2.25f;
            }
            else if (a_junction.JuntionType == LevelDesign.JunctionType.Right)
            {
                go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Right"));
                currentPosition += currentFoward * 2.25f;
            }

            go.transform.position = currentPosition;
            go.transform.forward = currentFoward;
            go.name = "Junction_" + plataformsLoad;

            int x = -2;
            int z = 1;
            GameObject tempGO = null;
            for (int i = 0; i < 15; i++)
            {
                if (a_junction.BlockWinPosition != null && a_junction.BlockWinPosition.Length > 0 && a_junction.BlockWinPosition[i] > 0)
                {
                    for (int j = 0; j < a_junction.BlockWinPosition[i]; j++)
                    {
                        tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Box"));

                        SetPositionByCurrentFoward(go.transform, tempGO.transform, x, z, 0.6f + j * 1);
                        //tempGO.transform.position = new Vector3(go.transform.position.x + x, 0.6f + j * 1, go.transform.position.z + z);
                        tempGO.name = "Box_" + i;
                        tempGO.transform.parent = a_mapParent;
                        tempGO.GetComponent<Renderer>().material = a_BoxMaterial;
                    }
                }
                else if ((i >= 5 && i <= 9)
                    && a_junction.BlockLosePosition != null && a_junction.BlockLosePosition.Length > 0 && a_junction.BlockLosePosition[i - 5] > 0)
                {
                    for (int j = 0; j < a_junction.BlockLosePosition[i - 5]; j++)
                    {
                        tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/BoxLose"));
                        SetPositionByCurrentFoward(go.transform, tempGO.transform, x, z, 0.6f + j * 1);
                        //tempGO.transform.position = new Vector3(go.transform.position.x + x, 0.6f + j * 1, go.transform.position.z + z);
                        tempGO.name = "BoxLose_" + i;
                        tempGO.transform.parent = a_mapParent;
                    }
                }
                else if (a_junction.CoinsPosition != null && a_junction.CoinsPosition.Length > 0 && a_junction.CoinsPosition[i])
                {
                    tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Coin"));
                    SetPositionByCurrentFoward(go.transform, tempGO.transform, x, z, 0.8f);
                    //tempGO.transform.position = new Vector3(go.transform.position.x + x, 0.8f, go.transform.position.z + z);
                    tempGO.name = "Coin_" + i;
                    tempGO.transform.parent = a_mapParent;
                }

                x++;

                if ((i + 1) % 5 == 0)
                    z--;
                if (x >= 3)
                    x = -2;
            }

            go.transform.parent = a_mapParent;

            // rotate foward for new position and add offset off curves
            if (a_junction.JuntionType == LevelDesign.JunctionType.Left)
            {
                currentPosition += currentFoward * 1.25f;
                currentFoward = -(new Vector3(currentFoward.z, 0, -currentFoward.x));
                currentPosition += currentFoward * 3.5f;
            }
            else if (a_junction.JuntionType == LevelDesign.JunctionType.Right)
            {
                currentPosition += currentFoward * 1.25f;
                currentFoward = new Vector3(currentFoward.z, 0, -currentFoward.x);
                currentPosition += currentFoward * 3.5f;
            }

            currentPosition += currentFoward * 3;
            plataformsLoad++;
        }

        /// <summary>
        /// Set postion by rotation of current Junction
        /// </summary>
        /// <param name="a_reference">Junction to be reference of positions</param>
        /// <param name="a_transform">Transform that will be set position</param>
        /// <param name="a_right">postion gap to right</param>
        /// <param name="a_foward">postion gap to foward</param>
        private void SetPositionByCurrentFoward(Transform a_reference, Transform a_transform, int a_right, int a_foward, float height)
        {
            if (currentFoward.z > 0)
                a_transform.position = new Vector3(a_reference.position.x + a_right, height, a_reference.position.z + a_foward);
            else if (currentFoward.x < 0)
                a_transform.position = new Vector3(a_reference.position.x - a_foward, height, a_reference.position.z + a_right);
            else if (currentFoward.x > 0)
                a_transform.position = new Vector3(a_reference.position.x + a_foward, height, a_reference.position.z - a_right);
            else
            {
                if (Application.isEditor)
                    DestroyImmediate(a_transform.gameObject);
                else
                    Destroy(a_transform.gameObject);
            }
        }

        public void DestroyCurrentLevel()
        {
            var tempList = GameObject.Find("Map").transform.Cast<Transform>().ToList();

            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    
        public void UndoJunction(LevelDesign.JunctionType a_junctionType)
        {
            // rotate foward for new position and add offset off curves
            if (a_junctionType == LevelDesign.JunctionType.Left)
            {
                currentPosition -= currentFoward * 1.25f;
                currentFoward = (new Vector3(currentFoward.z, 0, -currentFoward.x));
                currentPosition -= currentFoward * 3.5f;
            }
            else if (a_junctionType == LevelDesign.JunctionType.Right)
            {
                currentPosition -= currentFoward * 1.25f;
                currentFoward = -(new Vector3(currentFoward.z, 0, -currentFoward.x));
                currentPosition -= currentFoward * 3.5f;
            }

            Transform map = GameObject.Find("Map").transform;

            DestroyImmediate(map.GetChild(map.childCount - 1).gameObject);

            while (!map.GetChild(map.childCount - 1).name.StartsWith("Junction_"))
            {
                DestroyImmediate(map.GetChild(map.childCount - 1).gameObject);
            }

            currentPosition -= currentFoward * 3;
            plataformsLoad--;
        }
    }

}
