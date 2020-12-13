#define DEBUG_3DRUDDER

using UnityEngine;
using System;

namespace Unity3DRudder
{
    public class s3DRudderManager : ns3DRudder.CSdk
    {
        #region Properties
        public static readonly int _3DRUDDER_SDK_MAX_DEVICE = ns3DRudder.i3DR._3DRUDDER_SDK_MAX_DEVICE;
        public static readonly int _3DRUDDER_SDK_VERSION = ns3DRudder.i3DR._3DRUDDER_SDK_VERSION;

        // Instance
        private static s3DRudderManager _instance;
        public static s3DRudderManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new s3DRudderManager();
                }
                return _instance;
            }
        }

        // Event
        public EventRudder Events;

        // States for all rudders (MAX = 4)
        private Rudder[] rudders;
        #endregion

        #region Functions
        private s3DRudderManager()
        {
            // Set events Connected & Disconnected
            Events = new EventRudder();
            SetEvent(Events);
#if DEBUG_3DRUDDER
            Events.OnConnectEvent += (portNumber) => Debug.LogFormat("3dRudder {0} connected, firmware : {1:X4}", portNumber, GetVersion(portNumber));
            Events.OnDisconnectEvent += (portNumber) => Debug.LogFormat("3dRudder {0} disconnected, firmware : {1:X4}", portNumber, GetVersion(portNumber));
#endif
            // Init SDK
            Init();

            // Init States
            rudders = new Rudder[_3DRUDDER_SDK_MAX_DEVICE];
            for (uint i = 0; i < rudders.Length; ++i)
                rudders[i] = new Rudder(i, this);
#if DEBUG_3DRUDDER
            // Show info
            Debug.LogFormat("SDK version : {0:X4}" , GetSDKVersion());
#endif            
        }

        public void ShutDown()
        {
#if DEBUG_3DRUDDER
            Debug.Log("shutdown s3DRudderManager");
#endif
            Dispose();
        }

        public override void Dispose()
        {
#if DEBUG_3DRUDDER
            Debug.Log("dispose s3DRudderManager");
#endif
            base.Dispose();
            foreach (Rudder r in rudders)
                r.Dispose();
            Events.Dispose();

            _instance = null;
            GC.SuppressFinalize(this);
        }
#endregion

#region SDK
        /// <summary>
        /// Returns rudder
        /// </summary>
        /// <param name="portNumber"></param>
        /// <returns>Rudder</returns>
        public Rudder GetRudder(int portNumber)
        {
            return rudders[portNumber];
        }
#endregion
    }
}