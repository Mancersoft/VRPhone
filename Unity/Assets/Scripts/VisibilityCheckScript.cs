using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityCheckScript : MonoBehaviour
{
    public GameObject indirectHint;

    //private StudyManager studyManager;

    private void OnBecameVisible()
    {
        indirectHint.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        indirectHint.SetActive(true);
    }

    //private void Start()
    //{
    //    studyManager = StudyManager.Instance;
    //}

    //private void Update()
    //{
    //    if (indirectHint.activeSelf &&
    //        (studyManager.condition == StudyManager.Conditions.Indirect
    //        || studyManager.condition == StudyManager.Conditions.Warped))
    //    {
    //        StudyManager.Instance.SetIndirectPos();
    //    }
    //}
}
