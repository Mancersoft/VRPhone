using System;
using System.Collections;
using System.Collections.Generic;
////using System.Data;
using SQLite4Unity3d;
using UnityEngine;

/// <summary>
/// This is a DTO class with no business logic, used solely for moving data to the database.
/// </summary>
[Table("TestSequences")]
public class TestSequenceDTO {
    [PrimaryKey, AutoIncrement]
    public int TestSequenceId { get; set; }
    public int TestBlockId { get; set; }
    public int SequenceNumber { get; set; }
    public string StartTime { get; set; }
    public int TargetAmplitude { get; set; }
    public float TargetWidth { get; set; }
    public double IndexOfDifficulty { get; set; }
    public double EffectiveAmplitude { get; set; }
    public double EffectiveTargetWidth { get; set; }
    public double EffecttiveIndexOfDifficulty { get; set; }
    public int SequenceOfRepeats { get; set; }
    public int Errors { get; set; }
    public double MovementTime { get; set; }
    public double ErrorRate { get; set; }
    public double Throughput { get; set; }

    public List<TestTrialDTO> TestTrialDTOs;

    /// <summary>
    /// TestSequenceDTO constructor.
    /// </summary>
    /// <param name="sequenceNumber"> Number of the specific sequence (out of a number of sequences that a block makes up).</param>
    /// <param name="testBlockDto"> Id of the block this sequence is a part of.</param>
    /// <param name="startTime">  Start time in "MM/dd/yyyy hh:mm:ss.fff tt" format.</param>
    /// <param name="targetAmplitude"> Target amplitude, set through the GUI.</param>
    /// <param name="targetWidth"> Target width, set through the GUI.</param>
    /// <param name="indexOfDifficulty"> Index of difficulty, calculated from the target amplitude and width. (Fitts' Law)</param>
    /// <param name="effectiveAmplitude"> The calculated effective amplitude. (Fitts' Law)</param>
    /// <param name="effectiveTargetWidth"> The calculated effective width. (Fitts' Law)</param>
    /// <param name="effecttiveIndexOfDifficulty"> The calculated effective index of difficulty. (Fitts' Law)</param>
    /// <param name="sequenceOfRepeats"> The number of times the sequence had to be repeated due to the user exceeding the error threshold.</param>
    /// <param name="errors"> The number of errors that occured.</param>
    /// <param name="movementTime"> The mean movement time.</param>
    /// <param name="errorRate"> The percentage of errors that occured.</param>
    /// <param name="throughput"> The throughput of the sequence. (Fitts' Law)</param>
    public TestSequenceDTO(int sequenceNumber, int testBlockDto, DateTime startTime, int targetAmplitude, float targetWidth,
        double indexOfDifficulty, double effectiveAmplitude, double effectiveTargetWidth, double effecttiveIndexOfDifficulty, int sequenceOfRepeats, int errors, double movementTime, double errorRate, double throughput)
    {
        SequenceNumber = sequenceNumber;
        TestBlockId = testBlockDto;
        StartTime = startTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        TargetAmplitude = targetAmplitude;
        TargetWidth = targetWidth;
        TestTrialDTOs = new List<TestTrialDTO>();
        IndexOfDifficulty = indexOfDifficulty;
        EffectiveAmplitude = effectiveAmplitude;
        EffectiveTargetWidth = effectiveTargetWidth;
        EffecttiveIndexOfDifficulty = effecttiveIndexOfDifficulty;
        SequenceOfRepeats = sequenceOfRepeats;
        Errors = errors;
        MovementTime = movementTime;
        ErrorRate = errorRate;
        Throughput = throughput;
    }
}
