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
using UnityEngine.SceneManagement;
using static Game.Design.Level.LevelDesign;
using System.Linq;
using UnityEditorInternal;
using System;
using Game.Loader.Level;
#endregion usings

public class LevelMaker : EditorWindow
{
    private LevelDesign currentLevel;
    private List<Junction> currentJunctions;
    private int nameID;
    private string answerMessage;
    private answerMessageType currentAnswerType;

    string[] allJunctionsType;
    private int currentJuntionTypeID;

    public bool[] currentCoinsPosition;
    public int[] currentBlockWinPosition;
    public int[] currentBlockLosePosition;

    SerializedObject so;

    SerializedProperty coins;
    SerializedProperty boxes;
    SerializedProperty boxesLose;

    enum answerMessageType
    {
        Empety,
        Ok,
        Error,
    }

    [MenuItem("EditorLevel/Maker")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelMaker window = (LevelMaker)GetWindow(typeof(LevelMaker));
        window.Show();
    }

    void OnEnable()
    {
        currentCoinsPosition = new bool[15];
        currentBlockWinPosition = new int[15];
        currentBlockLosePosition = new int[5];

        ScriptableObject target = this;
        so = new SerializedObject(target);

        coins = so.FindProperty("currentCoinsPosition");
        boxes = so.FindProperty("currentBlockWinPosition");
        boxesLose = so.FindProperty("currentBlockLosePosition");
    }

