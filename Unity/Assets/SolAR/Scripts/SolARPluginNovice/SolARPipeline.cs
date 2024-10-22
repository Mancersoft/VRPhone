﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using SolAR.Datastructure;
using UnityEngine.Android;
using System.Collections;

namespace SolAR
{
    public class SolARPipeline : MonoBehaviour
    {
        public Transform m_RotationControl;
        public Transform m_PrevSolARObj;
        public Transform m_SolARObj;
        public Camera mainCamera;

        public DeviceCameraScript deviceCameraScript;
        public PhoneManagerScript phoneManagerScript;
        public SocketIOSignaler socketSignalerScript;

        public GameObject findDeviceCanvas;
        public GameObject cardboardLoader;

        public bool isScanning;
        public bool isMarkerFound;

        public float cameraXShift;

        #region Variables
        //#####################################################
        [HideInInspector]
        public float focalX;

        [HideInInspector]
        public float focalY;

        [HideInInspector]
        public int width;

        [HideInInspector]
        public int height;

        [HideInInspector]
        public float centerX;

        [HideInInspector]
        public float centerY;

        //#####################################################

        private byte[] array_imageData;

        [HideInInspector]
        public string m_pipelineFolder;

        [HideInInspector]
        public string[] m_pipelinesName;

        [HideInInspector]
        public string[] m_pipelinesUUID;

        [HideInInspector]
        public string[] m_pipelinesPath;

        [HideInInspector]
        public int m_selectedPipeline;

        [HideInInspector]
        public string m_configurationPath;

        [HideInInspector]
        public string m_uuid;

        [HideInInspector]
        public int m_webCamNum;

        [HideInInspector]
        public ConfXml conf;

        [HideInInspector]
        public SolARPluginPipelineManager m_pipelineManager;

        [HideInInspector]
        public WebCamTexture m_webCamTexture;

        [HideInInspector]
        public bool m_Unity_Webcam = false;

        private IntPtr sourceTexture;

        private byte[] m_vidframe_byte;
        private Color32[] data;
        private bool UpdateReady = false;

        #endregion

        void OnDestroy()
        {
            m_pipelineManager.stop();
            m_pipelineManager.Dispose();
            m_pipelineManager = null;
        }

        void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            while (!Permission.HasUserAuthorizedPermission(Permission.Camera)) { Permission.RequestUserPermission(Permission.Camera); }
            Android.AndroidCloneResources(Application.streamingAssetsPath + "/SolAR/Android/android.xml");
            Android.LoadConfiguration(this);
#endif

            Init();
        }

        private float distLerpTime = 0;
        private Vector3 curPos = Vector3.zero;
        private Vector3 prevPos = Vector3.zero;

        private float rotLerpTime = 0;
        private Quaternion curRot = Quaternion.identity;
        private Quaternion prevRot = Quaternion.identity;
        private float lerpSpeed = 4;

