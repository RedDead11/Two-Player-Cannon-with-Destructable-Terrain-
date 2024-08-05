using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFPS : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
