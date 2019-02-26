using System;
using System.Windows.Data;
//using Wss.Common;

namespace Wss.FoundationCore.Attributes
{
    public interface IModelProperty : IDesignProperty
    {
        string FieldName { get; }
        //UnitMark UnitMark { get; }
        TargetGroup TargetGroup { get; }
    }

    public interface IDesignProperty : ISerializeProperty
    {
        /// <summary>
        ///     显示名称
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        ///     获取是否为只读属性
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        ///     自定义编辑器
        /// </summary>
        Type Editer { get; }

        /// <summary>
        ///     数据绑定对象
        /// </summary>
        Binding Binding { get; }
        /// <summary>
        /// 每个属性可能都要绑定一个点位值,这里给点位ID
        /// </summary>
        string Guid { get; set; }
    }
}