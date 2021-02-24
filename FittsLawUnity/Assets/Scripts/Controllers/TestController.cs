using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pupil;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
using DxCalculationSet = TestDataHelper.DxCalculationSet;

public class TestController : MonoBehaviour
{
    private static TestBlock _storedTestBlock;

    [SerializeField] private bool _useDB = false;
    [SerializeField] private float _canvasDistanceToCamera = 0.5f;
    [SerializeField] private GameObject _eyeGazeTargetPrefab;
    [SerializeField] private GameObject _eyeGazeInactiveTargetPrefab;
    [SerializeField] private GameObject _verificationTargetPrefab;
    [SerializeField] private Canvas _targetCanvas;
    [SerializeField] private Transform _htcVive;
    [SerializeField] private Transform _fove;
    [SerializeField] private Transform _pcController;
    [SerializeField] private Text _errorThresholdText;
    [SerializeField] private Text _pressStartText;
    [SerializeField] private Camera _viveCanvasCamera;
    [SerializeField] private Camera _foveCanvasCamera;

    public static TestController Instance;
    public bool IsRunning { get; private set; }
    public TestBlock TestBlockData { get; private set; }
    public EyeGazeTarget CurrentTarget { get; private set; }

    public float VerticalShift { get; private set; }

    private DBController _dbController;
    private VREyeTrackerController _vrEyeTrackerController;
    private PupilGazeTracker _pupilGazeTracker;
    private AudioSource _audioSourceBeep;
    private int _currentTrialNumber;
    private int _trialIndex;
    private int _sequenceIndex = -1;
    private int _numberOfErrors;
    private int _repeatedFailedSessions;
    private int _totalCursorMovement;
    private int _targetLifetime = 3;
    private int _numberOfVerificationTargets = 9;
    private int _verificationTargetIndex = 0;
    private double _startMilli;
    private bool _cameraColorSet;
    private bool _logData;
    private bool _firstTarget;
    private bool _showCursor;
    private bool _verificationRunning;
    private float _timeoutTimer;
    private Vector2 _lastCursorPosition;
    private List<EyeGazeTarget> _currentTargetOutlines;
    private List<TestVerification> _testVerifications;
    private Vector2 _activeVerificationTargetCoordinates;

    private const float DOUBLE_TAP_DELAY_SECONDS = 1f;
    private float pauseStartTime = 0f;

    private bool startSequenceClicked = false;

    void Awake()
    {
        Instance = this;
        _audioSourceBeep = GetComponent<AudioSource>();
        _currentTargetOutlines = new List<EyeGazeTarget>();
        _testVerifications = new List<TestVerification>();
    }

    void Start () {
        _dbController = DBController.Instance;
        _pupilGazeTracker = FindObjectOfType<PupilGazeTracker>();
        //If data is stored from TestLoader scene, use that
        if (_storedTestBlock != null)
            LoadTestData(_storedTestBlock);
        else
        //If you need to run this scene repeatedly to test, create TestBlockData and load with LoadTestData(TestBlockData) and play project from MainScene
            LoadTestData(new TestBlock(TestBlock.VRHMD.NoHMD, "TestBlockData", "TestParticipant", "TestCondition", 11, new List<int>() {80}, new List<float>() { 50, 50 }, 100, 2, 
                TestBlock.ControlMethod.Mouse, TestBlock.ConfirmationMethod.Click, 200, 80000, 5, 12, false, true, true, true, true, false,
                new Color32(67, 67, 67, 255), Color.yellow, Color.black, new Color32(200, 200, 200, 255), new Color32(128, 128, 128, 255), Color.red));

        VerticalShift = (Screen.height / 2) - TestBlockData.TargetAmplitudes.Max() - (TestBlockData.TargetDiameters.Max() / 2);
#if !UNITY_EDITOR
        VerticalShift = -VerticalShift;
#endif

        Helper.SendConditionBroadcast();

#if UNITY_EDITOR
        StartCoroutine(InputListener());
#endif

        _vrEyeTrackerController.StartTrackingMovement();
        Cursor.lockState = CursorLockMode.Locked;
        GazeCursor.Instance.SetEnabled(_showCursor);

        SetPause();
    }

