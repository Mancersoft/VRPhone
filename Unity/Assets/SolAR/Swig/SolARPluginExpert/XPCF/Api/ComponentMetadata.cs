//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.1
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace XPCF.Api {

    using XPCF.Core;
    using XPCF.Collection;
    using XPCF.Properties;
    using XPCF.Traits;

public class ComponentMetadata : InterfaceMetadata {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnDerived;

  internal ComponentMetadata(global::System.IntPtr cPtr, bool cMemoryOwn) : base(xpcf_apiPINVOKE.ComponentMetadata_SWIGSmartPtrUpcast(cPtr), true) {
    swigCMemOwnDerived = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ComponentMetadata obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwnDerived) {
          swigCMemOwnDerived = false;
          xpcf_apiPINVOKE.delete_ComponentMetadata(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public ComponentMetadata(ComponentMetadata other) : this(xpcf_apiPINVOKE.new_ComponentMetadata__SWIG_0(ComponentMetadata.getCPtr(other)), true) {
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

  public ComponentMetadata(string name, UUID componentID, string description) : this(xpcf_apiPINVOKE.new_ComponentMetadata__SWIG_1(name, UUID.getCPtr(componentID), description), true) {
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

  public ComponentMetadata(string name, string componentID, string description) : this(xpcf_apiPINVOKE.new_ComponentMetadata__SWIG_2(name, componentID, description), true) {
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void addInterface(UUID interfaceUUID) {
    xpcf_apiPINVOKE.ComponentMetadata_addInterface(swigCPtr, UUID.getCPtr(interfaceUUID));
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

  public UUIDEnumerable getInterfaces() {
    UUIDEnumerable ret = new UUIDEnumerable(xpcf_apiPINVOKE.ComponentMetadata_getInterfaces(swigCPtr), false);
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
