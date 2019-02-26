using System;
using System.ComponentModel.Composition;
//using Wss.Objects;

namespace Wss.FoundationCore.Models
{


    [InheritedExport(typeof(BaseModel))]
    public abstract class BaseModel //: OmniObjectManager<OmniObject>
    {
        public Guid _guid = Guid.NewGuid();
        public BaseModel() : this("新模型")
        {
            
        }
        public BaseModel(string name) //: base(name, null)
        {
            //Bindings = new OmniPropertyManager("点位");
            //Propertys = new OmniObjectManager<OmniObject>("属性");
            //Actions = new OmniActionManager("动作");
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            //Add(Bindings);
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            //Add(Propertys);
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            //Add(Actions);
        }
        /// <summary>
        /// 点位集合 要与通信的点位列表（Ecms.Communiation.ModelPointCOnfig)做关联 
        /// </summary>
        //[Export]
        //public OmniPropertyManager Bindings { get; set; }
        //public OmniActionManager Actions { get; set; }
        //public OmniObjectManager<OmniObject> Propertys { get; set; }
       
    }
}