    void OnGUI()
    {
        if (allJunctionsType == null || allJunctionsType.Length < 0)
        {
            allJunctionsType = System.Enum.GetNames(typeof(JunctionType));
            allJunctionsType = allJunctionsType.Where(t => t != "Finish").ToArray();
        }

        GUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 40;
        nameID = EditorGUILayout.IntField("Level", nameID, GUILayout.Width(80));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start new Level"))
            StartNewLevel();
        if (GUILayout.Button("Restart the level"))
            RestartLevel();
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        currentJuntionTypeID = EditorGUILayout.Popup("", currentJuntionTypeID, allJunctionsType);

        if (currentJuntionTypeID == 0)
        {
            GUILayout.BeginHorizontal();
            // "target" can be any class derrived from ScriptableObject 
            // (could be EditorWindow, MonoBehaviour, etc)
            //ScriptableObject target = this;
            //SerializedObject so = new SerializedObject(target);

            if (currentCoinsPosition == null)
            {
                currentCoinsPosition = new bool[15];
                currentBlockWinPosition = new int[15];
                currentBlockLosePosition = new int[5];
            }
            //SerializedProperty p1 = so.FindProperty("currentCoinsPosition");
            //EditorGUILayout.PropertyField(p1);
            //so.ApplyModifiedProperties();
            EditorGUILayout.PropertyField(coins, new GUIContent("Coins positions"));
            EditorGUILayout.PropertyField(boxes, new GUIContent("Boxes positions"));
            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(boxesLose, new GUIContent("Boxes Lose positions"));

            //if (currentBlockWinPosition == null)
            //    currentBlockWinPosition = new int[15];
            //SerializedProperty p2 = so.FindProperty("currentBlockWinPosition");
            //EditorGUILayout.PropertyField(p2, true);
            //so.ApplyModifiedProperties();

            //if (currentBlockLosePosition == null)
            //    currentBlockLosePosition = new int[5];
            //SerializedProperty p3 = so.FindProperty("currentBlockLosePosition");
            //EditorGUILayout.PropertyField(p3, true);
            so.ApplyModifiedProperties();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Junction"))
            AddJunction();

        if (GUILayout.Button("Undo Junction"))
            UndoJunction();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Save level!"))
            SaveLevel();

        if (!string.IsNullOrEmpty(answerMessage))
        {
            GUILayout.FlexibleSpace();

            if (currentAnswerType == answerMessageType.Ok)
                GUI.contentColor = Color.green;
            else
            if (currentAnswerType == answerMessageType.Error)
                GUI.contentColor = Color.red;

            GUILayout.Label(answerMessage);
        }
    }

    private void StartNewLevel()
    {
        cleanMessage();
        if (nameID <= 0)
        {
            SetMessageAndType("Must select a level number higher than 0", answerMessageType.Error);
            return;
        }

        currentLevel = CreateInstance<LevelDesign>();
        currentJunctions = new List<Junction>();

        currentCoinsPosition = new bool[15];
        currentBlockWinPosition = new int[15];
        currentBlockLosePosition = new int[5];

        GameObject.Find("LevelLoader").GetComponent<LevelLoader>().setStartValues();

        SetMessageAndType("You start a new level: " + nameID, answerMessageType.Ok);
    }

    private void RestartLevel()
    {
        EraseLevel();
        StartNewLevel();
        Repaint(); ;
    }

    private void AddJunction()
    {
        cleanMessage();
        if (SceneManager.GetActiveScene().name != "LevelMaker")
            SetMessageAndType("Must be in scene 'LevelMaker', please open", answerMessageType.Error);

        if (currentLevel == null || nameID <= 0 || currentJunctions == null)
            SetMessageAndType("Must create a level first", answerMessageType.Error);

        if (!string.IsNullOrEmpty(answerMessage))
            return;


        Junction currentJunction = new Junction();
        JunctionType type = (JunctionType)Enum.Parse(typeof(JunctionType), System.Enum.GetNames(typeof(JunctionType))[currentJuntionTypeID]);
        currentJunction.Init(type, currentCoinsPosition, currentBlockWinPosition, currentBlockLosePosition);
        currentJunctions.Add(currentJunction);

        SetMessageAndType("Junction add with sucess", answerMessageType.Ok);

        GameObject.Find("LevelLoader").GetComponent<LevelLoader>().LoadAJunction(currentJunction, GameObject.Find("Map").transform, Resources.Load<Material>("Materials/Mat_Box_0"));

        currentCoinsPosition = new bool[15];
        currentBlockWinPosition = new int[15];
        currentBlockLosePosition = new int[5];
        Repaint();
    }

    private void UndoJunction()
    {
        cleanMessage();
        if (SceneManager.GetActiveScene().name != "LevelMaker")
            SetMessageAndType("Must be in scene 'LevelMaker', please open", answerMessageType.Error);

        if (currentLevel == null || nameID <= 0)
            SetMessageAndType("Must create a level first", answerMessageType.Error);

        if (currentJunctions == null || currentJunctions.Count <= 0)
            SetMessageAndType("No junctions added till moment", answerMessageType.Error);

        if (!string.IsNullOrEmpty(answerMessage))
            return;

        currentJunctions.RemoveAt(currentJunctions.Count - 1);
        GameObject.Find("LevelLoader").GetComponent<LevelLoader>().UndoJunction(currentJunctions[currentJunctions.Count-1].JuntionType);
    }

    /// <summary>
    /// Save the level in "Resources/Levels"
    /// </summary>
    private void SaveLevel()
    {
        if (currentJunctions == null || currentJunctions.Count <= 0
            || currentLevel == null)
        {
            SetMessageAndType("You need create a level and add some junctions!", answerMessageType.Error);
            return;
        }

        // Scriptable object create and save
        AssetDatabase.CreateAsset(currentLevel, "Assets/Resources/Levels/" + (nameID < 10 ? "0" : "") + nameID + ".asset");

        currentLevel.Init(currentJunctions);

        EditorUtility.SetDirty(currentLevel);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        EraseLevel();
        Repaint();

        SetMessageAndType("Level created with sucess", answerMessageType.Ok);
    }

    private void EraseLevel()
    {
        GameObject.Find("LevelLoader").GetComponent<LevelLoader>().DestroyCurrentLevel();
    }


    /// <summary>
    /// Set message
    /// </summary>
    /// <param name="a_message">message to display</param>
    /// <param name="a_Type">message type</param>
    /// <param name="a_ignore">just if is empety string</param>
    private void SetMessageAndType(string a_message, answerMessageType a_Type)
    {
        currentAnswerType = a_Type;
        answerMessage = a_message;
    }

    /// <summary>
    /// Clean the messge
    /// </summary>
    private void cleanMessage()
    {
        currentAnswerType = answerMessageType.Empety;
        answerMessage = "";
    }
}
