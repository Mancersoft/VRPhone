//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.1
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace SolAR.Datastructure {

    //using XPCF.Core;
    using SolAR.Core;

public class SquaredBinaryPattern : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnBase;

  internal SquaredBinaryPattern(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwnBase = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(SquaredBinaryPattern obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~SquaredBinaryPattern() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwnBase) {
          swigCMemOwnBase = false;
          solar_datastructurePINVOKE.delete_SquaredBinaryPattern(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public SquaredBinaryPattern() : this(solar_datastructurePINVOKE.new_SquaredBinaryPattern__SWIG_0(), true) {
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public SquaredBinaryPattern(SquaredBinaryPatternMatrix pattern) : this(solar_datastructurePINVOKE.new_SquaredBinaryPattern__SWIG_1(SquaredBinaryPatternMatrix.getCPtr(pattern)), true) {
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public SquaredBinaryPatternMatrix getPatternMatrix() {
    SquaredBinaryPatternMatrix ret = new SquaredBinaryPatternMatrix(solar_datastructurePINVOKE.SquaredBinaryPattern_getPatternMatrix(swigCPtr), false);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public FrameworkReturnCode setPatternMatrix(SquaredBinaryPatternMatrix pattern) {
    FrameworkReturnCode ret = (FrameworkReturnCode)solar_datastructurePINVOKE.SquaredBinaryPattern_setPatternMatrix(swigCPtr, SquaredBinaryPatternMatrix.getCPtr(pattern));
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getSize() {
    int ret = solar_datastructurePINVOKE.SquaredBinaryPattern_getSize(swigCPtr);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
