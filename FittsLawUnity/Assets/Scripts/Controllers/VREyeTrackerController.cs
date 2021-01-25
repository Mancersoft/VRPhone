using System;
using UnityEngine;
using System.Collections.Generic;
using Fove;
using UnityEngine.UI;
using Unity3DRudder;
using WheelchairSerialConnect;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles eye-tracking for the selected VRHMD.
/// It also handles processing of the selected control and confirmation method.
/// Supported control methods are eye, head, mouse, 3DRudder foot pedal, Xbox 360 controller, VIVE controller (only when using HTC VIVE HMD).
/// Supported confirmation methods are click, dwell, nod.
/// The click confirmation method is control method dependant. See the second switch (CurrentControlMethod) statement below.
/// </summary>
public class VREyeTrackerController : MonoBehaviour {

    public enum NodDirection { Up, Down }

    public static VREyeTrackerController Instance { get; private set; }

    public TestBlock.ControlMethod CurrentControlMethod { get; private set; }
    public TestBlock.ConfirmationMethod CurrentConfirmationMethod { get; private set; }
    public Vector2 LastHeadGazeScreenPoint { get; private set; }
    public Vector2 CurrentHeadGazeScreenPoint { get; private set; }
    public Vector2 LastEyeGazeScreenPoint { get; private set; }
    public Vector2 CurrentEyeGazeScreenPoint { get; private set; }
    public float TotalHeadMovement { get; private set; }
    public Camera VRCamera { get; private set; }
    public List<Camera> Eyes { get; private set; }

    [SerializeField] private float _nodThreshold = 0.5f;
    [SerializeField] private bool _drawRay;
    [SerializeField] private Camera _canvasCamera;

    private delegate void Nodded(NodDirection direction);
    private event Nodded HasNodded;
    private delegate void ClickDown();
    private event ClickDown HasClickedDown;
    private delegate void ClickUp();
    private event ClickUp HasClickedUp;
    private const float POINT_CALCULATION_DISTANCE = 1;

    private TestController _testController;
    private GazeCursor _gazeCursor;
    private List<Transform> _viveControllerTransforms;
    private EyeGazeTarget _currentHoveredTarget;
    private Vector3 _currentHeadVelocity;
    private Vector3 _lastHeadVelPoint;
    private Vector3 _rayOrigin;
    private Quaternion _lastHeadRotation;
    private bool _nodCooldown;
    private bool _isTrackingMovement;
    private bool _waitingForGesture;
    private bool _viveTriggerPressed;
    private float _velCheckDistance = 5;
    private Ray[] _eyeRayCache;
    private int _eyeRayCachePointer;
    private LineRenderer _lineRenderer;
    private PupilGazeTracker _pupilTracker;
    private FoveInterface _foveInterface;
    private ns3DRudder.ModeAxis _modeAxis = ns3DRudder.ModeAxis.NormalizedValueNonSymmetricalPitch;
    private Connector connector;

    protected Rudder Rudder;
    protected ns3DRudder.Axis Axis;

    void Awake() {
        VRCamera = GetComponent<Camera>();
        Instance = this;
        Cursor.visible = false;
        _lineRenderer = GetComponent<LineRenderer>();
        _pupilTracker = FindObjectOfType<PupilGazeTracker>();
        _foveInterface = GetComponent<FoveInterface>();
        _viveControllerTransforms = new List<Transform>();
        _viveControllerTransforms.Add(transform.root.Find("Controller (right)"));
        //_viveControllerTransforms.Add(transform.root.Find("Controller (left)"));
        if (_drawRay)
            _lineRenderer.positionCount = 2;
    }

    void Start() {
        _gazeCursor = GazeCursor.Instance;
        _testController = TestController.Instance;
        Rudder = s3DRudderManager.Instance.GetRudder(0);
        connector = Connector.getInstance();
        connector.AutoConnect();
        _pupilTracker.SetShowDot(false);
    }

