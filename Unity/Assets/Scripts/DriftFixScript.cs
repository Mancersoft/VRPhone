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
        Input.gyro.enabled = true;
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

        var currPosition = transform.position;
        camTransform.parent = null;
        transform.position = camTransform.position;
        camTransform.parent = transform;

        transform.Rotate(new Vector3(0, shiftAngle * Time.deltaTime, 0), Space.Self);

        camTransform.parent = null;
        transform.position = currPosition;
        camTransform.parent = transform;


        var currRotation = transform.localRotation;
        camTransform.parent = null;
        transform.rotation = Quaternion.identity;
        camTransform.parent = transform;

        transform.localPosition = new Vector3(
            -camTransform.localPosition.x,
            -camTransform.localPosition.y + 1.5f,
            -camTransform.localPosition.z);

        camTransform.parent = null;
        transform.rotation = currRotation;
        camTransform.parent = transform;
    }
}
