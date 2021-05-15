using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class contains all data required to make a TestBlockDTO.
/// It stores all parameters the user has selected in the GUI during the TestLoader scene.
/// </summary>
public class TestBlock {

    public enum ControlMethod { Footpedal = 0, Eyetracking = 1, Headtracking = 2, Mouse = 3, HandPointer = 4, Joystick = 5, Wheelchair = 6, Finger = 7}
    public enum ConfirmationMethod { Dwell = 0, Click = 1, HeadGesture = 2}
    public enum VRHMD { FOVE, VIVE, NoHMD }

    //TestSequence data
    public DateTime StartTime;
    public string ParticipantCode;
    public string ConditionCode;
    public string BlockCode;
    public int NumberOfTargets;
    public List<int> TargetAmplitudes;
    public List<float> TargetDiameters;
    public float ErrorThreshold;
    public float SpatialHysteresis;
    public ControlMethod SelectedControlMethod;
    public ConfirmationMethod SelectedConfirmationMethod;
    public float DwellTime;
    public int Timeout;
    public int EyeSmoothFactor;
    public float MouseSensivity;
    public bool TargetConditionsRandomized;
    public bool BeepOnError;
    public bool ShowCursor;
    public bool HoverHightlight;
    public bool ButtonDownHighlight;
    public bool RecordGazePosition;
    public Color BackgroundColor;
    public Color CursorColor;
    public Color TargetColor;
    public Color ButtonDownColor;
    public Color HoverColor;
    public Color ReadyForGestureColor;

    //Test Result Data
    public double MovementTime;
    public double ErrorRate;
    public double Throughput;

    public double MeanRotationSumDegree;

    public List<TestSequence> Sequences;
    public List<TestVerification> Verifications;
    public VRHMD SelectedVRHMD;

    /// <summary>
    /// TestBlock constructor.
    /// </summary>
    /// <param name="vrHmd"> The selected HMD.</param>
    /// <param name="participantCode"> Code of the participant.</param>
    /// <param name="conditionCode"> Code of the condition.</param>
    /// <param name="blockCode"> Code of the block.</param>
    /// <param name="numberOfTargets"> The number of targets per sequence.</param>
    /// <param name="targetAmplitudes"> A list of target amplitudes.</param>
    /// <param name="targetDiameters"> A list of target widths.</param>
    /// <param name="errorThreshold"> If this threshold is exceeded, a sequence has to be repeated.</param>
    /// <param name="spatialHysteresis"> A multiplier for the target collider. Applied upon entering the target.</param>
    /// <param name="controlMethod"> The chosen control method.</param>
    /// <param name="selectedConfirmationMethod"> The chosen confirmation(activation) method.</param>
    /// <param name="dwellTime"> Time in milliseconds required to activate a target when using the dwell confirmation method.</param>
    /// <param name="timeout"> Time in milliseconds before a timeout error is logged for a trial.</param>
    /// <param name="eyeSmoothFactor"> Number of frames over which gaze point is smoothed over.</param>
    /// <param name="mouseSensivity"> Mouse cursor sensitivity; this is different from your machine's default mouse sensitivity setting.
    /// For our tests, we set the mouse sensitivity in the system settings to average, and set this variable to 10.</param>
    /// <param name="targetConditionsRandomized"> Applies randomisation to width/amplitude combination order if enabled.</param>
    /// <param name="beepOnError"> If enabled, the program will make a sound if a click occurs outside a target.</param>
    /// <param name="showCursor"> If enabled, shows a cursor in the shape of a small circle.</param>
    /// <param name="hoverHightlight"> If enabled, upon hovering over a target its' color will change to the one set in hoverColor.</param>
    /// <param name="buttonDownHighlight"> If enabled, upon clicking and holding the button while over a target its' color will change to the one set in buttonDownColor.</param>
    /// <param name="recordGazePosition"> If enabled, gaze position will be logged in DataLogs.</param>
    /// <param name="backgroundColor"> Color of the background.</param>
    /// <param name="cursorColor"> Color of the cursor.</param>
    /// <param name="targetColor"> Default color of the target.</param>
    /// <param name="buttonDownColor"> Color of the target if clicked and held.</param>
    /// <param name="hoverColor"> Color of the target if hovered over.</param>
    /// <param name="readyForGestureColor"> Color of the target if using head gesture (nod) as confirmation method.</param>
    public TestBlock(VRHMD vrHmd, string participantCode, string conditionCode, string blockCode, int numberOfTargets, List<int> targetAmplitudes, List<float> targetDiameters,
        float errorThreshold, float spatialHysteresis, ControlMethod controlMethod, ConfirmationMethod selectedConfirmationMethod, float dwellTime, int timeout, int eyeSmoothFactor,
        float mouseSensivity, bool targetConditionsRandomized, bool beepOnError, bool showCursor, bool hoverHightlight, bool buttonDownHighlight, bool recordGazePosition,
        Color backgroundColor, Color cursorColor, Color targetColor, Color buttonDownColor, Color hoverColor, Color readyForGestureColor)
    {
        SelectedVRHMD = vrHmd;
        StartTime = DateTime.Now;
        ParticipantCode = participantCode;
        ConditionCode = conditionCode;
        BlockCode = blockCode;
        NumberOfTargets = numberOfTargets;
        TargetAmplitudes = targetAmplitudes;
        TargetDiameters = targetDiameters;
        ErrorThreshold = errorThreshold;
        SpatialHysteresis = spatialHysteresis;
        SelectedControlMethod = controlMethod;
        SelectedConfirmationMethod = selectedConfirmationMethod;
        DwellTime = dwellTime;
        Timeout = timeout;
        EyeSmoothFactor = eyeSmoothFactor;
        MouseSensivity = mouseSensivity;
        TargetConditionsRandomized = targetConditionsRandomized;
        BeepOnError = beepOnError;
        ShowCursor = showCursor;
        HoverHightlight = hoverHightlight;
        ButtonDownHighlight = buttonDownHighlight;
        RecordGazePosition = recordGazePosition;
        BackgroundColor = backgroundColor;
        CursorColor = cursorColor;
        TargetColor = targetColor;
        ButtonDownColor = buttonDownColor;
        HoverColor = hoverColor;
        ReadyForGestureColor = readyForGestureColor;

        Sequences = new List<TestSequence>();
        int trialnumber = 0;

        //for (int i = 0; i < 1; ++i)
        for (int i = 0; i < TargetAmplitudes.Count; ++i)
        {
            Sequences.Add(new TestSequence(trialnumber, TargetAmplitudes[i], targetDiameters[i], NumberOfTargets));
            trialnumber++;
        }

        //foreach (int amplitude in TargetAmplitudes)
        //{
        //    foreach (float diameter in targetDiameters)
        //    {
        //        Sequences.Add(new TestSequence(trialnumber, amplitude, diameter, NumberOfTargets));
        //        trialnumber++;
        //    }
        //}
    }
    
