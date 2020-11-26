using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectControlScript : MonoBehaviour
{
    public Transform controlParent;

    //public XRBaseInteractor rightInteractor;

    public float moveSpeed;
    public float rotSpeed;

    public bool IsObjHolded;

    public Transform holdedObj;
    private float objDistance;
    private Vector3 objOffset;
    private Rigidbody holdedObjRigidbody;
    private Transform holdedObjOldParent;

#if UNITY_EDITOR || UNITY_STANDALONE
    void Start()
    {
        
    }

    void Update()
    {
        if (IsObjHolded)
        {
            holdedObjRigidbody.velocity = Vector3.zero;

            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objDistance);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + objOffset;
            Vector3 oldLocalPosition = holdedObj.localPosition;
            holdedObj.position = curPosition;

            if (Input.GetMouseButton(1))
            {
                return;
            }

            if (!Input.GetKey(KeyCode.LeftControl))
            {
                float distValue = 0f;
                if (Input.GetKey(KeyCode.W))
                {
                    distValue++;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    distValue--;
                }

                objDistance += distValue * moveSpeed * Time.deltaTime;
            }
            else
            {
                Vector3 value = (holdedObj.localPosition - oldLocalPosition).normalized;
                if (Input.GetKey(KeyCode.W))
                {
                    value += Vector3.forward;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    value -= Vector3.forward;
                }

                var rotValue = new Vector3(-value.y, -value.x, value.z);
                holdedObj.Rotate(rotValue * rotSpeed * Time.deltaTime, Space.Self);
            }
        }
    }

    public void GrabObject(Transform obj)
    {
        if (IsObjHolded || obj == null)
        {
            return;
        }

        holdedObj = obj;
        holdedObjOldParent = holdedObj.parent;
        holdedObj.parent = controlParent;
        holdedObjRigidbody = holdedObj.GetComponent<Rigidbody>();
        holdedObjRigidbody.useGravity = false;
        holdedObjRigidbody.freezeRotation = true;

        //var grabInteractable = GetGrabInteractable(obj);
        //grabInteractable?.onSelectEntered?.Invoke(rightInteractor);

        objDistance = Camera.main.WorldToScreenPoint(holdedObj.position).z;
        objOffset = holdedObj.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objDistance));

        IsObjHolded = true;
    }

    private static XRGrabInteractable GetGrabInteractable(Transform obj)
    {
        return obj.GetComponent<XRGrabInteractable>();
    }

    public void ReleaseObject()
    {
        if (holdedObj != null)
        {
            holdedObjRigidbody.useGravity = true;
            holdedObjRigidbody.freezeRotation = false;
            holdedObj.parent = holdedObjOldParent;
            holdedObj = null;
            holdedObjRigidbody = null;
        }

        IsObjHolded = false;
    }
#endif
}