    void FixedUpdate() {
        Vector3 checkPoint = transform.position + transform.forward * _velCheckDistance;
        _currentHeadVelocity = checkPoint - _lastHeadVelPoint;
        _lastHeadVelPoint = checkPoint;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TestController.Instance.StopTest();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(0);
            //Application.Quit();
        }
    }

    //Shut down 3DRudder (foot pedal) thread
    void OnApplicationQuit() {
        s3DRudderManager.Instance.ShutDown();
        if (connector != null)
            connector.Disconnect();
    }

    void Update() {
        //Update head and eye gaze positions
        LastHeadGazeScreenPoint = CurrentHeadGazeScreenPoint;
        LastEyeGazeScreenPoint = CurrentEyeGazeScreenPoint;

        FindGazeOrigin();
        //If recording gaze position, track eyes regardless of control method
        if (_testController.TestBlockData.RecordGazePosition) {
            TrackEyes();
        }
        //Track head direction regardless of control method (for data collection purposes)
        TrackHead();

        //Track the current control method. Eyes and head were taken outside of this switch statement
        //as the experiment required them to be tracked at all times.
        switch (CurrentControlMethod) {
            case TestBlock.ControlMethod.Eyetracking:
                //If not recording gaze position, track eyes
                if (!_testController.TestBlockData.RecordGazePosition)
                    TrackEyes();
                break;
            case TestBlock.ControlMethod.Headtracking:
                //TrackHead();
                break;
            case TestBlock.ControlMethod.Mouse:
                TrackMouse();
                break;
            case TestBlock.ControlMethod.HandPointer:
                TrackHand();
                break;
            case TestBlock.ControlMethod.Joystick:
                TrackJoystick();
                break;
            case TestBlock.ControlMethod.Footpedal:
                TrackFootpedal();
                break;
            case TestBlock.ControlMethod.Wheelchair:
                TrackWheelchair();
                break;
            case TestBlock.ControlMethod.Finger:
                TrackFinger();
                break;
        }

        //If using nod as confirmation method, check for nod
        if (_waitingForGesture) {
            DetectNod();
            return;
        }
        
        //Track total head movement
        if (_isTrackingMovement) {
            TrackHeadMovement();
        }
        
        if (HasClickedUp == null || HasClickedDown == null)
            return;

        //If using click as activation method, this switch statement handles clicks up and down for the relevant control method.
        //For mouse, click is the left or right mouse button.
        //For the VIVE controller, click is the trigger button on the bottom of the controller.
        //For the Xbox 360 joystick, click is any of the four colored buttons (A, B, X, Y).
        //For any other control method, click is set up to be any from left/right mouse button, A, B, X, Y on Xbox 360 controller or space bar on a keyboard.
        switch (CurrentControlMethod) {
            case TestBlock.ControlMethod.Eyetracking:
                if (AnyInputUp())
                    HasClickedUp();
                if (AnyInputDown())
                    HasClickedDown();
                break;
            case TestBlock.ControlMethod.Headtracking:
                if (AnyInputUp())
                    HasClickedUp();
                if (AnyInputDown())
                    HasClickedDown();
                break;
            case TestBlock.ControlMethod.Joystick:
                if (Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp(KeyCode.JoystickButton1) ||
                    Input.GetKeyUp(KeyCode.JoystickButton2) || Input.GetKeyUp(KeyCode.JoystickButton3))
                    HasClickedUp();
                if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.JoystickButton1) ||
                    Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.JoystickButton3))
                    HasClickedDown();
                break;
            case TestBlock.ControlMethod.Footpedal:
                if (AnyInputUp())
                    HasClickedUp();
                if (AnyInputDown())
                    HasClickedDown();
                break;
            case TestBlock.ControlMethod.Mouse:
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                    HasClickedUp();
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                    HasClickedDown();
                break;
            case TestBlock.ControlMethod.HandPointer:
                //NOTE: This is hardcoded to one controller at the moment
                if (!_viveTriggerPressed && _viveControllerTransforms[0]
                        .GetComponent<SteamVR_TrackedController>()
                        .triggerPressed)
                {
                    _viveTriggerPressed = true;
                    HasClickedDown();
                }
                if (_viveTriggerPressed && !_viveControllerTransforms[0]
                        .GetComponent<SteamVR_TrackedController>()
                        .triggerPressed)
                {
                    _viveTriggerPressed = false;
                    HasClickedUp();
                }
                break;
            case TestBlock.ControlMethod.Wheelchair:
                if (AnyInputUp())
                    HasClickedUp();
                if (AnyInputDown())
                    HasClickedDown();
                break;
        }
    }

    /// <summary>
    /// When using the TestLoader scene to setup an experiment, this function fetches some parameters of importance from the block that was generated.
    /// </summary>
    /// <param name="testBlock"> </param>
    public void LoadTestData(TestBlock testBlock) {
        /*Eyes = new List<Camera>();
        switch (testBlock.SelectedVRHMD)
        {
            case TestBlock.VRHMD.VIVE:
                Eyes.Add(VRCamera);
                break;
            case TestBlock.VRHMD.FOVE:
                Eyes.Add(_foveInterface.GetEyeCamera(EFVR_Eye.Left));
                Eyes.Add(_foveInterface.GetEyeCamera(EFVR_Eye.Right));
                break;
        }*/
        GazeCursor.Instance.SetColor(testBlock.CursorColor);

        CurrentControlMethod = testBlock.SelectedControlMethod;
        CurrentConfirmationMethod = testBlock.SelectedConfirmationMethod;

        GazeCursor.Instance.TargetHovered += OnCursorHover;
        GazeCursor.Instance.TargetStayed += OnCursorStay;
        GazeCursor.Instance.TargetUnhovered += OnCursorUnhover;

        if (CurrentConfirmationMethod == TestBlock.ConfirmationMethod.Click) {
            HasClickedDown += OnClickDown;
            HasClickedUp += OnClickUp;
        }
        _eyeRayCache = new Ray[Mathf.Clamp(testBlock.EyeSmoothFactor, 1, int.MaxValue)];
    }

    /// <summary>
    /// Track total head movement based on the difference in head rotations between frames.
    /// </summary>
    private void TrackHeadMovement() {
        TotalHeadMovement += Quaternion.Angle(_lastHeadRotation, transform.rotation);
        _lastHeadRotation = transform.rotation;
    }

    /// <summary>
    /// Used when mouse is the selected control method.
    /// Updates cursor position on canvas based on mouse axis input.
    /// </summary>
    private void TrackMouse() {
        Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _gazeCursor.SetPositionFromMouse(mouse.x, mouse.y);
    }

    private void TrackFinger()
    {
#if !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            _gazeCursor.SetPositionFromFinger(touch.position.x, touch.position.y);
        }
