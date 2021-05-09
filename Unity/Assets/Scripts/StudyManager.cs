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

    public GameObject indirectHint1;
    public GameObject indirectHint2;

    public static StudyManager Instance;

    public Conditions condition = Conditions.Direct;

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
    }

    private IEnumerator SetContiditonCoroutine(Conditions condition)
    {
        switch (condition)
        {
            case Conditions.Direct:
            case Conditions.Warped:
            case Conditions.UnWarped:
                indirectHint1.SetActive(false);
                indirectHint2.SetActive(true);

                screen.parent = phoneScreen;
                screen.localPosition = Vector3.zero;
                screen.localRotation = Quaternion.identity;
                screen.gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case Conditions.Indirect:
                screen.parent = screenPlace;
                screen.localPosition = Vector3.zero;
                screen.localRotation = Quaternion.identity;

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
        //var camForward = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z).normalized;
        //screenPlace.localPosition = new Vector3(camForward.x, -0.1f, camForward.z) * 0.42f;
        screenPlace.localRotation = mainCam.transform.localRotation;
        //screenPlace.localRotation =
        //    Quaternion.LookRotation(new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z),
        //    mainCam.transform.up);
        //screenPlace.localRotation = Quaternion.Euler(screenPlace.eulerAngles.x, screenPlace.eulerAngles.y, mainCam.transform.eulerAngles.z);
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
            if ((indirectHint1.activeSelf || indirectHint2.activeSelf) && IsShownOnTheScreen(screen.position))
            {
                indirectHint1.SetActive(false);
                indirectHint2.SetActive(false);
            }
        }
    }

    private bool IsShownOnTheScreen(Vector3 worldPos)
    {
        var pos = mainCam.WorldToViewportPoint(worldPos);
        return pos.z >= 0 && pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1;
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