    private void SetPause()
    {
        if (StudyManager.Instance.StudyNumber == StudyManager.StudyEnum.Study1
            && int.Parse(StudyManager.Instance.BlockId.Substring(1)) == (StudyManager.Instance.Blocks.Count / 2) + 1
            && _sequenceIndex + 2 == 1)
        {
            _pressStartText.text =
                "First part of the study is over" +
                "\nPut out the VR Box and fill the Google Form\n" +
                "\nAfter that, continue and double tap to start the next condition sequence";
        }
        else
        {
            _pressStartText.text = "Double tap to start sequence" +
                "\nStudy number: " + StudyManager.Instance.StudyNumber +
                "\nCondition: " + StudyManager.Instance.Condition +
                "\nScale factor: " + StudyManager.Instance.ScaleFactor +
                "\nBlock: " + StudyManager.Instance.BlockId +
                "\nSequence: " + (_sequenceIndex + 2);
        }

        _pressStartText.enabled = true;
        pauseStartTime = Time.time;
    }

    private float doubleClickTimeLimit = 0.5f;
    private float variancePosition = 10;

    // Update is called once per frame
    private IEnumerator InputListener()
    {
        while (enabled)
        { //Run as long as this is activ

            if (Input.GetMouseButtonDown(0))
                yield return ClickEvent();

            yield return null;
        }
    }

