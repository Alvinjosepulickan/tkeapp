using System.Collections.Generic;
using TKE.SC.Common.Model;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// CreateProjectResponseObject
    /// </summary>
    public class CreateProjectResponseObject
    {
        public List<Sections> Sections { get; set; }
        public Dictionary<string, string> VariableDetails { get; set; }
        public Dictionary<string, string> ProjectDisplayDetails { get; set; }
        public List<string> Permissions { get; set; }
    }
}