using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MouseControlScript : MonoBehaviour
{
    //public XRBaseInteractor rightInteractor;
    //public XRBaseInteractor leftInteractor;
    public ObjectControlScript objectControlScript;

    //public float interactorHeightOffset;

    public float raycastMaxDistance = 3.0f;

#if UNITY_EDITOR || UNITY_STANDALONE
    void Start()
    {
        //leftInteractor.gameObject.SetActive(false);
        //rightInteractor.GetComponent<XRRayInteractor>().raycastMask =
        //    (1 << LayerMask.NameToLayer("Grab")) |
        //    (1 << LayerMask.NameToLayer("UI")) |
        //    (1 << LayerMask.NameToLayer("Grab Ignore Ray"));
        //rightInteractor.transform.position = new Vector3(
        //    rightInteractor.transform.position.x,
        //    rightInteractor.transform.position.y - interactorHeightOffset,
        //    rightInteractor.transform.position.z);
    }

    void Update()
    {
        bool isReleased = false;
        if (Input.GetMouseButtonDown(0) && objectControlScript.IsObjHolded)
        {
            isReleased = true;
            objectControlScript.ReleaseObject();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 30f, ~LayerMask.GetMask()))
        {
            //rightInteractor.transform.LookAt(hit.point);
            if (hit.distance > raycastMaxDistance)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0) && !isReleased)
            {
                if (Physics.Raycast(ray, out RaycastHit hit2, 30f, LayerMask.GetMask("Grab Ignore Ray And Inner Col")))
                {
                    if (CheckIsSimpleInteractable(hit2.transform))
                    {
                        //var buttonInteractable = hit2.transform.GetComponent<XRSimpleInteractable>();
                        //buttonInteractable.onHoverEntered?.Invoke(rightInteractor);
                    }
                }
                else if (CheckIsSimpleInteractable(hit.transform))
                {
                    //var coinInteractable = hit.transform.GetComponent<XRSimpleInteractable>();
                    //coinInteractable.onHoverEntered?.Invoke(rightInteractor);
                }
                else if (CheckIsGrabable(hit.transform) && CheckInteractableCollider(hit.transform, hit.collider))
                {
                    objectControlScript.GrabObject(hit.transform);
                }
            }
        }
    }

    private static bool CheckIsGrabable(Transform obj)
    {
        var xrGrabScript = obj.GetComponent<XRGrabInteractable>();
        return xrGrabScript != null && xrGrabScript.isActiveAndEnabled;
    }

    private static bool CheckInteractableCollider(Transform obj, Collider collider)
    {
        var xrGrabScript = obj.GetComponent<XRGrabInteractable>();
        return xrGrabScript.colliders.Contains(collider);
    }

    private static bool CheckIsSimpleInteractable(Transform obj)
    {
        return obj.GetComponent<XRSimpleInteractable>() != null;
    }
#endif
}
