using System;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// Model for AutoSave
    /// </summary>
    public class AutoSaveConfiguration
    {
        public JObject RequestMessage { get; set; }
        public string UserName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
