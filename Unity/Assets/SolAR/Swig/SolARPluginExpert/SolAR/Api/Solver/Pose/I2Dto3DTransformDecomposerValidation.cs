//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.1
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace SolAR.Api.Solver.Pose {

    using XPCF.Api;
    using SolAR.Core;
    using SolAR.Datastructure;

public class I2Dto3DTransformDecomposerValidation : IComponentIntrospect {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnDerived;

  internal I2Dto3DTransformDecomposerValidation(global::System.IntPtr cPtr, bool cMemoryOwn) : base(solar_api_solver_posePINVOKE.I2Dto3DTransformDecomposerValidation_SWIGSmartPtrUpcast(cPtr), true) {
    swigCMemOwnDerived = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(I2Dto3DTransformDecomposerValidation obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwnDerived) {
          swigCMemOwnDerived = false;
          solar_api_solver_posePINVOKE.delete_I2Dto3DTransformDecomposerValidation(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public virtual void testMethod() {
    solar_api_solver_posePINVOKE.I2Dto3DTransformDecomposerValidation_testMethod(swigCPtr);
    if (solar_api_solver_posePINVOKE.SWIGPendingException.Pending) throw solar_api_solver_posePINVOKE.SWIGPendingException.Retrieve();
  }

}

}
