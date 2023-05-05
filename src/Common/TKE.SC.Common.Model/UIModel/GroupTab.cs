using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// GroupTab
    /// </summary>
    public class GroupTab
    {
        /// <summary>
        /// Sections
        /// </summary>
        public List<GroupSectionsData> Sections { get; set; }
    }

    /// <summary>
    /// GroupSectionsData
    /// </summary>
    public class GroupSectionsData
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// tabHeading
        /// </summary>
        public string TabHeading { get; set; }
        /// <summary>
        /// routerLink
        /// </summary>
        public string RouterLink { get; set; }
        /// <summary>
        /// isDisabled
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}