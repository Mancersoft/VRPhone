//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.1
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace XPCF.Properties {

    using XPCF.Core;
    using XPCF.Collection;

public class IPropertyMap : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnBase;

  internal IPropertyMap(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwnBase = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(IPropertyMap obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~IPropertyMap() {
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
          xpcf_propertiesPINVOKE.delete_IPropertyMap(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public virtual XPCFErrorCode addProperty(IProperty p) {
    XPCFErrorCode ret = (XPCFErrorCode)xpcf_propertiesPINVOKE.IPropertyMap_addProperty(swigCPtr, IProperty.getCPtr(p));
    if (xpcf_propertiesPINVOKE.SWIGPendingException.Pending) throw xpcf_propertiesPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public virtual XPCFErrorCode setProperty(IProperty p) {
    XPCFErrorCode ret = (XPCFErrorCode)xpcf_propertiesPINVOKE.IPropertyMap_setProperty(swigCPtr, IProperty.getCPtr(p));
    if (xpcf_propertiesPINVOKE.SWIGPendingException.Pending) throw xpcf_propertiesPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public virtual PropertyEnumerable getProperties() {
    PropertyEnumerable ret = new PropertyEnumerable(xpcf_propertiesPINVOKE.IPropertyMap_getProperties(swigCPtr), false);
    if (xpcf_propertiesPINVOKE.SWIGPendingException.Pending) throw xpcf_propertiesPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public virtual IProperty at(string name) {
    global::System.IntPtr cPtr = xpcf_propertiesPINVOKE.IPropertyMap_at(swigCPtr, name);
    IProperty ret = (cPtr == global::System.IntPtr.Zero) ? null : new IProperty(cPtr, true);
    if (xpcf_propertiesPINVOKE.SWIGPendingException.Pending) throw xpcf_propertiesPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public enum AccessSpecifier {
    IPropertyMap_IN = 0x01,
    IPropertyMap_OUT = 0x02,
    IPropertyMap_INOUT = 0x04,
    IPropertyMap_ALL = 0x07
  }

}

}