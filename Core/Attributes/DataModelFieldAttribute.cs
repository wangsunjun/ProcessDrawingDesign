using System;

namespace Wss.FoundationCore.Attributes
{
    public enum TargetGroup
    {
        Default = 0,
        Mode = 1,
        Data = 2,
        Info,
        Config
    }

    /// <summary>
    /// 标记该属性可被绑定编辑器发现
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class DataModelFieldAttribute : Attribute
    {
        public string BindPath { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        //public Wss.Common.UnitMark UnitMark { get; set; }
        public override object TypeId
        {
            get { return this; }
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class PropertyGuidAttribute : Attribute
    {
        public string PropertyGuid { get; set; }
        public PropertyGuidAttribute(string guid)
        {
            PropertyGuid = guid;
        }
    }

    /// <summary>
    /// 标记该属性可被绑定编辑器发现
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DataModelAttribute : Attribute
    {
        public string ModelName { get; set; }
        public Type ModelType { get;  set; }
    }
}