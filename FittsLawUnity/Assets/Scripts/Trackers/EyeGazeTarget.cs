using UnityEngine;
using System.Collections;
using System;
using ns3DRudder;
using UnityEngine.UI;

/// <summary>
/// Class handling the functionality of the active target during the experiment.
/// </summary>
public class EyeGazeTarget : MonoBehaviour {

    public float GazeDuration { get; protected set; }
    public bool IsActivated { get; protected set; }
    public bool IsDisabled { get; protected set; }

    public delegate void Fixated();
    public event Fixated OnFixate;
    public delegate void Activated(bool hovered);
    public event Activated OnActivated;
    public delegate void Deactivated();
    public event Deactivated OnDeactivated;
    public delegate void Released();
    public event Released OnReleased;

    public delegate void Dwelled(float totalDwellTime);
    public event Dwelled OnDwell;

    protected Image _image;
    protected Rigidbody _rigidbody;
    protected MeshCollider _collider;
    protected RectTransform _rect;
    protected RectTransform _colliderRect;
    protected bool _isGazedAt;
    protected bool _halfActivated;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<MeshCollider>();
        _image = GetComponent<Image>();

        _rect = GetComponent<RectTransform>();
        _colliderRect = _collider.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Used when the cursor enters or is inside the target. Changes the color of the target; 
    /// if this is the frame in which the cursor first entered the target, adjust collider size.
    /// </summary>
    public virtual void OnEyeIn()
    {
        if (_halfActivated) return;
        _image.color = TestController.Instance.TestBlockData.HoverColor;
        if (_isGazedAt)
            OnContinuousGaze();
        else
        {
            if (OnFixate != null)
                OnFixate();
            _isGazedAt = true;
            SetColliderScale(TestController.Instance.TestBlockData.SpatialHysteresis);
        }
    }

    /// <summary>
    /// Used when the cursor exits the target. Resets the color of the target and collider size.
    /// </summary>
    public virtual void OnEyeOut() {
        _image.color = TestController.Instance.TestBlockData.TargetColor;
        _isGazedAt = false;
        ResetColliderScale();
    }

    /// <summary>
    /// Used when the target is activated (on click up or dwell duration reached).
    /// </summary>
    public virtual void Activate() {
        IsActivated = true;
        GazeDuration = 0;
        if (OnActivated != null)
            OnActivated(_isGazedAt);
    }

    public virtual void OnDeactivate() {
        IsActivated = false;
        GazeDuration = 0;
        if (OnDeactivated != null)
            OnDeactivated();
    }

    /// <summary>
    /// As long as cursor is inside the target and it's not yet been activated, increments dwell duration.
    /// </summary>
    public virtual void OnContinuousGaze() {
        if (!IsActivated)
        {
            GazeDuration += Time.deltaTime;
            if (OnDwell != null)
                OnDwell(GazeDuration);
        } 
    }

    /// <summary>
    /// If using click as activation method, change the target's color as long as the user
    /// doesn't let go of the clicked button.
    /// </summary>
    public virtual void OnHalfActivate()
    {
        _image.color = TestController.Instance.TestBlockData.ButtonDownColor;
        _halfActivated = true;
    }

    public void Grabbed()
    {
        _image.color = TestController.Instance.TestBlockData.ReadyForGestureColor;
        _collider.enabled = false;
        _rigidbody.isKinematic = true;
    }

    /// <summary>
    /// Set target rect's size to the passed diameter.
    /// </summary>
    /// <param name="diameter"> Diameter of the target. </param>
    public void SetSize(float diameter)
    {
        _rect.sizeDelta = new Vector2(diameter, diameter);
        if (_rect != null) {
            ResetColliderScale();
        }
    }

    /// <summary>
    /// Sets target's collider scale to the scale factor.
    /// </summary>
    /// <param name="scaleFactor"> The factor by which the collider is scaled. </param>
    public void SetColliderScale(float scaleFactor)
    {
        _colliderRect.localScale = new Vector3(scaleFactor * (_rect.rect.width / 100f), scaleFactor, scaleFactor * (_rect.rect.height / 100f));
    }

    /// <summary>
    /// Resets target's collider scale to its' default.
    /// </summary>
    public void ResetColliderScale()
    {
        _colliderRect.localScale = new Vector3(_rect.rect.width / 100f, 1, _rect.rect.height / 100f);
    }

    /// <summary>
    /// Set target's position.
    /// </summary>
    /// <param name="x"> X coordinate of new position.</param>
    /// <param name="y"> Y coordinate of new position.</param>
    public void SetPosition(float x, float y)
    {
        _rect.localPosition = new Vector3(x,y,0);
    }
}
