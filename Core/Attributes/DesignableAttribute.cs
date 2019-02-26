using System;

namespace Wss.FoundationCore.Attributes
{
    /// <summary>
    ///     标记该属性可被属性编辑器发现
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DesignableAttribute : Attribute
    {
        public Boolean ReadOnly { get; set; }
    }
}