        private void GetPhysicalCameraFrame()
        {
            m_webCamTexture.GetPixels32(data);
            //data = GetScaledTexture().GetPixels32();

            if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                int lengthMinusOne = data.Length - 1;
                for (int i = 0; i < data.Length; i++)
                {
                    m_vidframe_byte[3 * i] = data[lengthMinusOne - i].b;
                    m_vidframe_byte[3 * i + 1] = data[lengthMinusOne - i].g;
                    m_vidframe_byte[3 * i + 2] = data[lengthMinusOne - i].r;
                }
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    m_vidframe_byte[3 * i] = data[i].b;
                    m_vidframe_byte[3 * i + 1] = data[i].g;
                    m_vidframe_byte[3 * i + 2] = data[i].r;
                }
            }

            sourceTexture = Marshal.UnsafeAddrOfPinnedArrayElement(m_vidframe_byte, 0);
            m_pipelineManager.loadSourceImage(sourceTexture, width, height);
        }

        void Update()
        {
            if (isScanning && UpdateReady)
            {
                if (m_pipelineManager != null)
                {
                    if (m_Unity_Webcam)
                    {
                        GetPhysicalCameraFrame();
                    }
                    Transform3Df pose = new Transform3Df();

                    var _returnCode = m_pipelineManager.udpate(pose);

                    if (_returnCode == PIPELINEMANAGER_RETURNCODE._NEW_POSE || _returnCode == PIPELINEMANAGER_RETURNCODE._NEW_POSE_AND_IMAGE)
                    {
                        if (!isMarkerFound)
                        {
                            isMarkerFound = true;
                            findDeviceCanvas.SetActive(false);
                            cardboardLoader.SetActive(true);
                            deviceCameraScript.gameObject.SetActive(false);
                            phoneManagerScript.ChangeVisibility(true);
                        }

                        Matrix4x4 cameraPoseFromSolAR = new Matrix4x4();

                        cameraPoseFromSolAR.SetRow(0, new Vector4(pose.rotation().coeff(0, 0), pose.rotation().coeff(0, 1), pose.rotation().coeff(0, 2), pose.translation().coeff(0, 0)));
                        cameraPoseFromSolAR.SetRow(1, new Vector4(pose.rotation().coeff(1, 0), pose.rotation().coeff(1, 1), pose.rotation().coeff(1, 2), pose.translation().coeff(1, 0)));
                        cameraPoseFromSolAR.SetRow(2, new Vector4(pose.rotation().coeff(2, 0), pose.rotation().coeff(2, 1), pose.rotation().coeff(2, 2), pose.translation().coeff(2, 0)));
                        cameraPoseFromSolAR.SetRow(3, new Vector4(0, 0, 0, 1));

                        Matrix4x4 invertMatrix = new Matrix4x4();
                        invertMatrix.SetRow(0, new Vector4(1, 0, 0, 0));
                        invertMatrix.SetRow(1, new Vector4(0, -1, 0, 0));
                        invertMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
                        invertMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
                        Matrix4x4 unityCameraPose = invertMatrix * cameraPoseFromSolAR;

                        Vector3 forward = new Vector3(unityCameraPose.m02, unityCameraPose.m12, unityCameraPose.m22);
                        Vector3 up = new Vector3(unityCameraPose.m01, unityCameraPose.m11, unityCameraPose.m21);
                        var rotation = Quaternion.LookRotation(forward, -up);
                        var position = -new Vector3(unityCameraPose.m03, unityCameraPose.m13, unityCameraPose.m23);

                        m_PrevSolARObj.localPosition = position;
                        m_RotationControl.localRotation = rotation;
                        var prevParent = m_PrevSolARObj.parent;
                        m_PrevSolARObj.parent = m_RotationControl;
                        m_RotationControl.localRotation = Quaternion.identity;
                        m_PrevSolARObj.parent = prevParent;


                        m_PrevSolARObj.localRotation = Quaternion.Inverse(rotation);
                        m_PrevSolARObj.Rotate(new Vector3(-90, 0, 0), Space.Self);

                        if (Screen.orientation == ScreenOrientation.LandscapeRight)
                        {
                            m_PrevSolARObj.localPosition -= new Vector3(cameraXShift, 0, 0);
                        }
                        else
                        {
                            m_PrevSolARObj.localPosition += new Vector3(cameraXShift, 0, 0);
                        }

                        var rotDist = Quaternion.Angle(prevRot, m_PrevSolARObj.rotation);
                        if (rotDist >= 10)
                        {
                            prevRot = m_PrevSolARObj.rotation;
                            socketSignalerScript.ResetGyroInverse();
                        }

                        var posDist = Vector3.Distance(m_SolARObj.position, m_PrevSolARObj.position);
                        float distLerpStatus = (Time.time - distLerpTime) * lerpSpeed;
                        ////var angleDifferences = GetAngleDifferences(m_SolARObj.rotation, m_PrevSolARObj.rotation);
                        if (posDist >= 0.008 && distLerpStatus >= 1)
                        {
                            //Debug.Log("Pos dist: " + posDist);
                            //m_SolARObj.position = m_PrevSolARObj.position;
                            distLerpTime = Time.time;
                            curPos = m_SolARObj.position;
                            prevPos = m_PrevSolARObj.position;
                        }

                        distLerpStatus = (Time.time - distLerpTime) * lerpSpeed;
                        m_SolARObj.position = Vector3.Lerp(curPos, prevPos, distLerpStatus);

                    }
                }
            }

            SetFusedRotation();
        }

        private Quaternion realRotation = Quaternion.identity;

        private void SetFusedRotation()
        {
            var realCurrentRotation = prevRot * socketSignalerScript.gyroQuaternion;
            float rotLerpStatus = (Time.time - rotLerpTime) * lerpSpeed;
            if (rotLerpStatus >= 1)
            {
                rotLerpTime = Time.time;
                curRot = m_SolARObj.rotation;
                realRotation = realCurrentRotation;
            }

            rotLerpStatus = (Time.time - rotLerpTime) * lerpSpeed;

            m_SolARObj.rotation = Quaternion.Lerp(curRot, realRotation, rotLerpStatus);
        }

        public void Init()
        {
            m_pipelineManager = new SolARPluginPipelineManager();
#if UNITY_EDITOR
            // If in editor mode, the pipeline configuration file are stored in the unity assets folder but not in the streaminAssets folder
            if (!m_pipelineManager.init(Application.dataPath + m_configurationPath, m_uuid))
            {
                Debug.Log("Cannot init pipeline manager " + Application.dataPath + m_configurationPath + " with uuid " + m_uuid);
                return;
            }

#elif UNITY_ANDROID
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Android.ReplacePathToApp(Application.persistentDataPath + "/StreamingAssets" + m_configurationPath);
            // When the application is built, only the pipeline configuration files used by the application are moved to the an external folder on terminal
            Debug.Log("[ANDROID] Load pipeline : "+Application.persistentDataPath + "/StreamingAssets"+m_configurationPath);
           
            if (!m_pipelineManager.init(Application.persistentDataPath + "/StreamingAssets" + m_configurationPath, m_uuid))
            {
                Debug.Log("Cannot init pipeline manager " + Application.persistentDataPath + "/StreamingAssets" + m_configurationPath + " with uuid " + m_uuid);
                return;
            }
            Debug.Log("[ANDROID] Pipeline initialization successful");
            //m_Unity_Webcam = true;

#else
            // When the application is built, only the pipeline configuration files used by the application are moved to the streamingAssets folder
            if (!m_pipelineManager.init(Application.streamingAssetsPath + m_configurationPath, m_uuid))
                {
                Debug.Log("Cannot init pipeline manager " + Application.streamingAssetsPath + m_configurationPath + " with uuid " + m_uuid);
                return;
            }
            //m_Unity_Webcam = true;
#endif

            if (m_Unity_Webcam)
            {
                //m_webCamTexture = new WebCamTexture(WebCamTexture.devices[m_webCamNum].name, width, height);
                m_webCamTexture = deviceCameraScript.activeCameraTexture;
                m_webCamTexture.Play();

                data = new Color32[width * height];
                m_vidframe_byte = new byte[width * height * 3];

                GetPhysicalCameraFrame();
            }
            else
            {
                Matrix3x3f camParams = m_pipelineManager.getCameraParameters().intrinsic;
                width = Screen.width;
                height = Screen.height;
                focalX = camParams.coeff(0, 0); // focalX;
                focalY = camParams.coeff(1, 1);  // focalY;
                centerX = camParams.coeff(0, 2); // centerX;
                centerY = camParams.coeff(1, 2);// centerY;

            }

            SendParametersToCameraProjectionMatrix();
            array_imageData = new byte[width * height * 3];

            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(array_imageData, 0);
            m_pipelineManager.start(ptr);  //IntPtr

            //deviceCameraScript.UpdateScreenParams();
            //StartCoroutine(deviceCameraScript.UpdateScreenParamsCoroutine());
            UpdateReady = true;
        }

        void SendParametersToCameraProjectionMatrix()
        {
            Matrix4x4 projectionMatrix = new Matrix4x4();
            float near = mainCamera.nearClipPlane;
            float far = mainCamera.farClipPlane;

            Vector4 row0 = new Vector4(2.0f * focalX / width, 0, 1.0f - 2.0f * centerX / width, 0);
            Vector4 row1 = new Vector4(0, 2.0f * focalY / height, 2.0f * centerY / height - 1.0f, 0);
            Vector4 row2 = new Vector4(0, 0, (far + near) / (near - far), 2.0f * far * near / (near - far));
            Vector4 row3 = new Vector4(0, 0, -1, 0);

            projectionMatrix.SetRow(0, row0);
            projectionMatrix.SetRow(1, row1);
            projectionMatrix.SetRow(2, row2);
            projectionMatrix.SetRow(3, row3);

            mainCamera.fieldOfView = (Mathf.Rad2Deg * 2 * Mathf.Atan(width / (2 * focalX))) - 10;
            mainCamera.projectionMatrix = projectionMatrix;
        }
    }

}
