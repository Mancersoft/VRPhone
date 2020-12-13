using System;
using System.Collections;
using System.Collections.Generic;
using SQLite4Unity3d;
using UnityEngine;

/// <summary>
/// This is a DTO class with no business logic, used solely for moving data to the database.
/// </summary>
[Table("TestTrials")]
public class TestTrialDTO {
    [PrimaryKey, AutoIncrement]
    public int TestTrialId { get; set; }
    public int TestSequenceId { get; set; }
    public int TrialNumber { get; set; }
    public string StartTime { get; set; }
    public float TargetAngle { get; set; }
    public double TimeToFixate { get; set; }
    public double TimeToActivate { get; set; }
    public float TargetCenterErrorX { get; set; }
    public float TargetCenterErrorY { get; set; }
    public float TotalHeadMovement { get; set; }
    public float TotalCursorMovement { get; set; }
    public bool TimedOut { get; set; }
    public bool Error { get; set; }

    public List<DataLogDTO> DataLogDTOs;

    /// <summary>
    /// TestTrialDTO constructor.
    /// </summary>
    /// <param name="testSequenceId"> Id of the sequence this trial is a part of.</param>
    /// <param name="trialNumber"> The number of this specific trial (out of a number of trials that a sequence makes up).</param>
    /// <param name="startTime"> Start time in "MM/dd/yyyy hh:mm:ss.fff tt" format.</param>
    /// <param name="targetAngle"> The angle from the center of the screen at which the target is spawned.</param>
    /// <param name="timeToFixate"> The time in milliseconds to first enter the target.</param>
    /// <param name="timeToActivate"> The time in milliseconds to activate the target.</param>
    /// <param name="targetCenterErrorX"> The distance from the center of the target to the activation point in the X coordinate.</param>
    /// <param name="targetCenterErrorY"> The distance from the center of the target to the activation point in the Y coordinate.</param>
    /// <param name="totalHeadMovement"> The total head movement for the trial.</param>
    /// <param name="totalCursorMovement"> The total cursor movement for the trial.</param>
    /// <param name="timedOut"> Variable showing whether the target timed out before activation could occur.</param>
    /// <param name="error"> Variable showing whether activation occured outside the target.</param>
    public TestTrialDTO(int testSequenceId, int trialNumber, DateTime startTime, float targetAngle, double timeToFixate, double timeToActivate, float targetCenterErrorX, float targetCenterErrorY, float totalHeadMovement, float totalCursorMovement, bool timedOut, bool error)
    {
        TestSequenceId = testSequenceId;
        TrialNumber = trialNumber;
        StartTime = startTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        TargetAngle = targetAngle;
        TimeToFixate = timeToFixate;
        TimeToActivate = timeToActivate;
        TargetCenterErrorX = targetCenterErrorX;
        TargetCenterErrorY = targetCenterErrorY;
        TotalHeadMovement = totalHeadMovement;
        TotalCursorMovement = totalCursorMovement;
        TimedOut = timedOut;
        Error = error;

        DataLogDTOs = new List<DataLogDTO>();
    }
}
