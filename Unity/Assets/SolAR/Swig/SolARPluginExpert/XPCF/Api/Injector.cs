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

public class Injector : InjectableMetadata {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal Injector(global::System.IntPtr cPtr, bool cMemoryOwn) : base(xpcf_apiPINVOKE.Injector_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(Injector obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          xpcf_apiPINVOKE.delete_Injector(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public Injector(SWIGTYPE_p_std__functionT_void_fboost__shared_ptrT_org__bcom__xpcf__IComponentIntrospect_tF_t injector, UUID serviceUUID, bool optional) : this(xpcf_apiPINVOKE.new_Injector__SWIG_0(SWIGTYPE_p_std__functionT_void_fboost__shared_ptrT_org__bcom__xpcf__IComponentIntrospect_tF_t.getCPtr(injector), UUID.getCPtr(serviceUUID), optional), true) {
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

  public Injector(SWIGTYPE_p_std__functionT_void_fboost__shared_ptrT_org__bcom__xpcf__IComponentIntrospect_tF_t injector, UUID serviceUUID) : this(xpcf_apiPINVOKE.new_Injector__SWIG_1(SWIGTYPE_p_std__functionT_void_fboost__shared_ptrT_org__bcom__xpcf__IComponentIntrospect_tF_t.getCPtr(injector), UUID.getCPtr(serviceUUID)), true) {
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

  public Injector(SWIGTYPE_p_std__functionT_void_fboost__shared_ptrT_org__bcom__xpcf__IComponentIntrospect_tF_t injector, UUID serviceUUID, string name, bool optional) : this(xpcf_apiPINVOKE.new_Injector__SWIG_2(SWIGTYPE_p_std__functionT_void_fboost__shared_ptrT_org__bcom__xpcf__IComponentIntrospect_tF_t.getCPtr(injector), UUID.getCPtr(serviceUUID), name, optional), true) {
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

  public Injector(SWIGTYPE_p_std__functionT_void_fboost__shared_ptrT_org__bcom__xpcf__IComponentIntrospect_tF_t injector, UUID serviceUUID, string name) : this(xpcf_apiPINVOKE.new_Injector__SWIG_3(SWIGTYPE_p_std__functionT_void_fboost__shared_ptrT_org__bcom__xpcf__IComponentIntrospect_tF_t.getCPtr(injector), UUID.getCPtr(serviceUUID), name), true) {
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void inject(IComponentIntrospect instance) {
    xpcf_apiPINVOKE.Injector_inject(swigCPtr, IComponentIntrospect.getCPtr(instance));
    if (xpcf_apiPINVOKE.SWIGPendingException.Pending) throw xpcf_apiPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
