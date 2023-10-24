using Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    GameObject[] Walls;

    public Cell Cell { get; set; }

    public void SetActive(Directions dir, bool flag)
    {
        Walls[(int)dir].SetActive(flag);
    }
}
