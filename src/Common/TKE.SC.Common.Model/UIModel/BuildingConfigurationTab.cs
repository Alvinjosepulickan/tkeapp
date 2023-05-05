using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// BuildingConfigurationTab
    /// </summary>
    public class BuildingConfigurationTab
    {
        public List<SectionsData> sections { get; set; }
    }

    /// <summary>
    /// SectionsData
    /// </summary>
    public class SectionsData
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool isDisabled { get; set; }
    }
}
