//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: proto/Move.proto
namespace proto.Move
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Move")]
  public partial class Move : global::ProtoBuf.IExtensible
  {
    public Move() {}
    
    private string _desc = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"desc", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string desc
    {
      get { return _desc; }
      set { _desc = value; }
    }
    private float _x = default(float);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"x", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float x
    {
      get { return _x; }
      set { _x = value; }
    }
    private float _y = default(float);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"y", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float y
    {
      get { return _y; }
      set { _y = value; }
    }
    private float _z = default(float);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"z", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float z
    {
      get { return _z; }
      set { _z = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}