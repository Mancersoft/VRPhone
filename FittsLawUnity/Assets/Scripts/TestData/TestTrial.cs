using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class collects all data required to create a TestTrialDTO. 
/// Data includes trial number, start time, the target's angle, time to fixate on and activate target,
/// target center error, total head and cursor movement throughout trial, and whether a timeout or error occured.
/// TestTrials are visually represented by the active target during the sequence.
/// As such, the class is also in charge of the creation of DataLogs and DataLogDTOs.
/// </summary>

public class TestTrial
{
    // Pre-defined data.
    public float TargetAngle;

    // TestSequence result data.
    public DateTime StartTime;
    public int TrialNumber;
    public double TimeToFixate;
    public double TimeToActivate;
    public Vector2 TargetCenterError;
    public float TotalHeadMovement;
    public float TotalCursorMovement;
    public bool TimedOut;
    public bool Error;

    public List<DataLog> Logs;

    /// <summary>
    /// TestTrial constructor. 
    /// </summary>
    /// <param name="targetAngle"> Modified angle at which the target should spawn.</param>
    public TestTrial(float targetAngle)
    {
        TargetAngle = targetAngle;
        Logs = new List<DataLog>();
    }
    
    /// <summary>
    /// Add new DataLog to a list of DataLogs.
    /// </summary>
    /// <param name="msProgress"> Milliseconds since start of the trial.</param>
    /// <param name="cursorPosition"> Position on the canvas of the cursor.</param>
    /// <param name="gazePosition"> Position on the canvas of the gaze.</param>
    /// <param name="pupilDiameterLeft"> Pupil diameter of the left eye. (PupilLabs only)</param>
    /// <param name="pupilDiameterRight"> Pupil diameter of the right eye. (PupilLabs only)</param>
    /// <param name="pupilDiameter3DLeft"> 3d Pupil diameter of the left eye. (PupilLabs only)</param>
    /// <param name="pupilDiameter3DRight"> 3d Pupil diameter of the right eye. (PupilLabs only)</param>
    /// <param name="pupilConfidenceLeft"> Pupil confidence of the left eye. (PupilLabs only)</param>
    /// <param name="pupilConfidenceRight"> Pupil confidence of the right eye. (PupilLabs only)</param>
    /// <param name="headMovement"> Sum of angular head movement up to this DataLog.</param>
    /// <param name="nosePosition"> Position on the canvas to which the center of the head (nose) is pointing.</param>
    /// <param name="hmdPositionX"> X coordinate position of the hmd.</param>
    /// <param name="hmdPositionY"> Y coordinate position of the hmd.</param>
    /// <param name="hmdPositionZ"> Z coordinate position of the hmd.</param>
    /// <param name="hmdRotationX"> X coordinate rotation of the hmd.</param>
    /// <param name="hmdRotationY"> Y coordinate rotation of the hmd.</param>
    /// <param name="hmdRotationZ"> Z coordinate rotation of the hmd.</param>
    public void LogData(double msProgress, Vector2 cursorPosition, Vector2 gazePosition, double? pupilDiameterLeft, double? pupilDiameterRight,
        double? pupilDiameter3DLeft, double? pupilDiameter3DRight, double? pupilConfidenceLeft, double? pupilConfidenceRight, double? pupilTimestampLeft, double? pupilTimestampRight,
        float headMovement, Vector2 nosePosition, float? hmdPositionX, float? hmdPositionY, float? hmdPositionZ, float? hmdRotationX, float? hmdRotationY, float? hmdRotationZ)
    {
        Logs.Add(new DataLog(msProgress, cursorPosition, gazePosition, pupilDiameterLeft, pupilDiameterRight, pupilDiameter3DLeft, pupilDiameter3DRight,
            pupilConfidenceLeft, pupilConfidenceRight, pupilTimestampLeft, pupilTimestampRight, headMovement, nosePosition, hmdPositionX, hmdPositionY, hmdPositionZ,
            hmdRotationX, hmdRotationY, hmdRotationZ));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="testSequenceId"> The Id of the sequence that this trial is a part of.</param>
    /// <returns>Returns a TestTrialDTO.</returns>
    public TestTrialDTO CreateDTO(int testSequenceId)
    {
       return new TestTrialDTO(testSequenceId, TrialNumber, StartTime, TargetAngle, TimeToFixate, TimeToActivate, 
            TargetCenterError.x, TargetCenterError.y, TotalHeadMovement, TotalCursorMovement, TimedOut, Error);
    }

    /// <summary>
    /// Transform the list of DataLogs to a list of DataLogDTOs. 
    /// </summary>
    /// <param name="testTrialId"> The Id of the trial that these DataLogs are a part of.</param>
    /// <returns></returns>
    public List<DataLogDTO> GenerateDataLogDTOList(int testTrialId) {
        List<DataLogDTO> dtoLogs = new List<DataLogDTO>();
        foreach (DataLog dataLog in Logs) {
            dtoLogs.Add(dataLog.CreateDTO(testTrialId));
        }
        return dtoLogs;
    }
    
    public override string ToString()
    {
        string s = "StartTime: " + TrialNumber + ": TimeToFixate " + TimeToFixate + ", TimeToActivate " + TimeToActivate +
                   ", TargetCenterError " + TargetCenterError + ", TotalHeadMovement " + TotalHeadMovement +
                   "\b, TotalCursorMovement " + TotalCursorMovement;
        return s;
    }

}
