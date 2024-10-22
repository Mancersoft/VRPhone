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

public class Frame : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnBase;

  internal Frame(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwnBase = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(Frame obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~Frame() {
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
          solar_datastructurePINVOKE.delete_Frame(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public Frame(Frame frame) : this(solar_datastructurePINVOKE.new_Frame__SWIG_0(Frame.getCPtr(frame)), true) {
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public Frame(Keyframe keyframe) : this(solar_datastructurePINVOKE.new_Frame__SWIG_1(Keyframe.getCPtr(keyframe)), true) {
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public Frame(KeypointArray keypoints, DescriptorBuffer descriptors, Image view, Keyframe refKeyframe, Transform3Df pose) : this(solar_datastructurePINVOKE.new_Frame__SWIG_2(KeypointArray.getCPtr(keypoints), DescriptorBuffer.getCPtr(descriptors), Image.getCPtr(view), Keyframe.getCPtr(refKeyframe), Transform3Df.getCPtr(pose)), true) {
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public Frame(KeypointArray keypoints, DescriptorBuffer descriptors, Image view, Keyframe refKeyframe) : this(solar_datastructurePINVOKE.new_Frame__SWIG_3(KeypointArray.getCPtr(keypoints), DescriptorBuffer.getCPtr(descriptors), Image.getCPtr(view), Keyframe.getCPtr(refKeyframe)), true) {
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public Frame(KeypointArray keypoints, DescriptorBuffer descriptors, Image view, Transform3Df pose) : this(solar_datastructurePINVOKE.new_Frame__SWIG_4(KeypointArray.getCPtr(keypoints), DescriptorBuffer.getCPtr(descriptors), Image.getCPtr(view), Transform3Df.getCPtr(pose)), true) {
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public Frame(KeypointArray keypoints, DescriptorBuffer descriptors, Image view) : this(solar_datastructurePINVOKE.new_Frame__SWIG_5(KeypointArray.getCPtr(keypoints), DescriptorBuffer.getCPtr(descriptors), Image.getCPtr(view)), true) {
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public Image getView() {
    global::System.IntPtr cPtr = solar_datastructurePINVOKE.Frame_getView(swigCPtr);
    Image ret = (cPtr == global::System.IntPtr.Zero) ? null : new Image(cPtr, true);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public Transform3Df getPose() {
    Transform3Df ret = new Transform3Df(solar_datastructurePINVOKE.Frame_getPose(swigCPtr), true);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setPose(Transform3Df pose) {
    solar_datastructurePINVOKE.Frame_setPose(swigCPtr, Transform3Df.getCPtr(pose));
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setKeypoints(KeypointArray kpts) {
    solar_datastructurePINVOKE.Frame_setKeypoints(swigCPtr, KeypointArray.getCPtr(kpts));
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setReferenceKeyframe(Keyframe keyframe) {
    solar_datastructurePINVOKE.Frame_setReferenceKeyframe(swigCPtr, Keyframe.getCPtr(keyframe));
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public Keyframe getReferenceKeyframe() {
    global::System.IntPtr cPtr = solar_datastructurePINVOKE.Frame_getReferenceKeyframe(swigCPtr);
    Keyframe ret = (cPtr == global::System.IntPtr.Zero) ? null : new Keyframe(cPtr, true);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public DescriptorBuffer getDescriptors() {
    global::System.IntPtr cPtr = solar_datastructurePINVOKE.Frame_getDescriptors(swigCPtr);
    DescriptorBuffer ret = (cPtr == global::System.IntPtr.Zero) ? null : new DescriptorBuffer(cPtr, true);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public KeypointArray getKeypoints() {
    KeypointArray ret = new KeypointArray(solar_datastructurePINVOKE.Frame_getKeypoints(swigCPtr), false);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public MapIntInt getVisibleKeypoints() {
    MapIntInt ret = new MapIntInt(solar_datastructurePINVOKE.Frame_getVisibleKeypoints(swigCPtr), false);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void addVisibleKeypoints(MapIntInt kpVisibility) {
    solar_datastructurePINVOKE.Frame_addVisibleKeypoints(swigCPtr, MapIntInt.getCPtr(kpVisibility));
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public void addVisibleMapPoints(MapIntInt mapPoints) {
    solar_datastructurePINVOKE.Frame_addVisibleMapPoints(swigCPtr, MapIntInt.getCPtr(mapPoints));
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public void addVisibleMapPoint(uint id_keypoint, uint id_cloudPoint) {
    solar_datastructurePINVOKE.Frame_addVisibleMapPoint(swigCPtr, id_keypoint, id_cloudPoint);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
  }

  public MapIntInt getVisibleMapPoints() {
    MapIntInt ret = new MapIntInt(solar_datastructurePINVOKE.Frame_getVisibleMapPoints(swigCPtr), false);
    if (solar_datastructurePINVOKE.SWIGPendingException.Pending) throw solar_datastructurePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
