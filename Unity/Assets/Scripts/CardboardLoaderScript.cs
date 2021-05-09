using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CardboardLoaderScript : MonoBehaviour
{
    public DriftFixScript driftFixScript;

    void Start()
    {
        StartCoroutine(LoadDevice("cardboard"));
    }

    IEnumerator LoadDevice(string newDevice)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = true;
        yield return null;
        Screen.orientation = ScreenOrientation.Landscape;
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        driftFixScript.Init();
    }
}
