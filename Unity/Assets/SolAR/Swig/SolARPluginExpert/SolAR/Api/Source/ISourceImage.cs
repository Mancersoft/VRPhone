//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.1
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace SolAR.Api.Source {

    using XPCF.Api;
    using SolAR.Core;
    using SolAR.Datastructure;

public class ISourceImage : IComponentIntrospect {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnDerived;

  internal ISourceImage(global::System.IntPtr cPtr, bool cMemoryOwn) : base(solar_api_sourcePINVOKE.ISourceImage_SWIGSmartPtrUpcast(cPtr), true) {
    swigCMemOwnDerived = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ISourceImage obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwnDerived) {
          swigCMemOwnDerived = false;
          solar_api_sourcePINVOKE.delete_ISourceImage(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public virtual SourceReturnCode setInputTexture(global::System.IntPtr sourceTexturehandle, int width, int height) {
    SourceReturnCode ret = (SourceReturnCode)solar_api_sourcePINVOKE.ISourceImage_setInputTexture(swigCPtr, sourceTexturehandle, width, height);
    if (solar_api_sourcePINVOKE.SWIGPendingException.Pending) throw solar_api_sourcePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public virtual SourceReturnCode getNextImage(Image image) {
    SourceReturnCode ret = (SourceReturnCode)solar_api_sourcePINVOKE.ISourceImage_getNextImage(swigCPtr, Image.getCPtr(image));
    if (solar_api_sourcePINVOKE.SWIGPendingException.Pending) throw solar_api_sourcePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}