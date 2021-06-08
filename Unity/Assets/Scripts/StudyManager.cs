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
    public Transform camTransform;

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
                    SetIndirectPos();
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
        screenPlace.LookAt(camTransform);
    }

    public void SetIndirectPos()
    {
        screenPlace.localPosition = cameraOffset.localPosition
            + new Vector3(0, -0.1f, 0.3f);
        //var camForward = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
        //screenPlace.localPosition = new Vector3(0, 1.5f, 0) + new Vector3(camForward.x, -0.1f, camForward.z) * 0.4f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetContiditon(condition == Conditions.Direct ? Conditions.Indirect : Conditions.Direct);
        }

        if (condition == Conditions.Indirect || condition == Conditions.Warped)
        {
            SetIndirectRot();
        }
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
