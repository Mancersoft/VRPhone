using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneManagerScript : MonoBehaviour
{
    public GameObject screen;

    private void Start()
    {
        ChangeVisibility(false);
    }

    public void ChangeVisibility(bool isVisible)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var renderer in renderers)
        {
            foreach (var collider in colliders)
            {
                renderer.enabled = isVisible;
                collider.enabled = isVisible;
            }
        }

        var screenRenderer = screen.GetComponent<Renderer>();
        if (screenRenderer != null)
        {
            screenRenderer.enabled = isVisible;
        }

        var screenCollider = screen.GetComponent<Collider>();
        if (screenCollider != null)
        {
            screenCollider.enabled = isVisible;
        }
    }
}