    public List<Vector2> GetTrialCenterError() {
        List<Vector2> centerErrors = new List<Vector2>();
        foreach (TestSequence sequence in Sequences)
        {
            centerErrors.AddRange(sequence.GetTrialCenterError());
        }
        return centerErrors;
    }

    public List<double> GetMovementTimes() {
        List<double> movementTimes = new List<double>();
        foreach (TestSequence sequence in Sequences) {
            movementTimes.AddRange(sequence.GetMovementTimes());
        }
        return movementTimes;
    }

    /// <summary>
    /// Calculate the mean activation time for the block.
    /// </summary>
    public void CalculateMeanMovementTime() {
        List<double> movementTimes = new List<double>();
        foreach (TestSequence sequence in Sequences)
        {
            movementTimes.AddRange(sequence.Trials.Select(trial => trial.TimeToActivate));
        }
        MovementTime = TestDataHelper.Mean(movementTimes);
    }

    /// <summary>
    /// Calculate the total error rate for the block.
    /// </summary>
    public void CalculateErrorRate()
    {
        int errors = Sequences.Sum(sequence => sequence.Errors);
        ErrorRate = errors / (float)(NumberOfTargets * TargetAmplitudes.Count * TargetDiameters.Count) * 100f;
    }

    /// <summary>
    /// Creates a block DTO, along with all sequence, trial and datalog DTOs that comprise it.
    /// </summary>
    /// <returns> Returns the block DTO.</returns>
    public TestBlockDTO CreateDTO()
    {
        DBController.LastTableIds lastIds = DBController.Instance.GetLastTableIds();

        TestBlockDTO blockDTO = new TestBlockDTO(BlockCode, ParticipantCode, StartTime, SelectedVRHMD.ToString(),
            ConditionCode, ErrorThreshold, SpatialHysteresis, SelectedControlMethod.ToString(),
            SelectedConfirmationMethod.ToString(), DwellTime, Timeout, EyeSmoothFactor, MouseSensivity, TargetConditionsRandomized,
            BeepOnError, ShowCursor, HoverHightlight, ButtonDownHighlight, RecordGazePosition, BackgroundColor,
            CursorColor, TargetColor, ButtonDownColor, HoverColor, ReadyForGestureColor, MovementTime, ErrorRate, Throughput);

        for (int sequenceIndex = 0; sequenceIndex < Sequences.Count; sequenceIndex++)
        {
            blockDTO.TestSequenceDTOs.Add(Sequences[sequenceIndex].CreateDTO(lastIds.BlockId+1));

            for (int trialIndex = 0; trialIndex < Sequences[sequenceIndex].Trials.Count; trialIndex++)
            {
                blockDTO.TestSequenceDTOs[sequenceIndex].TestTrialDTOs.Add(Sequences[sequenceIndex].Trials[trialIndex].CreateDTO(lastIds.SequenceId+1 + sequenceIndex));

                blockDTO.TestSequenceDTOs[sequenceIndex].TestTrialDTOs[trialIndex].DataLogDTOs =
                    Sequences[sequenceIndex].Trials[trialIndex].GenerateDataLogDTOList(lastIds.TrialId+1 + trialIndex + sequenceIndex * Sequences[sequenceIndex].Trials.Count);
            }

        }
        if (Verifications != null)
        {
            for (int verificationIndex = 0; verificationIndex < Verifications.Count; verificationIndex++)
            {
                blockDTO.TestVerificationDTOs.Add(Verifications[verificationIndex].CreateDTO(lastIds.BlockId + 1));
            }
            Verifications.Clear();
        }
        return blockDTO;
    }

}
