// file=""LevelDesign.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 17/09/2021

#region usings
using System.Collections.Generic;
using UnityEngine;
using Game.Design.Juntion;
#endregion usings

namespace Game.Design.Level
{
    [CreateAssetMenu(fileName = "LevelDesign", menuName = "ScriptableObjects/LevelDesign")]
    public class LevelDesign : ScriptableObject
    {
        public enum JunctionType
        {
            Straight,
            Left,
            Right,
            Finish,
        }
        public enum JunctionPositions
        {
            Left,
            CenterLeft,
            Center,
            CenterRight,
            Right,
        }

        public List<Junction> junctions;

        public void Init(List<Junction> a_junctions)
        {
            junctions = a_junctions;
        }
    }
}
