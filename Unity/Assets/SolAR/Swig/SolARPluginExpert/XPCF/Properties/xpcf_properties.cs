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

public class xpcf_properties {
  public static IPropertyMap getPropertyMapInstance() {
    global::System.IntPtr cPtr = xpcf_propertiesPINVOKE.getPropertyMapInstance();
    IPropertyMap ret = (cPtr == global::System.IntPtr.Zero) ? null : new IPropertyMap(cPtr, true);
    if (xpcf_propertiesPINVOKE.SWIGPendingException.Pending) throw xpcf_propertiesPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
