//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.1
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace SolAR.Api.Tracking {

    using XPCF.Api;
    using SolAR.Core;
    using SolAR.Datastructure;

public class UCharList : global::System.IDisposable, global::System.Collections.IEnumerable, global::System.Collections.Generic.IList<byte>
 {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal UCharList(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(UCharList obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~UCharList() {
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
          solar_api_trackingPINVOKE.delete_UCharList(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public UCharList(global::System.Collections.IEnumerable c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (byte element in c) {
      this.Add(element);
    }
  }

  public UCharList(global::System.Collections.Generic.IEnumerable<byte> c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (byte element in c) {
      this.Add(element);
    }
  }

  public bool IsFixedSize {
    get {
      return false;
    }
  }

  public bool IsReadOnly {
    get {
      return false;
    }
  }

  public byte this[int index]  {
    get {
      return getitem(index);
    }
    set {
      setitem(index, value);
    }
  }

  public int Capacity {
    get {
      return (int)capacity();
    }
    set {
      if (value < size())
        throw new global::System.ArgumentOutOfRangeException("Capacity");
      reserve((uint)value);
    }
  }

  public int Count {
    get {
      return (int)size();
    }
  }

  public bool IsSynchronized {
    get {
      return false;
    }
  }

  public void CopyTo(byte[] array)
  {
    CopyTo(0, array, 0, this.Count);
  }

  public void CopyTo(byte[] array, int arrayIndex)
  {
    CopyTo(0, array, arrayIndex, this.Count);
  }

  public void CopyTo(int index, byte[] array, int arrayIndex, int count)
  {
    if (array == null)
      throw new global::System.ArgumentNullException("array");
    if (index < 0)
      throw new global::System.ArgumentOutOfRangeException("index", "Value is less than zero");
    if (arrayIndex < 0)
      throw new global::System.ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
    if (count < 0)
      throw new global::System.ArgumentOutOfRangeException("count", "Value is less than zero");
    if (array.Rank > 1)
      throw new global::System.ArgumentException("Multi dimensional array.", "array");
    if (index+count > this.Count || arrayIndex+count > array.Length)
      throw new global::System.ArgumentException("Number of elements to copy is too large.");
    for (int i=0; i<count; i++)
      array.SetValue(getitemcopy(index+i), arrayIndex+i);
  }

  public byte[] ToArray() {
    byte[] array = new byte[this.Count];
    this.CopyTo(array);
    return array;
  }

  global::System.Collections.Generic.IEnumerator<byte> global::System.Collections.Generic.IEnumerable<byte>.GetEnumerator() {
    return new UCharListEnumerator(this);
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
    return new UCharListEnumerator(this);
  }

  public UCharListEnumerator GetEnumerator() {
    return new UCharListEnumerator(this);
  }

  // Type-safe enumerator
  /// Note that the IEnumerator documentation requires an InvalidOperationException to be thrown
  /// whenever the collection is modified. This has been done for changes in the size of the
  /// collection but not when one of the elements of the collection is modified as it is a bit
  /// tricky to detect unmanaged code that modifies the collection under our feet.
  public sealed class UCharListEnumerator : global::System.Collections.IEnumerator
    , global::System.Collections.Generic.IEnumerator<byte>
  {
    private UCharList collectionRef;
    private int currentIndex;
    private object currentObject;
    private int currentSize;

    public UCharListEnumerator(UCharList collection) {
      collectionRef = collection;
      currentIndex = -1;
      currentObject = null;
      currentSize = collectionRef.Count;
    }

    // Type-safe iterator Current
    public byte Current {
      get {
        if (currentIndex == -1)
          throw new global::System.InvalidOperationException("Enumeration not started.");
        if (currentIndex > currentSize - 1)
          throw new global::System.InvalidOperationException("Enumeration finished.");
        if (currentObject == null)
          throw new global::System.InvalidOperationException("Collection modified.");
        return (byte)currentObject;
      }
    }

    // Type-unsafe IEnumerator.Current
    object global::System.Collections.IEnumerator.Current {
      get {
        return Current;
      }
    }

    public bool MoveNext() {
      int size = collectionRef.Count;
      bool moveOkay = (currentIndex+1 < size) && (size == currentSize);
      if (moveOkay) {
        currentIndex++;
        currentObject = collectionRef[currentIndex];
      } else {
        currentObject = null;
      }
      return moveOkay;
    }

    public void Reset() {
      currentIndex = -1;
      currentObject = null;
      if (collectionRef.Count != currentSize) {
        throw new global::System.InvalidOperationException("Collection modified.");
      }
    }

    public void Dispose() {
        currentIndex = -1;
        currentObject = null;
    }
  }

  public void Clear() {
    solar_api_trackingPINVOKE.UCharList_Clear(swigCPtr);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Add(byte x) {
    solar_api_trackingPINVOKE.UCharList_Add(swigCPtr, x);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  private uint size() {
    uint ret = solar_api_trackingPINVOKE.UCharList_size(swigCPtr);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private uint capacity() {
    uint ret = solar_api_trackingPINVOKE.UCharList_capacity(swigCPtr);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private void reserve(uint n) {
    solar_api_trackingPINVOKE.UCharList_reserve(swigCPtr, n);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public UCharList() : this(solar_api_trackingPINVOKE.new_UCharList__SWIG_0(), true) {
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public UCharList(UCharList other) : this(solar_api_trackingPINVOKE.new_UCharList__SWIG_1(UCharList.getCPtr(other)), true) {
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public UCharList(int capacity) : this(solar_api_trackingPINVOKE.new_UCharList__SWIG_2(capacity), true) {
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  private byte getitemcopy(int index) {
    byte ret = solar_api_trackingPINVOKE.UCharList_getitemcopy(swigCPtr, index);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private byte getitem(int index) {
    byte ret = solar_api_trackingPINVOKE.UCharList_getitem(swigCPtr, index);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private void setitem(int index, byte val) {
    solar_api_trackingPINVOKE.UCharList_setitem(swigCPtr, index, val);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public void AddRange(UCharList values) {
    solar_api_trackingPINVOKE.UCharList_AddRange(swigCPtr, UCharList.getCPtr(values));
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public UCharList GetRange(int index, int count) {
    global::System.IntPtr cPtr = solar_api_trackingPINVOKE.UCharList_GetRange(swigCPtr, index, count);
    UCharList ret = (cPtr == global::System.IntPtr.Zero) ? null : new UCharList(cPtr, true);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Insert(int index, byte x) {
    solar_api_trackingPINVOKE.UCharList_Insert(swigCPtr, index, x);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public void InsertRange(int index, UCharList values) {
    solar_api_trackingPINVOKE.UCharList_InsertRange(swigCPtr, index, UCharList.getCPtr(values));
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveAt(int index) {
    solar_api_trackingPINVOKE.UCharList_RemoveAt(swigCPtr, index);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveRange(int index, int count) {
    solar_api_trackingPINVOKE.UCharList_RemoveRange(swigCPtr, index, count);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public static UCharList Repeat(byte value, int count) {
    global::System.IntPtr cPtr = solar_api_trackingPINVOKE.UCharList_Repeat(value, count);
    UCharList ret = (cPtr == global::System.IntPtr.Zero) ? null : new UCharList(cPtr, true);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Reverse() {
    solar_api_trackingPINVOKE.UCharList_Reverse__SWIG_0(swigCPtr);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Reverse(int index, int count) {
    solar_api_trackingPINVOKE.UCharList_Reverse__SWIG_1(swigCPtr, index, count);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetRange(int index, UCharList values) {
    solar_api_trackingPINVOKE.UCharList_SetRange(swigCPtr, index, UCharList.getCPtr(values));
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
  }

  public bool Contains(byte value) {
    bool ret = solar_api_trackingPINVOKE.UCharList_Contains(swigCPtr, value);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int IndexOf(byte value) {
    int ret = solar_api_trackingPINVOKE.UCharList_IndexOf(swigCPtr, value);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int LastIndexOf(byte value) {
    int ret = solar_api_trackingPINVOKE.UCharList_LastIndexOf(swigCPtr, value);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool Remove(byte value) {
    bool ret = solar_api_trackingPINVOKE.UCharList_Remove(swigCPtr, value);
    if (solar_api_trackingPINVOKE.SWIGPendingException.Pending) throw solar_api_trackingPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
