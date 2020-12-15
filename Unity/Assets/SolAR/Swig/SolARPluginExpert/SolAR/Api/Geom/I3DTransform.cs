//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.1
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace SolAR.Api.Geom {

    using XPCF.Api;
    using SolAR.Core;
    using SolAR.Datastructure;

public class I3DTransform : IComponentIntrospect {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnDerived;

  internal I3DTransform(global::System.IntPtr cPtr, bool cMemoryOwn) : base(solar_api_geomPINVOKE.I3DTransform_SWIGSmartPtrUpcast(cPtr), true) {
    swigCMemOwnDerived = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(I3DTransform obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwnDerived) {
          swigCMemOwnDerived = false;
          solar_api_geomPINVOKE.delete_I3DTransform(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public virtual FrameworkReturnCode transform(Point3DfArray inputPoints, Transform3Df transformation, Point3DfArray outputPoints) {
    FrameworkReturnCode ret = (FrameworkReturnCode)solar_api_geomPINVOKE.I3DTransform_transform(swigCPtr, Point3DfArray.getCPtr(inputPoints), Transform3Df.getCPtr(transformation), Point3DfArray.getCPtr(outputPoints));
    if (solar_api_geomPINVOKE.SWIGPendingException.Pending) throw solar_api_geomPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}