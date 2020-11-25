using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControlScript : MonoBehaviour
{
    public float mouseSensitivity = 0.25f;
    public float speed = 1;
    public float controllerRadius = 0.5f;

    public CharacterController controller;
    public Transform rotationTransfrorm;
    public ObjectControlScript objectControlScript;

    private Vector3 lastMouse;

#if UNITY_EDITOR || UNITY_STANDALONE
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        controller.radius = controllerRadius;
        StartCoroutine(StartDelayCoroutine());
    }


    void Update()
    {
        if (!objectControlScript.IsObjHolded || Input.GetMouseButton(1))
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            controller.Move(rotationTransfrorm.rotation * move * speed * Time.deltaTime);
        }
    }

    private IEnumerator StartDelayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(RotationCoroutine());
    }

    private IEnumerator RotationCoroutine()
    {
        lastMouse = Input.mousePosition;
        yield return null;
        while (true)
        {
            if (Input.GetMouseButton(1))
            {

                lastMouse = Input.mousePosition - lastMouse;
                lastMouse = new Vector3(-lastMouse.y * mouseSensitivity, lastMouse.x * mouseSensitivity, 0);
                lastMouse = new Vector3(rotationTransfrorm.eulerAngles.x + lastMouse.x, rotationTransfrorm.eulerAngles.y + lastMouse.y, 0);
                rotationTransfrorm.eulerAngles = lastMouse;
            }

            lastMouse = Input.mousePosition;
            yield return null;
        }
    }
#endif
}
