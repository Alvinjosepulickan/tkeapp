using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// COMMON
    /// </summary>
    public class COMMON
    {
        public List<item> VALUES { get; set; }
    }

    /// <summary>
    /// item
    /// </summary>
    public class item
    {
        public string NAME { get; set; }
        public string VALUE { get; set; }
    }
}