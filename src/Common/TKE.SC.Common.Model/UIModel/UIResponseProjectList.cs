using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// UIResponseProjectList
    /// </summary>
    public class UIResponseProjectList
    {
        public bool IsViewUser { get; set; }
        public List<ProjectResponseDetails> Projects { get; set; }
        public List<string> Permissions { get; set; }
    }
}