// file=""LevelLoader.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 17/09/2021


#region usings
using Game.Design.Juntion;
using Game.Design.Level;
using System.Collections.Generic;
using UnityEngine;
#endregion usings

namespace Game.Loader.Level
{
    public class LevelLoader : MonoBehaviour
    {
        public LevelDesign loadLevel(int a_level)
        {
            LevelDesign level = (LevelDesign)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/Levels/"
                + (a_level < 10 ? "0" : "") + a_level + ".asset", typeof(LevelDesign));


            int count = 1;
            Vector3 currentFoward = Vector3.forward;
            Vector3 currentPosition = Vector3.zero;
            GameObject go = null;
            foreach (Junction junction in level.junctions)
            {
                go = null;

                if (junction.JuntionType == LevelDesign.JunctionType.Straight)
                    go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Straight"));

                go.transform.position = currentPosition;
                go.name = "" + count;

                int x = -2;
                int z = 1;
                for (int i = 0; i < 15; i++)
                {
                    if (junction.BlockWinPosition != null && junction.BlockWinPosition.Length > 0 && junction.BlockWinPosition[i] > 0)
                    {
                        for (int j = 0; j < junction.BlockWinPosition[i]; j++)
                        {
                            GameObject tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Box"));
                            tempGO.transform.position = new Vector3(go.transform.position.x + x, 0.6f + j*1, go.transform.position.z + z);
                            tempGO.name = "Box_" + i;
                            tempGO.transform.parent = GameObject.Find("Boxes").transform;
                        }
                    }
                    else if ((i >= 5 && i <= 9)
                        && junction.BlockLosePosition != null && junction.BlockLosePosition.Length > 0 && junction.BlockLosePosition[i-5] > 0)
                    {
                        for (int j = 0; j < junction.BlockLosePosition[i-5]; j++)
                        {
                            GameObject tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/BoxLose"));
                            tempGO.transform.position = new Vector3(go.transform.position.x + x, 0.6f + j*1, go.transform.position.z + z);
                            tempGO.name = "BoxLose_" + i;
                            tempGO.transform.parent = GameObject.Find("BoxesLose").transform;
                        }
                    }
                    else if (junction.CoinsPosition != null && junction.CoinsPosition.Length > 0 && junction.CoinsPosition[i])
                    {
                        GameObject tempGO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Coin"));
                        tempGO.transform.position = new Vector3(go.transform.position.x + x, 0.6f, go.transform.position.z + z);
                        tempGO.name = "Coin_" + i;
                        tempGO.transform.parent = GameObject.Find("Coins").transform;
                    }

                    x++;

                    if ((i + 1) % 5 == 0)
                        z--;
                    if (x >= 3)
                        x = -2;
                }
                go.transform.parent = GameObject.Find("Map").transform;
                count++;
                currentPosition += currentFoward * 3;
            }

            go = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Finish"));
            go.transform.position = currentPosition;
            go.name = "Finish";

            return level;
        }
    }
}
