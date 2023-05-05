using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
     public class Tp2DocumentGeneration : UnitSummaryUIModel
    {
        /// <summary>
        /// Variables
        /// </summary>
        public Dictionary<string, object> Variables { get; set; }
        /// <summary>
        /// Culture
        /// </summary>
        public string Culture { get; set; }
        /// <summary>
        /// Identifier
        /// </summary>
        public string Identifier { get; set; }
    }
    public class DocumentrequestBody
    {
        public Tp2DocumentGeneration DocumentModel { get; set; }
    }
}
