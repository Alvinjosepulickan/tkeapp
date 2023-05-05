using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
    public class DuplicateVersionDetails
    {
        /// <summary>
        /// ActionType
        /// </summary>
        public string ActionType { get; set; }
        /// <summary>
        /// OpportunityId
        /// </summary>
        public string OpportunityId { get; set; }
        /// <summary>
        /// QuoteId
        /// </summary>
        public string QuoteId { get; set; }
        /// <summary>
        /// SourceVersion
        /// </summary>
        public string SourceVersion { get; set; }
        /// <summary>
        /// TargetVersion
        /// </summary>
        public int TargetVersion { get; set; }

    }
}
