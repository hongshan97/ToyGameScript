//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: proto/Attack.proto
namespace proto.Attack
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Attack")]
  public partial class Attack : global::ProtoBuf.IExtensible
  {
    public Attack() {}
    
    private string _desc = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"desc", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string desc
    {
      get { return _desc; }
      set { _desc = value; }
    }
    private float _e = default(float);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"e", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float e
    {
      get { return _e; }
      set { _e = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}