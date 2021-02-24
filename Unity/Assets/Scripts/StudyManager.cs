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
                screenPlace.parent = Camera.main.transform;
                screenPlace.localPosition = new Vector3(0, 0, 0.3f);
                screenPlace.localRotation = Quaternion.identity;

                screen.parent = phoneScreen;
                screen.localPosition = Vector3.zero;
                screen.localRotation = Quaternion.identity;
                screen.gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case Conditions.Indirect:
                screenPlace.parent = cameraOffset;
                screen.parent = screenPlace;
                screen.localPosition = Vector3.zero;
                screen.localRotation = Quaternion.identity;

                SetIndirectPosRot();

                screen.gameObject.layer = LayerMask.NameToLayer("LayerOnTop");
                break;
        }

        this.condition = condition;
        yield return null;
    }

    private void SetIndirectPosRot()
    {
        var camForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
        screenPlace.localPosition = new Vector3(camForward.x, -0.1f, camForward.z) * 0.28f;
        screenPlace.localRotation =
            Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z),
            Camera.main.transform.up);
        screenPlace.rotation = Quaternion.Euler(screenPlace.eulerAngles.x, screenPlace.eulerAngles.y, Camera.main.transform.eulerAngles.z);
    }

    private void Update()
    {
        screenPlace.rotation = Quaternion.Euler(screenPlace.eulerAngles.x, screenPlace.eulerAngles.y, Camera.main.transform.eulerAngles.z);
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetContiditon(condition == Conditions.Direct ? Conditions.Indirect : Conditions.Direct);
        }

        if (condition == Conditions.Indirect)
        {
            SetIndirectPosRot();
        }
    }

    public void SetContiditon(Conditions condition)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(SetContiditonCoroutine(condition));
    }

    private IEnumerator SetScreenAndPhoneSizeCoroutine(float width, float height, float ratio)
    {
        phoneModel.localScale = new Vector3(width * 0.0108262f, height * 0.0057727f, phoneModel.localScale.z);
        float screenWidth = width * 0.0259079f;
        screen.localScale = new Vector3(screenWidth, screenWidth * ratio, screen.localScale.z);
        yield return null;
    }

    public void SetScreenAndPhoneSize(float width, float height, float ratio)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(SetScreenAndPhoneSizeCoroutine(width, height, ratio));
    }
}