    private IEnumerator ClickEvent()
    {
        //pause a frame so you don't pick up the same mouse down event.
        yield return new WaitForEndOfFrame();
        float count = 0f;
        while (count < doubleClickTimeLimit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoubleClick();
                yield break;
            }
            count += Time.deltaTime;// increment counter by change in time between frames
            yield return null; // wait for the next frame
        }
        SingleClick();
    }


    private void SingleClick()
    {
    }

    private void DoubleClick()
    {
        if (!IsRunning && Time.time - pauseStartTime > DOUBLE_TAP_DELAY_SECONDS)
        {
            startSequenceClicked = true;
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).tapCount > 1)
        {
            DoubleClick();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton7) || startSequenceClicked)
        {
            startSequenceClicked = false;
            StartTest();
        }
        if (Input.GetKeyDown(KeyCode.T) && !_verificationRunning && !IsRunning)
            StartVerification();
        if (_verificationRunning)
            VerificationStep();
        if (!IsRunning || _firstTarget) return;

        if (_logData)
        {
            double? pupilDiameterLeft = null;
            double? pupilDiameterRight = null;
            double? pupilDiameter3DLeft = null;
            double? pupilDiameter3DRight = null;
            double? pupilConfidenceLeft = null;
            double? pupilConfidenceRight = null;
            double? pupilTimestampLeft = null;
            double? pupilTimestampRight = null;
            float? hmdPositionX = null;
            float? hmdPositionY = null;
            float? hmdPositionZ = null;
            float? hmdRotationX = null;
            float? hmdRotationY = null;
            float? hmdRotationZ = null;
            if (TestBlockData.SelectedVRHMD == TestBlock.VRHMD.VIVE)
            {
                pupilConfidenceLeft = Pupil.values.Confidences[0];
                pupilConfidenceRight = Pupil.values.Confidences[1];
                pupilDiameter3DLeft = Pupil.values.BaseData[0].diameter_3d;
                pupilDiameter3DRight = Pupil.values.BaseData[1].diameter_3d;
                pupilDiameterLeft = Pupil.values.BaseData[0].diameter;
                pupilDiameterRight = Pupil.values.BaseData[1].diameter;
                pupilTimestampLeft = Pupil.values.BaseData[0].timestamp;
                pupilTimestampRight = Pupil.values.BaseData[1].timestamp;
                Vector3 htcPosition = _htcVive.Find("Camera (eye)").position;
                Vector3 htcRotation = _htcVive.Find("Camera (eye)").rotation.eulerAngles;
                hmdPositionX = htcPosition.x;
                hmdPositionY = htcPosition.y;
                hmdPositionZ = htcPosition.z;
                hmdRotationX = htcRotation.x;
                hmdRotationY = htcRotation.y;
                hmdRotationZ = htcRotation.z;
            }
            //TODO: _fove is the outside object, might need to use correct child to fetch position, like VIVE
            else if (TestBlockData.SelectedVRHMD == TestBlock.VRHMD.FOVE)
            {
                hmdPositionX = _fove.position.x;
                hmdPositionY = _fove.position.y;
                hmdPositionZ = _fove.position.z;
            }
            TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].LogData(GetNowInMilliseconds() - _startMilli, GazeCursor.Instance.Position, GazeCursor.Instance.GazePosition,
               pupilDiameterLeft, pupilDiameterRight, pupilDiameter3DLeft, pupilDiameter3DRight, pupilConfidenceLeft, pupilConfidenceRight, pupilTimestampLeft, pupilTimestampRight, _vrEyeTrackerController.TotalHeadMovement,
               GazeCursor.Instance.HeadPosition, hmdPositionX, hmdPositionY, hmdPositionZ, hmdRotationX, hmdRotationY, hmdRotationZ);
        }

        _timeoutTimer += Time.deltaTime;
        if (_timeoutTimer >= TestBlockData.Timeout/1000f)
            TargetTimeout();
    }

    private void FixedUpdate()
    {
        if (IsRunning && !_firstTarget)
        {
            TrackCursorMovement();
        }

        //FOVE Cameras don't exist on Start and needs to be set in runtime
        if (_cameraColorSet ) return;
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            cam.backgroundColor = TestBlockData.BackgroundColor;
            _cameraColorSet = true;
        }
    }

    public void LoadTestData(TestBlock testBlock)
    {
        TestBlockData = testBlock;

        if (TestBlockData.SelectedVRHMD == TestBlock.VRHMD.VIVE)
        {
            _htcVive.gameObject.SetActive(true);
            _vrEyeTrackerController = _htcVive.GetComponentInChildren<VREyeTrackerController>();
        }
        else if (TestBlockData.SelectedVRHMD == TestBlock.VRHMD.FOVE) 
        {
            _fove.gameObject.SetActive(true);
            _vrEyeTrackerController = _fove.GetComponentInChildren<VREyeTrackerController>();
        }
        else if (TestBlockData.SelectedVRHMD == TestBlock.VRHMD.NoHMD)
        {
            _pcController.gameObject.SetActive(true);
            _vrEyeTrackerController = _pcController.GetComponent<VREyeTrackerController>();
        }
        _targetCanvas.worldCamera = _vrEyeTrackerController.VRCamera;
        _targetCanvas.planeDistance = _canvasDistanceToCamera;
        _showCursor = TestBlockData.ShowCursor;
        GazeCursor.Instance.SetEnabled(_showCursor);
        _vrEyeTrackerController.LoadTestData(this.TestBlockData);
        GazeCursor.Instance.MouseSensitivity = TestBlockData.MouseSensivity;
        if(TestBlockData.RecordGazePosition)
        //Starts VIVE pupil capture software regardless of control condition
        _pupilGazeTracker.enabled = true && TestBlockData.SelectedVRHMD == TestBlock.VRHMD.VIVE;
        else
        //Starts only if control method is eyetracking with VIVE headset
        _pupilGazeTracker.enabled = (TestBlockData.SelectedControlMethod == TestBlock.ControlMethod.Eyetracking)
        && TestBlockData.SelectedVRHMD == TestBlock.VRHMD.VIVE;
    }

    public void StartTest()
    {
        if (TestBlockData == null)
        {
            Debug.LogError("TestBlockData has not been loaded. Ensure TestBlockData is loaded before starting test.");
            return;
        }
        if (IsRunning) return;
        _errorThresholdText.enabled = false;
        _pressStartText.enabled = false;

        _sequenceIndex++;
        if (_sequenceIndex >= TestBlockData.Sequences.Count) return;
        Cursor.lockState = CursorLockMode.Locked;
        GazeCursor.Instance.SetEnabled(_showCursor);
        _trialIndex = -1;
        _timeoutTimer = 0;
        _numberOfErrors = 0;
        IsRunning = true;
        _firstTarget = true;
        //_targetCanvas.renderMode = RenderMode.WorldSpace;
        if (TestBlockData.SelectedVRHMD == TestBlock.VRHMD.VIVE)
            _viveCanvasCamera.transform.parent = null;
        if(TestBlockData.SelectedVRHMD == TestBlock.VRHMD.FOVE)
            _foveCanvasCamera.transform.parent = null;
        _currentTrialNumber = -1;
        SpawnInitialTargets(_sequenceIndex);
        TestBlockData.Sequences[_sequenceIndex].StartTime = DateTime.Now;
        Debug.Log("----Test Block Started-----");
        Debug.Log("----Test Sequence Started-----");
        NextStep();
    }

    public void StopTest()
    {
        if (!IsRunning) return;
        IsRunning = false;
        Debug.Log("----Test Sequence Ended----");
        Debug.Log("SequenceIndex: " + _sequenceIndex + "; SequecneCount: " + TestBlockData.Sequences.Count);
        if (_sequenceIndex >= TestBlockData.Sequences.Count - 1)
        {
            Debug.Log("----Test Block Ended----");
            EvaluateEndedBlock();
        }
        else
        {
            StudyManager.Instance.SaveLogFiles();
            SetPause();
        }
    }

    private void EvaluateEndedSequence()
    {
        if ((float)_numberOfErrors / TestBlockData.NumberOfTargets > TestBlockData.ErrorThreshold / 100f)
        {
            Debug.Log("----Number of errors exceeds error threshold - Restarting current test sequence----");
            TestBlockData.Sequences[_sequenceIndex].SequenceOfRepeats++;
            _sequenceIndex--;
            _errorThresholdText.enabled = true;
            ClearInitialTargets();
            StopTest();
        }
        else
        {
            Debug.Log("----Test Sequence Completed Successfully----");
            //Calculate result data for sequence just completed
            List<double> dxs = new List<double>();
            List<double> aes = new List<double>();

            for (int i = 0; i < TestBlockData.Sequences[_sequenceIndex].Trials.Count; i++) 
            {
                DxCalculationSet calcSet = new DxCalculationSet();
                TestTrial fromTarget = i == 0 ? 
                    TestBlockData.Sequences[_sequenceIndex].Trials[TestBlockData.Sequences[_sequenceIndex].Trials.Count - 1] : 
                    TestBlockData.Sequences[_sequenceIndex].Trials[i-1];

                TestTrial toTarget = TestBlockData.Sequences[_sequenceIndex].Trials[i];

                calcSet.From = GetTargetSpawnPosition(TestBlockData.Sequences[_sequenceIndex].TargetAmplitude, fromTarget.TargetAngle);
                calcSet.To = GetTargetSpawnPosition(TestBlockData.Sequences[_sequenceIndex].TargetAmplitude, toTarget.TargetAngle);
                calcSet.Selection = calcSet.To + toTarget.TargetCenterError;

                double dx = TestDataHelper.CalculateDeltaX(calcSet);
                dxs.Add(dx);

                double a = TestDataHelper.CalculateA(calcSet);

                if (i == 0)
                    aes.Add(a + dx);
                else
                    aes.Add(a + dx + dxs[i -1]);
            }

            TestBlockData.Sequences[_sequenceIndex].Throughput = TestDataHelper.CalculateThroughput(aes, dxs, TestBlockData.Sequences[_sequenceIndex].GetMovementTimes());
            TestBlockData.Sequences[_sequenceIndex].CalculateMeanMovementTime();
            TestBlockData.Sequences[_sequenceIndex].Errors = _numberOfErrors;
            TestBlockData.Sequences[_sequenceIndex].CalculateErrorRate();
            TestBlockData.Sequences[_sequenceIndex].EffectiveAmplitude = TestDataHelper.Mean(aes);
            TestBlockData.Sequences[_sequenceIndex].EffectiveTargetWidth = TestDataHelper.CalculateEffectiveWidth(dxs);
            TestBlockData.Sequences[_sequenceIndex].EffecttiveIndexOfDifficulty = TestDataHelper.CalculateEffectiveDifficultyIndex(aes, TestBlockData.Sequences[_sequenceIndex].EffectiveTargetWidth);

            StudyManager.Instance.AddLogDetail(TestBlockData, _sequenceIndex);
            
            ClearInitialTargets();
            StopTest();
        }
    }

    private void EvaluateEndedBlock()
    {
        //Calculates a throughput for the block as an average of the throughputs of all sequences
        double throughputTotal = 0;
        foreach (TestSequence sequence in TestBlockData.Sequences)
        {
            throughputTotal += sequence.Throughput;
        }
        TestBlockData.Throughput = throughputTotal / TestBlockData.Sequences.Count;
        //Calculates a throughput for the block using all sequences' and trials' center errors and movement times
        //TestBlockData.Throughput = TestDataHelper.CalculateThroughput(TestBlockData.TargetAmplitudes,
        //    TestDataHelper.CalculateDeltaX(TestBlockData.GetTrialCenterError()), TestBlockData.GetMovementTimes());
        TestBlockData.CalculateMeanMovementTime();
        TestBlockData.CalculateErrorRate();

        //if (_useDB)
        //{
        //    SaveToDb();
        //    Debug.Log("----Test Data Saved To Database----");
        //}

        StudyManager.Instance.AddLogGeneral(TestBlockData);
        Debug.Log("Evaluating block ended");
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
        //SendToEmail();
        StudyManager.Instance.StartChangeBlock();
    }

    private void NextStep()
    {
        if (_firstTarget)
        {
            if (TestBlockData.NumberOfTargets % 2 == 0)
                SpawnTarget(_sequenceIndex, TestBlockData.NumberOfTargets - 2, true);
            else
                SpawnTarget(_sequenceIndex, TestBlockData.NumberOfTargets - 1, true);
            return;
        }
        _timeoutTimer = 0;
        _currentTrialNumber++;
        if (_currentTrialNumber >= TestBlockData.NumberOfTargets)
        {
            EvaluateEndedSequence();
            return;
        }

        _trialIndex++;
        _vrEyeTrackerController.StartTrackingMovement();
        _totalCursorMovement = 0;
        _startMilli = GetNowInMilliseconds();
        SpawnTarget(_sequenceIndex, _trialIndex, true);
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].StartTime = DateTime.Now;
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TrialNumber = _currentTrialNumber;
        _logData = true;
    }

    private void SpawnInitialTargets(int sequcenceIndex)
    {
        for (int i = 0; i < TestBlockData.Sequences[sequcenceIndex].Trials.Count; i++) 
            _currentTargetOutlines.Add(SpawnTarget(sequcenceIndex, i, false));
    }

    private void ClearInitialTargets()
    {
        foreach (EyeGazeTarget target in _currentTargetOutlines)
        {
            Destroy(target.gameObject);
        }
        _currentTargetOutlines = new List<EyeGazeTarget>();
    }

    private EyeGazeTarget SpawnTarget(int sequenceIndex, int stepNumber, bool activeTarget)
    {
        float angle = TestBlockData.Sequences[sequenceIndex].Trials[stepNumber].TargetAngle;
        float amplitude = TestBlockData.Sequences[sequenceIndex].TargetAmplitude;
        float size = TestBlockData.Sequences[sequenceIndex].TargetWidth;
        Vector2 spawnPosition = GetTargetSpawnPosition(amplitude, angle);

        GameObject target = Instantiate(activeTarget ? _eyeGazeTargetPrefab : _eyeGazeInactiveTargetPrefab, Vector3.zero, Quaternion.identity);
        target.transform.SetParent(_targetCanvas.transform, false);
        if (activeTarget)
            target.transform.SetAsFirstSibling();

        EyeGazeTarget eyeGazeTarget = target.GetComponent<EyeGazeTarget>();
        eyeGazeTarget.SetSize(size);
        eyeGazeTarget.SetPosition(spawnPosition.x, spawnPosition.y);
        
        CurrentTarget = target.GetComponent<EyeGazeTarget>();

        CurrentTarget.OnFixate += TargetFixated;
        CurrentTarget.OnActivated += TargetActivated;
        CurrentTarget.OnReleased += TargetReleased;
        eyeGazeTarget.transform.SetAsLastSibling();
        return eyeGazeTarget;
    }

    private void TargetFixated()
    {
        if (_firstTarget) return;
        double milliseconds = GetNowInMilliseconds();
        milliseconds = milliseconds - _startMilli;
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TimeToFixate = milliseconds;
    }

    private void TargetActivated(bool targetHovered)
    {
        Destroy(CurrentTarget.gameObject);
        if (_firstTarget)
        {
            AdvancedStep();
            return;
        }
        _logData = false;
        double milliseconds = GetNowInMilliseconds();
        milliseconds = milliseconds - _startMilli;
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TimeToActivate = milliseconds;
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TargetCenterError = CalculateTargetingError();
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TimedOut = false;
        if (!targetHovered)
            RegisterTargetError();
        AdvancedStep();
    }

    private void TargetTimeout()
    {
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TimeToActivate = TestBlockData.Timeout;
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TargetCenterError = Vector2.zero;
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TimedOut = true;
        Destroy(CurrentTarget.gameObject);
        _numberOfErrors++;
        AdvancedStep();
    }

    private void RegisterTargetError()
    {
        _numberOfErrors++;
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].Error = true;
        if (TestBlockData.BeepOnError)
            _audioSourceBeep.Play();
    }

    private void AdvancedStep()
    {
        if (_firstTarget)
        {
            _firstTarget = false;
            NextStep();
            return;
        }
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TotalHeadMovement = _vrEyeTrackerController.TotalHeadMovement;
        TestBlockData.Sequences[_sequenceIndex].Trials[_currentTrialNumber].TotalCursorMovement = _totalCursorMovement;
        _vrEyeTrackerController.StopTrackingMovement();

        StudyManager.Instance.AddLogTrial(TestBlockData, _sequenceIndex, _currentTrialNumber);

        NextStep();
    }

    private void TargetReleased()
    {
        AdvancedStep();
    }

    private double GetNowInMilliseconds()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        double milli = (System.DateTime.UtcNow - epochStart).TotalMilliseconds;
        return Math.Floor(milli);
    }

    /// <summary>
    /// Calculates target center error
    /// </summary>
    /// <param name="isFromRightToLeft">Is the target being calculated on the left side (TargetAngle > 180)</param>
    /// <returns>Difference between selected point and center of target</returns>
    private Vector2 CalculateTargetingError()
    {
        Vector2 cursorPos = GazeCursor.Instance.transform.localPosition;
        Vector2 targetPos = CurrentTarget.transform.localPosition;
        return cursorPos - targetPos;
    }

    private void TrackCursorMovement()
    {
        _totalCursorMovement += (int)((Vector2) GazeCursor.Instance.transform.localPosition - _lastCursorPosition).magnitude;
        _lastCursorPosition = GazeCursor.Instance.transform.localPosition;
    }

    private void SaveToDb()
    {
        var dataDto = TestBlockData.CreateDTO();
        _dbController.InsertTestResults(dataDto);
    }

    public static void StoreTestData(TestBlock blockToStore)
    {
        _storedTestBlock = blockToStore;
        StudyManager.Instance.initBlock = blockToStore;
    }

    public Vector2 GetTargetSpawnPosition(float amplitude, float angle)
    {
        angle -= 90;
        float x = amplitude * Mathf.Cos(-angle * Mathf.Deg2Rad);
        float y = amplitude * Mathf.Sin(-angle * Mathf.Deg2Rad);

        return new Vector2(x, y + VerticalShift);
    }

    public void StartVerification()
    {
        _verificationRunning = true;
        _verificationTargetIndex = 0;
        ActivateNextTarget();
    }

    public void ActivateNextTarget()
    {
        int auxX = _verificationTargetIndex % 3;
        int auxY = _verificationTargetIndex / 3;
        int targetX = 0;
        int targetY = 0;
        if (auxX == 0)
        {
            targetX = -100;
        }
        else if (auxX == 1)
        {
            targetX = 0;
        }
        else if (auxX == 2)
        {
            targetX = 100;
        }

        if (auxY == 0) {
            targetY = 100;
        }
        else if (auxY == 1) {
            targetY = 0;
        }
        else if (auxY == 2) {
            targetY = -100;
        }
        GameObject target = Instantiate(_verificationTargetPrefab, new Vector3( targetX, targetY, 0), Quaternion.identity);
        _activeVerificationTargetCoordinates = new Vector2(targetX, targetY);
        target.transform.SetParent(_targetCanvas.transform, false);
        StartCoroutine(WaitToDeactivate(target));
    }
    
    //Wait a specified ammount of time before deactivating the target
    IEnumerator WaitToDeactivate(GameObject target) {
        yield return new WaitForSeconds(_targetLifetime);
        DeactivateTarget(target);
    }

    //Deactivate target and call ActivateNextTarget, if it exists, 
    //otherwise transition to the actual typing experiment
    private void DeactivateTarget(GameObject target) {
        target.gameObject.SetActive(false);
        
        if ((_verificationTargetIndex + 1) < _numberOfVerificationTargets)
        {
            _verificationTargetIndex++;
            ActivateNextTarget();
        }
        else {
            Debug.Log("Calibration test ended!");
            _verificationRunning = false;
            //Write to database
            TestBlockData.Verifications = _testVerifications;
        }
    }

    public void VerificationStep()
    {

        double? pupilDiameterLeft = null;
        double? pupilDiameterRight = null;
        double? pupilDiameter3DLeft = null;
        double? pupilDiameter3DRight = null;
        double? pupilConfidenceLeft = null;
        double? pupilConfidenceRight = null;
        double? pupilTimestampLeft = null;
        double? pupilTimestampRight = null;
        float? hmdPositionX = null;
        float? hmdPositionY = null;
        float? hmdPositionZ = null;
        float? hmdRotationX = null;
        float? hmdRotationY = null;
        float? hmdRotationZ = null;
        if (TestBlockData.SelectedVRHMD == TestBlock.VRHMD.VIVE) {
            pupilConfidenceLeft = Pupil.values.Confidences[0];
            pupilConfidenceRight = Pupil.values.Confidences[1];
            pupilDiameter3DLeft = Pupil.values.BaseData[0].diameter_3d;
            pupilDiameter3DRight = Pupil.values.BaseData[1].diameter_3d;
            pupilDiameterLeft = Pupil.values.BaseData[0].diameter;
            pupilDiameterRight = Pupil.values.BaseData[1].diameter;
            pupilTimestampLeft = Pupil.values.BaseData[0].timestamp;
            pupilTimestampRight = Pupil.values.BaseData[1].timestamp;
            Vector3 htcPosition = _htcVive.Find("Camera (eye)").position;
            Vector3 htcRotation = _htcVive.Find("Camera (eye)").rotation.eulerAngles;
            hmdPositionX = htcPosition.x;
            hmdPositionY = htcPosition.y;
            hmdPositionZ = htcPosition.z;
            hmdRotationX = htcRotation.x;
            hmdRotationY = htcRotation.y;
            hmdRotationZ = htcRotation.z;
        }

        _testVerifications.Add(new TestVerification(_verificationTargetIndex, _activeVerificationTargetCoordinates, GazeCursor.Instance.Position, GazeCursor.Instance.GazePosition,
            pupilDiameterLeft, pupilDiameterRight, pupilDiameter3DLeft, pupilDiameter3DRight, pupilConfidenceLeft, pupilConfidenceRight, pupilTimestampLeft, pupilTimestampRight, 
            GazeCursor.Instance.HeadPosition, hmdPositionX, hmdPositionY, hmdPositionZ, hmdRotationX, hmdRotationY, hmdRotationZ));
    }

}
