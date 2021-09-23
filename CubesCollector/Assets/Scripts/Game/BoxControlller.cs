// file=""BoxControlller.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 23/09/2021

#region usings
using UnityEngine;
#endregion usings

namespace Game.Controller.Box
{
    public class BoxControlller : MonoBehaviour
    {
        [HideInInspector]
        public bool firstCollide;

        private void Awake()
        {
            firstCollide = false;
        }
    }
}
