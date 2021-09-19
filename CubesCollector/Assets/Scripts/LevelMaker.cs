// file=""LevelMaker.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 19/09/2021

#region usings
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Game.Design.Level;
using Game.Design.Juntion;
#endregion usings

public class LevelMaker : EditorWindow
{
    [MenuItem("EditorLevel/Maker")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelMaker window = (LevelMaker)GetWindow(typeof(LevelMaker));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Create"))
            CreateLevel();
    }

    private void CreateLevel()
    {
        List<Junction> allJunctions = new List<Junction>();
        int count = 2;

        for (int i = 0; i < 10; i++)
        {
            Junction junction = new Junction();
            bool[] coins = new bool[15];
            int[] blockWins = new int[15];
            int[] blockLose = new int[5];

            for (int x = 0; x < 5; x++)
                for (int y = 0; y < 3; y++)
                {
                    coins[x+y*5] = false;
                }


            if (i == 4)
                blockWins[7] = 1;
            else if (i == 8)
                blockLose[2] = 1;

            junction.Init(LevelDesign.JunctionType.Straight, coins, blockWins, blockLose);

            count++;
            if (count >= 5)
                count = 0;

            allJunctions.Add(junction);
        }

        LevelDesign asset = CreateInstance<LevelDesign>();
        AssetDatabase.CreateAsset(asset, "Assets/Resources/Levels/02.asset");

        asset.Init(allJunctions);

        EditorUtility.SetDirty(asset);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
}