#else
        TrackMouse();
#endif
    }

    /// <summary>
    /// Update cursor position on canvas based on a ray originating at the VR headset's position and using the headset's forward direction.
    /// </summary>
    private void TrackHead() {
        Vector3 direction = VRCamera.transform.forward;
        Ray ray = new Ray(_rayOrigin, direction);
        CurrentHeadGazeScreenPoint = VRCamera.WorldToScreenPoint(_rayOrigin + ray.direction * POINT_CALCULATION_DISTANCE);
        HandleHeadRay(ray);
    }

    /// <summary>
    /// Update cursor position on canvas based on xbox joystick axis input.
    /// </summary>
    private void TrackJoystick() {
        Vector2 joystick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _gazeCursor.SetPositionFromJoystick(joystick.x, joystick.y);
    }

    /// <summary>
    /// Update cursor position on canvas based on footpedal axis input.
    /// Since the experiment is 2D, it only requires two axes from the pedal so this control method is mechanically identical to the xbox joystick method.
    /// </summary>
    private void TrackFootpedal() {
        Axis = Rudder.GetAxis(_modeAxis);
        _gazeCursor.SetPositionFromJoystick(-Axis.GetXAxis(), -Axis.GetYAxis());
    }

    /// <summary>
    /// Update cursor position on canvas based on wheelchair wheel inputs.
    /// This control method uses an in-house developed wheelchair simulator to show that custom-built controllers can work as well.
    /// </summary>
    private void TrackWheelchair()
    {
        Movement m = connector.Collect();
        _gazeCursor.SetPositionFromMouse(m.RightTicks, m.LeftTicks);
    }

    /// <summary>
    /// Update cursor position on screen based on the eye gaze ray from VRHMD (if using eyetracking as control method). 
    /// This function is also called if RecordGazePosition is true, in which case the ray is still handled but the cursor position is not updated.
    /// This is in case we want to record gaze position regardless of using eyetracking as control method.
    /// </summary>
    private void TrackEyes() {
        List<Vector3> eyeDirections = new List<Vector3>();

        switch (TestController.Instance.TestBlockData.SelectedVRHMD) {
            case TestBlock.VRHMD.VIVE:
                Vector3 gaze = Pupil.values.GazePoint3D;
                //Transform and correct eye-tracking
                gaze = (transform.rotation * gaze).normalized;
                Vector3 delta = transform.forward.normalized - gaze;
                gaze = gaze + delta * 2;
                //float eyeConfidence = (Pupil.values.Confidences[0] + Pupil.values.Confidences[1]) / 2.0f;
                //if (eyeConfidence > 0.7f)
                //{
                    eyeDirections.Add(gaze);
                //}
                break;

            case TestBlock.VRHMD.FOVE:
                FoveInterface.EyeRays rays = _foveInterface.GetGazeRays();
                EFVR_Eye eyeClosed = FoveInterface.CheckEyesClosed();
                switch (eyeClosed) {
                    case (EFVR_Eye.Neither):
                        eyeDirections.Add(rays.left.direction);
                        eyeDirections.Add(rays.right.direction);
                        break;
                    case (EFVR_Eye.Left):
                        eyeDirections.Add(rays.right.direction);
                        break;
                    case (EFVR_Eye.Right):
                        eyeDirections.Add(rays.left.direction);
                        break;
                    case (EFVR_Eye.Both):
                        eyeDirections.Add(Vector3.zero);
                        break;
                }
                break;
        }

        Vector3 direction = Vector3.zero;

        foreach (Vector3 eyeDirection in eyeDirections) {
            direction += eyeDirection;
        }
        direction = direction / eyeDirections.Count;
        Ray ray = new Ray(_rayOrigin, direction);
        ray = GetAverageEyeRay(ray);
        CurrentEyeGazeScreenPoint = VRCamera.WorldToScreenPoint(_rayOrigin + ray.direction * POINT_CALCULATION_DISTANCE);
        if(CurrentControlMethod == TestBlock.ControlMethod.Eyetracking)
        HandleRay(ray);
        else
        HandleGazeTrackingRay(ray);
        Debug.DrawRay(ray.origin, ray.direction * 100);

    }

    /// <summary>
    /// This function handles updates when using the VIVE controller as control method.
    /// Update cursor position on canvas based on a ray originating at the VIVE controller's position and using the  its' forward direction.
    /// </summary>
    private void TrackHand() {
        foreach (Transform controllerTransform in _viveControllerTransforms) {
            if (controllerTransform.gameObject.activeSelf) {
                Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
                HandleRay(ray);
                Debug.DrawRay(ray.origin, ray.direction * 100);
            }
        }
    }

    private void OnCursorHover(EyeGazeTarget target)
    {
        OnCursorStay(target);
    }

    private void OnCursorStay(EyeGazeTarget target) 
    {
        HandleTrackedObject(target);
    }

    private void OnCursorUnhover(EyeGazeTarget target) 
    {
        HandleTrackedObject(null);
    }

    /// <summary>
    /// Function for handling ray-casting control methods (eye-tracking and VIVE-controller).
    /// For any method, if the ray hits, update the cursor's position to the hit point.
    /// If eye-tracking, ensure that gaze position is also updated. 
    /// </summary>
    /// <param name="ray"> The passed ray to check.</param>
    private void HandleRay(Ray ray) {
        if (_drawRay)
            _lineRenderer.SetPositions(new Vector3[] { _rayOrigin, _rayOrigin + ray.direction * 100 });
        Debug.DrawRay(ray.origin, ray.direction * 100);
        RaycastHit hit;
    
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity)) {
                GazeCursor.Instance.SetPositionFromRay(hit, _canvasCamera);
            if (CurrentControlMethod == TestBlock.ControlMethod.Eyetracking)
                GazeCursor.Instance.SetGazePositionFromRay(hit, _canvasCamera);
        }
    }

    /// <summary>
    /// Function for handling ray-casts from head.
    /// If the ray hits, update head position to the hit point.
    /// If head-tracking, ensure that cursor position is also updated.
    /// </summary>
    /// <param name="ray"> The passed ray to check.</param>
    private void HandleHeadRay(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity)) {
            GazeCursor.Instance.SetHeadPositionFromRay(hit, _canvasCamera);
            if (CurrentControlMethod == TestBlock.ControlMethod.Headtracking)
                GazeCursor.Instance.SetPositionFromRay(hit, _canvasCamera);
        }
    }

    /// <summary>
    /// This function's purpose is to ensure the gaze position is updated if the ray hits.
    /// It does not update the visible cursor on the canvas. 
    /// </summary>
    /// <param name="ray"> The passed ray to check.</param>
    private void HandleGazeTrackingRay(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity)) {
            GazeCursor.Instance.SetGazePositionFromRay(hit, _canvasCamera);
        }
    }

    /// <summary>
    /// Handles possible interactions when the subscribed cursor is within a target. 
    /// This includes contingencies like the gazeTarget being null, subscribing to the functions that wait
    /// for a nod or dwell time and calling the DeselectHoveredObject to handle unsubscribing.
    /// </summary>
    /// <param name="gazeTarget"></param>
    private void HandleTrackedObject(EyeGazeTarget gazeTarget) {
        if (gazeTarget == null) {
            DeselectHoveredObject();
            return;
        }
        if (!gazeTarget.IsDisabled) {
            if (gazeTarget != _currentHoveredTarget) {
                if (_currentHoveredTarget != null)
                    DeselectHoveredObject();
                _currentHoveredTarget = gazeTarget;
                switch (CurrentConfirmationMethod) {
                    case TestBlock.ConfirmationMethod.Dwell:
                        _currentHoveredTarget.OnDwell += WaitForDwell;
                        break;
                    case TestBlock.ConfirmationMethod.HeadGesture:
                        HasNodded += OnNod;
                        _waitingForGesture = true;
                        break;
                }
            }
            gazeTarget.OnEyeIn();
        }
    }

    /// <summary>
    /// Handles unsubscribing when cursor leaves target.
    /// </summary>
    private void DeselectHoveredObject() {
        if (_currentHoveredTarget == null) return;
        switch (CurrentConfirmationMethod) {
            case TestBlock.ConfirmationMethod.Dwell:
                _currentHoveredTarget.OnDwell -= WaitForDwell;
                break;
            case TestBlock.ConfirmationMethod.HeadGesture:
                HasNodded -= OnNod;
                break;
        }
        _currentHoveredTarget.OnEyeOut();
        _currentHoveredTarget = null;
    }

    /// <summary>
    /// Determines if the user has nodded by looking at the head's vertical velocity and comparing that to a threshold.
    /// </summary>
    //TODO: Further testing and _nodThreshold adjustments with Headtracking required
    private void DetectNod() {
        List<float> velocities = new List<float>();
        if (CurrentConfirmationMethod == TestBlock.ConfirmationMethod.HeadGesture)
            velocities.Add(_currentHeadVelocity.y);

        foreach (float velocity in velocities) {
            Vector3 tempVel = new Vector3(0, velocity, 0);
            if (tempVel.magnitude > _nodThreshold && !_nodCooldown) {
                if (HasNodded != null)
                    HasNodded(tempVel.y > 0 ? NodDirection.Up : NodDirection.Down);
                _nodCooldown = true;
                return;
            }
            else if (tempVel.magnitude < _nodThreshold)
                _nodCooldown = false;
        }
    }

    /// <summary>
    /// When using nod confirmation method, activate the target and stop detecting nod.
    /// </summary>
    /// <param name="direction"> The direction (Up/Down) that the user nodded.</param>
    private void OnNod(NodDirection direction) {
        //TODO: Where do we define which direction should be nodded?
        if (_currentHoveredTarget == null) return;
        HasNodded -= OnNod;
        _currentHoveredTarget.Activate();
        _waitingForGesture = false;
    }

    /// <summary>
    /// Handles user clicking and holding down the mouse/joystick button.
    /// </summary>
    private void OnClickDown() {
        if (!_testController.IsRunning || _currentHoveredTarget == null) return;
        _currentHoveredTarget.OnHalfActivate();
    }

    /// <summary>
    /// Handles user releasing clicked mouse/joystick button.
    /// </summary>
    private void OnClickUp() {
        if (!_testController.IsRunning) return;
        _testController.CurrentTarget.Activate();
    }

    /// <summary>
    /// Handles user releasing any of the listed keys during this frame.
    /// </summary>
    /// <returns>Returns true if the user has released any of the listed keys.</returns>
    private bool AnyInputUp()
    {
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)
            || Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp(KeyCode.JoystickButton1) ||
            Input.GetKeyUp(KeyCode.JoystickButton2) || Input.GetKeyUp(KeyCode.JoystickButton3))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Handles user pressing any of the listed keys during this frame.
    /// </summary>
    /// <returns>Returns true if the user has pressed any of the listed keys.</returns>
    private bool AnyInputDown() {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)
            || Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.JoystickButton1) ||
            Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.JoystickButton3)) {
            return true;
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// For the dwell confirmation method, waits until the current dwell time exceeds the block's assigned dwell time, 
    /// then unsubscribes from OnDwell and activates the target.
    /// </summary>
    /// <param name="currentDwellTime"> The current dwell time.</param>
    private void WaitForDwell(float currentDwellTime) {
        if (currentDwellTime >= _testController.TestBlockData.DwellTime / 1000f) {
            _currentHoveredTarget.OnDwell -= WaitForDwell;
            _currentHoveredTarget.Activate();
        }
    }
    
    public void StartTrackingMovement() {
        _isTrackingMovement = true;
        TotalHeadMovement = 0;
        _lastHeadRotation = transform.rotation;
    }

    public void StopTrackingMovement() {
        _isTrackingMovement = false;
    }

    /// <summary>
    /// Get the average ray direction of the rays in _eyeRayCache.
    /// </summary>
    /// <returns> Returns a new ray with the average direction.</returns>
    public Ray GetAverageEyeRay() {
        Vector3 averageDirection = Vector3.zero;
        foreach (Ray ray in _eyeRayCache) {
            if (ray.Equals(null)) continue;
            averageDirection += ray.direction;
        }
        averageDirection /= _eyeRayCache.Length;
        return new Ray(_rayOrigin, averageDirection);
    }

    /// <summary>
    /// Updates the ray cache with a new ray.
    /// </summary>
    /// <param name="newRay"> The ray to be added to the cache.</param>
    /// <returns> Calls GetAverageEyeRay() to return the new average ray.</returns>
    public Ray GetAverageEyeRay(Ray newRay)
    {
        _eyeRayCachePointer++;
        if (_eyeRayCachePointer == _eyeRayCache.Length)
            _eyeRayCachePointer = 0;
        _eyeRayCache[_eyeRayCachePointer] = newRay;
        return GetAverageEyeRay();
    }

    /// <summary>
    /// Sets the _rayOrigin variable based on the used VRHMD.
    /// </summary>
    private void FindGazeOrigin()
    {
        switch (TestController.Instance.TestBlockData.SelectedVRHMD)
        {
            case TestBlock.VRHMD.VIVE:
                _rayOrigin = transform.position;
                break;
            case TestBlock.VRHMD.FOVE:
                if (CurrentControlMethod == TestBlock.ControlMethod.Eyetracking)
                {
                    FoveInterface.EyeRays rays = _foveInterface.GetGazeRays();
                    EFVR_Eye eyeClosed = FoveInterface.CheckEyesClosed();
                    switch (eyeClosed)
                    {
                        case (EFVR_Eye.Neither):
                            _rayOrigin = (rays.left.origin + rays.right.origin) * 0.5f;
                            break;
                        case (EFVR_Eye.Left):
                            _rayOrigin = rays.right.origin;
                            break;
                        case (EFVR_Eye.Right):
                            _rayOrigin = rays.left.origin;
                            break;
                        case (EFVR_Eye.Both):
                            _rayOrigin = Vector3.zero;
                            break;
                    }

                }
                else if (CurrentControlMethod == TestBlock.ControlMethod.Headtracking)
                {
                    _rayOrigin = transform.position;
                }
                break;
        }
    }
}
