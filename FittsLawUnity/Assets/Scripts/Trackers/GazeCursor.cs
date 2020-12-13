using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// This class contains functionality that ensures a consistent cursor representation 
/// across different control methods.
/// </summary>

public class GazeCursor : MonoBehaviour
{

    public static GazeCursor Instance { get; private set; }

    /// <returns>Returns the cursor's position on the canvas.</returns>
    public Vector2 Position
    {
        get { return _cursor.localPosition; }
    }

    /// <returns>Returns the gaze position on the canvas.</returns>
    public Vector2 GazePosition
    {
        get { return _gazePosition.localPosition; }
    }

    /// <returns>Returns the head position on the canvas.</returns>
    public Vector2 HeadPosition
    {
        get { return _headPosition.localPosition; }
    }

    /* Delegates and events for handling interactions between the cursor and targets.
     * See OnTriggerEnter(), OnTriggerStay(), OnTriggerExit below. */
    public delegate void OnTargetHover(EyeGazeTarget gazeTarget);

    public event OnTargetHover TargetHovered;

    public delegate void OnTargetStay(EyeGazeTarget gazeTarget);

    public event OnTargetStay TargetStayed;

    public delegate void OnTargetUnhover(EyeGazeTarget gazeTarget);

    public event OnTargetUnhover TargetUnhovered;

    private float _squareSide;
    [SerializeField] public float MouseSensitivity;
    private RectTransform _cursor;
    private RectTransform _gazePosition;
    private RectTransform _headPosition;
    private SphereCollider _collider;
    private EyeGazeTarget _trackedTarget;
    private Image _image;
    private RectTransform _background;
    private Canvas _canvas;
    private Vector2 _previousPosition;
    private Vector2 _mouse;
    private Vector2 _canvasSize;

    void Awake()
    {
        Instance = this;
        _image = GetComponent<Image>();
        _canvas = GetComponentInParent<Canvas>();
        _cursor = GetComponent<RectTransform>();
        _collider = GetComponent<SphereCollider>();
        _background = transform.parent.Find("Background").GetComponent<RectTransform>();
        _gazePosition = transform.parent.Find("Gaze Position").GetComponent<RectTransform>();
        _headPosition = transform.parent.Find("Head Position").GetComponent<RectTransform>();
    }

    void Start()
    {
        CalculateScreenBounds();
        SetSquareSide();
        _previousPosition = new Vector2(0, 0);
        _mouse = new Vector2(0, 0);

        _canvasSize = new Vector2(_canvas.pixelRect.width, _canvas.pixelRect.height);
    }

    /// <summary>
    /// This function assigns proportions to a background collider for the canvas,
    /// which can then be used for collision detection if setting the cursor position using a ray.
    /// </summary>
    public void CalculateScreenBounds()
    {
        _background.sizeDelta = new Vector2(_canvas.pixelRect.width, _canvas.pixelRect.height);
        BoxCollider backgroundCollider = _background.gameObject.GetComponent<BoxCollider>();
        backgroundCollider.size = new Vector3(_canvas.pixelRect.width, _canvas.pixelRect.height, 1);
    }
    /// <summary>
    /// Initializes the _squareSide variable, used for the correct setting 
    /// of cursor position when using an analog joystick-style control method.
    /// </summary>
    public void SetSquareSide()
    {
        _squareSide = (_canvas.pixelRect.width < _canvas.pixelRect.height)
            ? _canvas.pixelRect.width
            : _canvas.pixelRect.height;
    }
    /// <summary>
    /// Sets the sprite of the cursor.
    /// </summary>
    /// <param name="sprite"> Sprite for the cursor.</param>
    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    /// <summary>
    /// Sets the color of the cursor.
    /// </summary>
    /// <param name="color"> Color for the cursor.</param>
    public void SetColor(Color color)
    {
        _image.color = color;
    }
    /// <summary>
    /// Sets the size of the cursor and its' collider.
    /// </summary>
    /// <param name="size"> Cursor size.</param>
    public void SetSize(float size)
    {
        _cursor.sizeDelta = new Vector2(size, size);

        if (_cursor != null)
            _collider.radius = _cursor.rect.width / 2;
    }

    /// <summary>
    /// Toggles the visual representation of the cursor.
    /// </summary>
    /// <param name="isEnabled"> Value to enable/disable image of cursor.</param>
    public void SetEnabled(bool isEnabled)
    {
        _image.enabled = isEnabled;
    }

    /// <summary>
    /// Calculates and sets the cursor position within the canvas when using the mouse or wheelchair control method.
    /// </summary>
    /// <param name="deltaX"> X coordinate of new mouse position.</param>
    /// <param name="deltaY"> Y coordinate of new mouse position.</param>
    public void SetPositionFromMouse(float deltaX, float deltaY)
    {
        _previousPosition = new Vector2(_cursor.localPosition.x, _cursor.localPosition.y);
        _mouse = new Vector2(_previousPosition.x + deltaX * MouseSensitivity,
            _previousPosition.y + deltaY * MouseSensitivity);
        _mouse = new Vector2(Mathf.Clamp(_mouse.x, -_canvasSize.x / 2, _canvasSize.x / 2),
            Mathf.Clamp(_mouse.y, -_canvasSize.y / 2, _canvasSize.y / 2));
        _cursor.localPosition = new Vector3(_mouse.x, _mouse.y, _cursor.localPosition.z);
    }

