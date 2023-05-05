using System;
using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ResultUnitConfiguration
    /// </summary>
    public class ResultUnitConfiguration
    {
        public int result { get; set; }
        public int unitConfigurationId { get; set; }
        /// <summary>
		/// GroupConfigurationId
		/// </summary>
		public int groupConfigurationId { get; set; }
        public string message { get; set; }
        /// <summary>
		/// IsDuplicateNameError
		/// </summary>
		public Boolean isDuplicateNameError { get; set; }
        public List<string> carPositionsWithDuplicateNames { get; set; }
    }

    /// <summary>
    /// UnitNames
    /// </summary>
    public class UnitNames
    {
        /// <summary>
        /// UnitId
        /// </summary>
        public int Unitid { get; set; }
        /// <summary>
        /// unitname
        /// </summary>
        public string Unitname { get; set; }
        /// <summary>
        /// ueid 
        /// </summary>
        public string Ueid { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// SetId
        /// </summary>
        public int SetId { get; set; }
        /// <summary>
        /// ProductName
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }
    }

    public class ResultSetConfiguration
    {
        public int result { get; set; }
        public int setId { get; set; }
        public string message { get; set; }
        public string description { get; set; }
    }
}
