using System;

namespace Wss.FoundationCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelTypeNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public ModelTypeNameAttribute(string name)
        {
            Name = name;
        }
    }
}