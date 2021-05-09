using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    [SerializeField]
    float sensitivity;
    //[SerializeField]
    //float zoomSpeed;

    private Vector3 anchorPoint;
    private Quaternion anchorRot;

    private void Awake()
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        enabled = false;
#endif
    }

    void Update()
    {
        //transform.Translate(move * Time.deltaTime);

        if (Input.GetMouseButtonDown(1))
        {
            anchorPoint = new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            anchorRot = transform.rotation;
        }

        if (Input.GetMouseButton(1))
        {
            Quaternion rot = anchorRot;
            Vector3 dif = anchorPoint - new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            rot.eulerAngles += dif * sensitivity;
            transform.rotation = rot;
        }

        //transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime, Space.Self);
    }
}
