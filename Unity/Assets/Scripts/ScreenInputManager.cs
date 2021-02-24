using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StudyManager;

public class ScreenInputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ////var condition = Instance.condition;
            ////Instance.SetContiditon(condition == Conditions.Direct ? Conditions.Indirect : Conditions.Direct);
            Application.Quit();
        }
    }
}
