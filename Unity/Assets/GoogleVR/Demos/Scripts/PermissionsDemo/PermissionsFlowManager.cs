//-----------------------------------------------------------------------
// <copyright file="PermissionsFlowManager.cs" company="Google Inc.">
// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0(the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleVR.PermissionsDemo
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_ANDROID || UNITY_EDITOR
    public class PermissionsFlowManager : MonoBehaviour
    {
        private static string[] permissionNames = { "android.permission.CAMERA" };

        private static List<GvrPermissionsRequester.PermissionStatus> permissionList =
            new List<GvrPermissionsRequester.PermissionStatus>();

        void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            RequestPermissions();
#endif
        }

        /// <summary>
        /// Checks whether all necessary external permissions have been granted and informs user of
        /// the current state.
        /// </summary>
        public void CheckPermission()
        {
            Debug.Log("Checking permission....");
            GvrPermissionsRequester permissionRequester = GvrPermissionsRequester.Instance;
            if (permissionRequester != null)
            {
                bool granted = permissionRequester.IsPermissionGranted(permissionNames[0]);
                Debug.Log(permissionNames[0] + ": " + (granted ? "Granted" : "Denied"));
            }
            else
            {
                Debug.Log("Permission requester cannot be initialized.");
            }
        }

        /// <summary>
        /// Checks external permissions requirements and current permissions granted.
        /// </summary>
        /// <remarks>
        /// Prompts user with requests for outstanding needed permissions, and informs them of the
        /// current state.
        /// </remarks>
        public void RequestPermissions()
        {
            Debug.Log("Requesting permission....");

            GvrPermissionsRequester permissionRequester = GvrPermissionsRequester.Instance;
            if (permissionRequester == null)
            {
                Debug.Log("Permission requester cannot be initialized.");
                return;
            }

            Debug.Log("Permissions.RequestPermisions: Check if permission has been granted");
            if (!permissionRequester.IsPermissionGranted(permissionNames[0]))
            {
                Debug.Log("Permissions.RequestPermisions: Permission has not been previously " +
                          "granted");

                permissionRequester.RequestPermissions(permissionNames,
                    (GvrPermissionsRequester.PermissionStatus[] permissionResults) =>
                    {
                        permissionList.Clear();
                        permissionList.AddRange(permissionResults);
                        string msg = "";
                        foreach (GvrPermissionsRequester.PermissionStatus p in permissionList)
                        {
                            msg += p.Name + ": " + (p.Granted ? "Granted" : "Denied") + "\n";
                        }

                        Debug.Log(msg);
                    });
            }
            else
            {
                Debug.Log("Permission already granted!");
            }
        }
    }
#endif  // (UNITY_ANDROID || UNITY_EDITOR)
}
