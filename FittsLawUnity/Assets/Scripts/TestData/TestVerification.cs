using System;
using UnityEngine;

/// <summary>
/// This class holds all data needed to create a TestVerificationDTO object.
/// Data includes a timestamp, milliseconds since the start of the trial, head movement,
/// cursor and gaze position on screen, and pupil data(if using the Pupil Labs eye-tracker).
/// A TestVerification represents the smallest timestep in the simulation, and is created every update.
/// This class is essentially identical to DataLog, but is only used during calibration verification.
/// </summary>

public class TestVerification {

    public string TimeStamp;
    public int TargetIndex;
    public Vector2 TargetPosition;
    public Vector2 CursorPosition;
    public Vector2 GazePosition;
    public double? PupilDiameterLeft;
    public double? PupilDiameterRight;
    public double? PupilDiameter3DLeft;
    public double? PupilDiameter3DRight;
    public double? PupilConfidenceLeft;
    public double? PupilConfidenceRight;
    public double? PupilTimestampLeft;
    public double? PupilTimestampRight;
    public Vector2 NosePosition;
    public float? HmdPositionX;
    public float? HmdPositionY;
    public float? HmdPositionZ;
    public float? HmdRotationX;
    public float? HmdRotationY;
    public float? HmdRotationZ;
    /// <summary>
    /// DataLog constructor.
    /// </summary>
    /// <param name="targetIndex"> The index of the currently active target.</param>
    /// <param name="targetPosition"> Position of the target.</param>
    /// <param name="cursorPosition"> Position on the canvas of the cursor.</param>
    /// <param name="gazePosition"> Position on the canvas of the gaze.</param>
    /// <param name="pupilDiameterLeft"> Pupil diameter of the left eye. (PupilLabs only)</param>
    /// <param name="pupilDiameterRight"> Pupil diameter of the right eye. (PupilLabs only)</param>
    /// <param name="pupilDiameter3DLeft"> 3d Pupil diameter of the left eye. (PupilLabs only)</param>
    /// <param name="pupilDiameter3DRight"> 3d Pupil diameter of the right eye. (PupilLabs only)</param>
    /// <param name="pupilConfidenceLeft"> Pupil confidence of the left eye. (PupilLabs only)</param>
    /// <param name="pupilConfidenceRight"> Pupil confidence of the right eye. (PupilLabs only)</param>
    /// <param name="pupilTimestampLeft"> Pupil timestamp of the left eye. (PupilLabs only)</param>
    /// <param name="pupilTimestampRight"> Pupil timestamp of the right eye. (PupilLabs only)</param>
    /// <param name="nosePosition"> Position on the canvas to which the center of the head (nose) is pointing.</param>
    /// <param name="hmdPositionX"> X coordinate position of the hmd.</param>
    /// <param name="hmdPositionY"> Y coordinate position of the hmd.</param>
    /// <param name="hmdPositionZ"> Z coordinate position of the hmd.</param>
    /// <param name="hmdRotationX"> X coordinate rotation of the hmd.</param>
    /// <param name="hmdRotationY"> Y coordinate rotation of the hmd.</param>
    /// <param name="hmdRotationZ"> Z coordinate rotation of the hmd.</param>
    public TestVerification(int targetIndex, Vector2 targetPosition, Vector2 cursorPosition, Vector2 gazePosition, double? pupilDiameterLeft, double? pupilDiameterRight,
        double? pupilDiameter3DLeft, double? pupilDiameter3DRight, double? pupilConfidenceLeft, double? pupilConfidenceRight, double? pupilTimestampLeft, double? pupilTimestampRight,
        Vector2 nosePosition, float? hmdPositionX, float? hmdPositionY, float? hmdPositionZ, float? hmdRotationX, float? hmdRotationY, float? hmdRotationZ) {
        TimeStamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        TargetIndex = targetIndex;
        TargetPosition = targetPosition;
        CursorPosition = cursorPosition;
        GazePosition = gazePosition;
        PupilDiameterLeft = pupilDiameterLeft;
        PupilDiameterRight = pupilDiameterRight;
        PupilDiameter3DLeft = pupilDiameter3DLeft;
        PupilDiameter3DRight = pupilDiameter3DRight;
        PupilConfidenceLeft = pupilConfidenceLeft;
        PupilConfidenceRight = pupilConfidenceRight;
        PupilTimestampLeft = pupilTimestampLeft;
        PupilTimestampRight = pupilTimestampRight;
        NosePosition = nosePosition;
        HmdPositionX = hmdPositionX;
        HmdPositionY = hmdPositionY;
        HmdPositionZ = hmdPositionZ;
        HmdRotationX = hmdRotationX;
        HmdRotationY = hmdRotationY;
        HmdRotationZ = hmdRotationZ;
    }

    /// <summary>
    /// Create a new DataLogDTO.
    /// </summary>
    /// <param name="verificationTargetId"> The Id of the trial that this DTO is a part of.</param>
    /// <returns> The newly created DTO.</returns>
    public TestVerificationDTO CreateDTO(int testBlockId) {
        return new TestVerificationDTO(testBlockId, TimeStamp, TargetIndex, TargetPosition.x, TargetPosition.y, CursorPosition.x, CursorPosition.y, GazePosition.x, GazePosition.y,
            PupilDiameterLeft, PupilDiameterRight, PupilDiameter3DLeft, PupilDiameter3DRight, PupilConfidenceLeft, PupilConfidenceRight, PupilTimestampLeft, PupilTimestampRight,
            NosePosition.x, NosePosition.y, HmdPositionX, HmdPositionY, HmdPositionZ, HmdRotationX, HmdRotationY, HmdRotationZ);
    }
}
