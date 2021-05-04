using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftFixScript : MonoBehaviour
{
    public Transform camTransform;

    private float startShift;
    private bool isInit = false;

    public void Init()
    {
        Input.compass.enabled = true;
        StartCoroutine(StartShiftCoroutine());
    }

    private IEnumerator StartShiftCoroutine()
    {
        while (Input.compass.trueHeading == 0)
        {
            yield return null;
        }

        startShift = Mathf.DeltaAngle(camTransform.eulerAngles.y, Input.compass.magneticHeading);
        isInit = true;
    }

    void Update()
    {
        if (!isInit)
        {
            return;
        }

        float shiftAngle = Mathf.DeltaAngle(camTransform.eulerAngles.y, Input.compass.magneticHeading - startShift);
        transform.Rotate(new Vector3(0, shiftAngle * Time.deltaTime, 0), Space.Self);
    }
}
