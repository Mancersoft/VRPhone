//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.1
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace SolAR.Api.Fusion {

    using XPCF.Api;
    using SolAR.Core;
    using SolAR.Datastructure;

public class InertialData : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal InertialData(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(InertialData obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~InertialData() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          solar_api_fusionPINVOKE.delete_InertialData(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public Vector3f accelData {
    set {
      solar_api_fusionPINVOKE.InertialData_accelData_set(swigCPtr, Vector3f.getCPtr(value));
      if (solar_api_fusionPINVOKE.SWIGPendingException.Pending) throw solar_api_fusionPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      Vector3f ret = new Vector3f(solar_api_fusionPINVOKE.InertialData_accelData_get(swigCPtr), true);
      if (solar_api_fusionPINVOKE.SWIGPendingException.Pending) throw solar_api_fusionPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public Vector3f gyroData {
    set {
      solar_api_fusionPINVOKE.InertialData_gyroData_set(swigCPtr, Vector3f.getCPtr(value));
      if (solar_api_fusionPINVOKE.SWIGPendingException.Pending) throw solar_api_fusionPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      Vector3f ret = new Vector3f(solar_api_fusionPINVOKE.InertialData_gyroData_get(swigCPtr), true);
      if (solar_api_fusionPINVOKE.SWIGPendingException.Pending) throw solar_api_fusionPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public Vector3f magData {
    set {
      solar_api_fusionPINVOKE.InertialData_magData_set(swigCPtr, Vector3f.getCPtr(value));
      if (solar_api_fusionPINVOKE.SWIGPendingException.Pending) throw solar_api_fusionPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      Vector3f ret = new Vector3f(solar_api_fusionPINVOKE.InertialData_magData_get(swigCPtr), true);
      if (solar_api_fusionPINVOKE.SWIGPendingException.Pending) throw solar_api_fusionPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public InertialData() : this(solar_api_fusionPINVOKE.new_InertialData(), true) {
    if (solar_api_fusionPINVOKE.SWIGPendingException.Pending) throw solar_api_fusionPINVOKE.SWIGPendingException.Retrieve();
  }

}

}