    public void SetPositionFromFinger(float x, float y)
    {
        var fingerPos = new Vector2(Mathf.Clamp(x, 0, _canvasSize.x), Mathf.Clamp(y, 0, _canvasSize.y));
        _cursor.localPosition = new Vector3(fingerPos.x - _canvasSize.x / 2, fingerPos.y - _canvasSize.y / 2, _cursor.localPosition.z);
    }

    /// <summary>
    /// Sets the cursor position on the canvas when using an analog stick type control method (gamepad, 3DRudder). 
    /// </summary>
    /// <param name="x"> X coordinate of analog stick.</param>
    /// <param name="y"> Y coordinate of analog stick.</param>
    public void SetPositionFromJoystick(float x, float y)
    {
        _cursor.localPosition = new Vector3(x * _squareSide / 4, y * _squareSide / 4, _cursor.localPosition.z);
    }

    /// <summary>
    /// Set the cursor position on the canvas when using a ray casting control method (eyetracking, headtracking, VIVE hand pointer). 
    /// </summary>
    /// <param name="hit"> Position on the canvas hit by the raycast from eye/head/hand-tracking.</param>
    /// <param name="camera"> Camera fixed to always look at the canvas.</param>
    public void SetPositionFromRay(RaycastHit hit, Camera camera)
    {
        Vector2 viewportPosition = camera.WorldToViewportPoint(hit.point);
        Vector2 proportionalPosition = new Vector2(viewportPosition.x * _background.sizeDelta.x,
            viewportPosition.y * _background.sizeDelta.y);
        _cursor.localPosition = proportionalPosition - _canvasSize / 2;
    }

    /// <summary>
    /// Calculate and set the gaze position on the canvas.
    /// Gaze position is by default invisible and only tracked for data collection purposes.
    /// You can enable it in the Unity inspector under MainScene -> TargetCanvas -> Gaze Position -> Tick the checkmark "Image (Script)".
    /// </summary>
    /// <param name="hit"> Position on the canvas hit by the raycast from eye-tracking.</param>
    /// <param name="camera"> Camera fixed to always look at the canvas.</param>
    // TODO: Test if this works on the FOVE 
    public void SetGazePositionFromRay(RaycastHit hit, Camera camera) {
        Vector2 viewportPosition = camera.WorldToViewportPoint(hit.point);
        Vector2 proportionalPosition = new Vector2(viewportPosition.x * _background.sizeDelta.x,
            viewportPosition.y * _background.sizeDelta.y);
        _gazePosition.localPosition = proportionalPosition - _canvasSize / 2;
    }

    /// <summary>
    /// Calculate and set the head position on the canvas.
    /// Head position is by default invisible and only tracked for data collection purposes. 
    /// You can enable it in the Unity inspector under MainScene -> TargetCanvas -> Head Position -> Tick the checkmark "Image (Script)".
    /// </summary>
    /// <param name="hit"> Position on the canvas hit by the raycast from head-tracking.</param>
    /// <param name="camera"> Camera fixed to always look at the canvas.</param>
    // TODO: Test if this works on the FOVE 
    public void SetHeadPositionFromRay(RaycastHit hit, Camera camera) {
        Vector2 viewportPosition = camera.WorldToViewportPoint(hit.point);
        Vector2 proportionalPosition = new Vector2(viewportPosition.x * _background.sizeDelta.x,
            viewportPosition.y * _background.sizeDelta.y);
        _headPosition.localPosition = proportionalPosition - _canvasSize / 2;
    }


    /// <summary>
    /// If the other object has an EyeGazeTarget component,
    /// signal the subscriber to the appropriate event in VREyeTrackercontroller.cs.
    /// </summary>
    /// <param name="other"> The collider of the other object.</param>
    void OnTriggerEnter(Collider other)
    {
        EyeGazeTarget target = other.transform.parent.GetComponent<EyeGazeTarget>();
        if (target == null)
            return;
        
        if (_trackedTarget != null)
        {
            if (_trackedTarget == target)
                return;
            if (TargetUnhovered != null)
                TargetUnhovered(_trackedTarget);
        }
        _trackedTarget = target;
        if (TargetHovered != null)
            TargetHovered(target);
    }

    /// <summary>
    /// If the other object has an EyeGazeTarget component, signal the TargetStayed event's subscriber.
    /// </summary>
    /// <param name="other"> The collider of the other object.</param>
    void OnTriggerStay(Collider other)
    {
        EyeGazeTarget target = other.transform.parent.GetComponent<EyeGazeTarget>();
        if (target != null)
            if(TargetStayed != null)
               TargetStayed(target);
    }

    /// <summary>
    /// If the other object has an EyeGazeTarget component, signal the TargetUnhovered event's subscriber. 
    /// </summary>
    /// <param name="other"> The collider of the other object.</param>
    void OnTriggerExit(Collider other) 
    {
        EyeGazeTarget target = other.transform.parent.GetComponent<EyeGazeTarget>();
        if(target != null)
            if (TargetUnhovered != null)
                TargetUnhovered(target);
        _trackedTarget = null;
    }
}
