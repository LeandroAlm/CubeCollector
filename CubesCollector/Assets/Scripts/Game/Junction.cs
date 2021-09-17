// file=""Juntion.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 17/09/2021

#region usings
using static Game.Design.Level.LevelDesign;
#endregion usings

namespace Game.Design.Juntion
{
    public class Junction
    {
        private JuntionType JuntionType;
        private bool[,] CoinsPosition;
        private int[] BlockPosition;

        void Init(JuntionType a_junctionType, bool[,] a_CoinsPosition, int[] a_BlockPosition)
        {
            JuntionType = a_junctionType;
            CoinsPosition = a_CoinsPosition;
            BlockPosition = a_BlockPosition;
        }
    }
}
