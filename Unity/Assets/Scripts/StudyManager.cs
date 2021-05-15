using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StudyManager : MonoBehaviour
{
    public Transform screenPlace;
    public Transform screen;
    public Transform phoneScreen;
    public Transform phoneModel;
    public Transform cameraOffset;
    public Camera mainCam;

    public GameObject indirectHint;

    public static StudyManager Instance;

    public Conditions condition = Conditions.Direct;

    private Renderer screenRenderer;

    public enum Conditions
    {
        Direct,
        Indirect,
        UnWarped,
        Warped
    }

    private void Awake()
    {
        Instance = this;
        screenRenderer = screen.GetComponent<Renderer>();
    }

    private IEnumerator SetContiditonCoroutine(Conditions condition)
    {
        switch (condition)
        {
            case Conditions.Direct:
            case Conditions.UnWarped:
                screen.parent = phoneScreen;
                screen.localPosition = Vector3.zero;
                screen.localRotation = Quaternion.identity;
                screen.gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case Conditions.Indirect:
            case Conditions.Warped:
                screen.parent = screenPlace;
                screen.localPosition = Vector3.zero;
                screen.localEulerAngles = new Vector3(0, 180, 0);

                if (this.condition != condition)
                {
                    var camForward = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z).normalized;
                    screenPlace.localPosition = new Vector3(camForward.x, -0.1f, camForward.z) * 0.42f;
                    SetIndirectRot();
                }

                screen.gameObject.layer = LayerMask.NameToLayer("LayerOnTop");
                break;
        }

        this.condition = condition;
        yield return null;
    }

    private void SetIndirectRot()
    {
        screenPlace.LookAt(mainCam.transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetContiditon(condition == Conditions.Direct ? Conditions.Indirect : Conditions.Direct);
        }

        if (condition == Conditions.Indirect)
        {
            SetIndirectRot();
            indirectHint.SetActive(!IsShownOnTheScreen());
            //Debug.Log(mainCam.fieldOfView + " " + mainCam.farClipPlane + " " + mainCam.aspect + " " + mainCam.focalLength
            //    + " " + mainCam.nearClipPlane + " " + mainCam.scaledPixelWidth + " " + mainCam.scaledPixelHeight);
        }
    }

    private bool IsShownOnTheScreen()
    {
        //var pos = mainCam.WorldToViewportPoint(screen.position);
        //Debug.Log(pos);
        //return pos.z >= 0 && pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1;
        var planes = GeometryUtility.CalculateFrustumPlanes(mainCam);
        return GeometryUtility.TestPlanesAABB(planes, screenRenderer.bounds);
    }

    public void SetContiditon(Conditions condition)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(SetContiditonCoroutine(condition));
    }

    private IEnumerator SetScreenAndPhoneSizeCoroutine(float width, float height, float ratio)
    {
        phoneModel.localScale = new Vector3(width * 0.0174884938229822f, height * 0.009325172830074f, phoneModel.localScale.z);
        float screenWidth = width * 0.0437212241476402f;
        screen.localScale = new Vector3(screenWidth, screenWidth * ratio, screen.localScale.z);
        yield return null;
    }

    public void SetScreenAndPhoneSize(float width, float height, float ratio)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(SetScreenAndPhoneSizeCoroutine(width, height, ratio));
    }
}
