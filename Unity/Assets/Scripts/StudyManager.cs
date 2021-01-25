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

    public static StudyManager Instance;

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
                screen.parent = phoneScreen;
                screen.localPosition = Vector3.zero;
                screen.localRotation = Quaternion.identity;
                screen.gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case Conditions.Indirect:
                screen.parent = screenPlace;
                screen.localPosition = Vector3.zero;
                screen.localRotation = Quaternion.identity;
                screen.gameObject.layer = LayerMask.NameToLayer("LayerOnTop");
                break;
        }

        yield return null;
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
