using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneManagerScript : MonoBehaviour
{
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
    }
}
