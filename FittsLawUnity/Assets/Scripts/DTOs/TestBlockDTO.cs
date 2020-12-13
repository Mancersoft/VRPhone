using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SQLite4Unity3d;
using UnityEngine;

/// <summary>
/// This is a DTO class with no business logic, used solely for moving data to the database.
/// </summary>
[Table("TestBlocks")]
public class TestBlockDTO {
    [PrimaryKey, AutoIncrement]
    public int TestBlockId { get; set; }
    public string BlockCode { get; set; }
    public string ParticipantCode { get; set; }
    public string StartTime { get; set; }
    public string VRHMD { get; set; }
    public string ConditionCode { get; set; }
    public float ErrorThreshold { get; set; }
    public float SpatialHysteresis { get; set; }
    public string ControlMethod { get; set; }
    public string ConfirmationMethod { get; set; }
    public float DwellTime { get; set; }
    public int Timeout { get; set; }
    public int EyeSmoothFactor { get; set; }
    public float MouseSensivity { get; set; }
    public bool RandomizedTargetConditions { get; set; }
    public bool BeepOnError { get; set; }
    public bool ShowCursor { get; set; }
    public bool HoverHightlight { get; set; }
    public bool ButtonDownHighlight { get; set; }
    public bool RecordGazePosition { get; set; }
    public string BackgroundColor { get; set; }
    public string CursorColor { get; set; }
    public string TargetColor { get; set; }
    public string ButtonDownColor { get; set; }
    public string HoverColor { get; set; }
    public string ReadyForGestureColor { get; set; }

    public double MovementTime { get; set; }
    public double ErrorRate { get; set; }
    public double Throughput { get; set; }

    public List<TestSequenceDTO> TestSequenceDTOs;
    public List<TestVerificationDTO> TestVerificationDTOs;

    /// <summary>
    /// TestBlockDTO constructor.
    /// </summary>
    /// <param name="blockCode"> Assign a unique identifier to the block.</param>
    /// <param name="participantCode"> Assign a unique identifier to the participant.</param>
    /// <param name="startTime"> Start time in "MM/dd/yyyy hh:mm:ss.fff tt" format.</param>
    /// <param name="vrhmd"> Which head-mounted display is in use.</param>
    /// <param name="conditionCode"> Assign a unique identifier to the condition.</param>
    /// <param name="errorThreshold"> Error threshold percentage. If exceeded, forces sequence to repeat.</param>
    /// <param name="spatialHysteresis"> Scales collider of target by this number upon selection.</param>
    /// <param name="controlMethod"> The selected control method.</param>
    /// <param name="confirmationMethod"> The selected confirmation method.</param>
    /// <param name="dwellTime"> If using the Dwell confirmation method, the time in milliseconds it takes to activate the target after fixation.</param>
    /// <param name="timeout"> The amount of time in milliseconds before the target times out if not activated.</param>
    /// <param name="eyeSmoothFactor"> The number of frames over which to gather an average of eye ray directions.</param>
    /// <param name="mouseSensivity"> If using the Mouse control method, sets the sensitivity of the GazeCursor.</param>
    /// <param name="randomizedTargetConditions"> Whether the order of sequence widths and amplitudes is randomised.</param>
    /// <param name="beepOnError"> Whether the program makes a sound on error.</param>
    /// <param name="showCursor"> Whether the GazeCursor is visible.</param>
    /// <param name="hoverHightlight"> Whether the target changes color on hover.</param>
    /// <param name="buttonDownHighlight"> Whether the target changes color on clicking down.</param>
    /// <param name="recordGazePosition"> Whether gaze position is logged.</param>
    /// <param name="backgroundColor"> The color of the background.</param>
    /// <param name="cursorColor"> The color of the cursor.</param>
    /// <param name="targetColor"> The color of the target.</param>
    /// <param name="buttonDownColor"> The color of the target when clicking on it.</param>
    /// <param name="hoverColor"> The color of the target when hovering over it.</param>
    /// <param name="readyForGestureColor"></param>
    /// <param name="movementTime"></param>
    /// <param name="errorRate"> The average percentage of errors over all sequences.</param>
    /// <param name="throughput"> The average throughput of all sequences.</param>
    public TestBlockDTO(string blockCode, string participantCode, DateTime startTime, string vrhmd, string conditionCode, float errorThreshold, float spatialHysteresis, string controlMethod, string confirmationMethod, float dwellTime, int timeout, int eyeSmoothFactor, float mouseSensivity, 
        bool randomizedTargetConditions, bool beepOnError, bool showCursor, bool hoverHightlight, bool buttonDownHighlight, bool recordGazePosition, Color backgroundColor, Color cursorColor, Color targetColor, Color buttonDownColor, Color hoverColor, Color readyForGestureColor, double movementTime, double errorRate, double throughput)
    {
        TestSequenceDTOs = new List<TestSequenceDTO>();
        TestVerificationDTOs = new List<TestVerificationDTO>();
        BlockCode = blockCode;
        ParticipantCode = participantCode;
        StartTime = startTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        VRHMD = vrhmd;
        ConditionCode = conditionCode;
        ErrorThreshold = errorThreshold;
        SpatialHysteresis = spatialHysteresis;
        ControlMethod = controlMethod;
        ConfirmationMethod = confirmationMethod;
        DwellTime = dwellTime;
        Timeout = timeout;
        EyeSmoothFactor = eyeSmoothFactor;
        MouseSensivity = mouseSensivity;
        RandomizedTargetConditions = randomizedTargetConditions;
        BeepOnError = beepOnError;
        ShowCursor = showCursor;
        HoverHightlight = hoverHightlight;
        ButtonDownHighlight = buttonDownHighlight;
        RecordGazePosition = recordGazePosition;
        BackgroundColor = backgroundColor.ToString();
        CursorColor = cursorColor.ToString();
        TargetColor = targetColor.ToString();
        ButtonDownColor = buttonDownColor.ToString();
        HoverColor = hoverColor.ToString();
        ReadyForGestureColor = readyForGestureColor.ToString();
        MovementTime = movementTime;
        ErrorRate = errorRate;
        Throughput = throughput;
    }

}
