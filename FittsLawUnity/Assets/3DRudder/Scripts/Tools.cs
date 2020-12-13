using UnityEngine;
using ns3DRudder;
using System;

namespace Unity3DRudder
{
    public class EventRudder : ns3DRudder.IEvent
    {
        public Action<uint> OnConnectEvent;
        public Action<uint> OnDisconnectEvent;

        public override void OnConnect(uint nDeviceNumber)
        {
            if (OnConnectEvent != null)
                OnConnectEvent(nDeviceNumber);
        }

        public override void OnDisconnect(uint nDeviceNumber)
        {
            if (OnDisconnectEvent != null)
                OnDisconnectEvent(nDeviceNumber);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnConnectEvent = null;
            OnDisconnectEvent = null;
        }
    }

    public class CurveRudder : ns3DRudder.Curve
    {
        public bool UseUnityCurve = false;
        public AnimationCurve Curve = null;

        public CurveRudder(float fDeadZone, float fxSat, float fyMax, float fExp) : base(fDeadZone, fxSat, fyMax, fExp)
        {
        }

        public CurveRudder(AnimationCurve curve, bool useUnityCurve = true)
        {
            UseUnityCurve = useUnityCurve;
            Curve = curve;
        }

        public override float CalcCurveValue(float fValue)
        {
            return UseUnityCurve ? Curve.Evaluate(fValue) : base.CalcCurveValue(fValue);
        }
    }

    public class Rudder : IDisposable
    {
        #region Properties
        private uint portNumber;
        private ns3DRudder.Axis axis;
        private ns3DRudder.Axis axisOffset;
        private s3DRudderManager sdk;
        private bool freeze;
        #endregion

        #region Functions
        public Rudder(uint portNumber, s3DRudderManager sdk)
        {
            this.portNumber = portNumber;
            this.sdk = sdk;
            axis = new ns3DRudder.Axis();
            axisOffset = new ns3DRudder.Axis();
            freeze = false;
        }

        public void Dispose()
        {
            axis.Dispose();
            axisOffset.Dispose();
        }

        ~Rudder()
        {
            Dispose();
        }
        #endregion

        #region SDK
        /// <summary>
        /// Returns if rudder is connected
        /// </summary>
        /// <returns>bool</returns>
        public bool IsDeviceConnected()
        {
            return sdk.IsDeviceConnected(portNumber);
        }

        /// <summary>
        /// Returns version of firmware
        /// </summary>
        /// <returns>uint</returns>
        public uint GetVersion()
        {
            return sdk.GetVersion(portNumber);
        }

        /// <summary>
        /// Enable/Disable rudder as joystick
        /// </summary>
        /// <param name="hide"></param>
        public void HideSystemDevice(bool hide)
        {
            ErrorCode error = sdk.HideSystemDevice(portNumber, hide);
            if (error != ErrorCode.Success)
                Debug.LogWarningFormat("HideSystemDevice : {0}, portnumber : {1}, hide : {2}", error, portNumber, hide);
        }

        /// <summary>
        /// Returns if rudder is hided
        /// </summary>
        /// <returns>bool</returns>
        public bool IsSystemDeviceHidden()
        {
            return sdk.IsSystemDeviceHidden(portNumber);
        }

        /// <summary>
        /// Plays sound
        /// </summary>
        /// <param name="frequencyHz"></param>
        /// <param name="durationMillisecond"></param>
        public void PlaySnd(ushort frequencyHz, ushort durationMillisecond)
        {
            ErrorCode error = sdk.PlaySnd(portNumber, frequencyHz, durationMillisecond);
            if (error != ErrorCode.Success)
                Debug.LogWarningFormat("PlaySnd : {0}, frequency : {1}, duration : {2}", error, frequencyHz, durationMillisecond);
        }

        /// <summary>
        /// Returns if rudder is frozen
        /// </summary>
        /// <returns>bool</returns>
        public bool IsFrozen()
        {
            return freeze;
        }

        /// <summary>
        /// Plays sound
        /// </summary>
        public void SetFreeze(bool active)
        {
            freeze = active;
            ErrorCode error = sdk.SetFreeze(portNumber, freeze);
            if (error != ErrorCode.Success)
                Debug.LogWarningFormat("Freeze : {0}, active : {1}", error, freeze);
        }

        /// <summary>
        /// Returns user offset
        /// </summary>
        /// <returns>Axis</returns>
        public ns3DRudder.Axis GetUserOffset()
        {
            ErrorCode error = sdk.GetUserOffset(portNumber, axisOffset);
            if (error != ErrorCode.Success)
                Debug.LogWarningFormat("GetUserOffset : {0}, portnumber : {1}", error, portNumber);
            return axisOffset;
        }

        /// <summary>
        /// Returns axis
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>Axis</returns>
        public ns3DRudder.Axis GetAxis(ModeAxis mode = ModeAxis.NormalizedValue)
        {
            ErrorCode error = sdk.GetAxis(portNumber, mode, axis);
            if (error != ErrorCode.Success)
                Debug.LogWarningFormat("GetAxis : {0}, portnumber : {1}, mode : {2}", error, portNumber, mode);
            return axis;
        }

        /// <summary>
        /// Returns axis with Curve
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>Axis</returns>
        public ns3DRudder.Axis GetAxisWithCurve(ModeAxis mode, CurveArray curves)
        {
            ErrorCode error = sdk.GetAxis(portNumber, mode, axis, curves);
            if (error != ErrorCode.Success)
                Debug.LogWarningFormat("GetAxisWithCurve : {0}, portnumber : {1}, mode : {2}", error, portNumber, mode);
            return axis;
        }

        /// <summary>
        /// Returns axis in unity's format (X,Y,Z)
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 GetAxis3D(ns3DRudder.Axis pAxis = null)
        {
            if (pAxis != null)
                return new Vector3(pAxis.GetXAxis(), pAxis.GetZAxis(), pAxis.GetYAxis());

            GetAxis();
            return new Vector3(axis.GetXAxis(), axis.GetZAxis(), axis.GetYAxis());
        }

        /// <summary>
        /// Returns status
        /// </summary>
        /// <returns>Status</returns>
        public ns3DRudder.Status GetStatus()
        {
            return sdk.GetStatus(portNumber);
        }

        /// <summary>
        /// Returns sensor's value
        /// </summary>
        /// <param name="index"></param>
        /// <returns>uint</returns>
        public uint GetSensor(uint index)
        {
            return sdk.GetSensor(portNumber, index);
        }
        #endregion
    }
}