// file=""CoinFlip.cs" company=""
// Copyright (c) 2021 All Rights Reserved
// Author: Leandro Almeida
// Date: 20/09/2021

#region usings
using UnityEngine;
#endregion usings

public class CoinFlip : MonoBehaviour
{
    private void Awake()
    {
        transform.Rotate(0, Random.Range(0, 360), 0, Space.World);
    }

    void Update()
    {
        transform.Rotate(0, 100*Time.deltaTime, 0, Space.World);
    }
}
