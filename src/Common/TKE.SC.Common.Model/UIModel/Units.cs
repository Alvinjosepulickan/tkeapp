/************************************************************************************************************
************************************************************************************************************
    File Name     :   model class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// Units
    /// </summary>
    public class Units
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Client Name
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// Unit Id
        /// </summary>
        public string UnitId { get; set; }
        /// <summary>
        /// Unit Name
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// CarPosition
        /// </summary>
        public string CarPosition { get; set; }
        /// <summary>
        /// Number Of Landings
        /// </summary>
        public int numberOfLandings { get; set; }
        /// <summary>
        /// Capacity Unit
        /// </summary>
        public string CapacityUnit { get; set; }
        /// <summary>
        /// Capacity Value
        /// </summary>
        public Int64 CapacityValue { get; set; }
        /// <summary>
        /// Car Speed Unit
        /// </summary>
        public string CarspeedUnit { get; set; }
        /// <summary>
        /// Car speed Value
        /// </summary>
        public Int16 CarspeedValue { get; set; }
        /// <summary>
        /// Currency Code
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// Status Id
        /// </summary>
        public int StatusID { get; set; }
        /// <summary>
        /// Status Value
        /// </summary>
        public string StatusValue { get; set; }
        /// <summary>
        /// Control Group
        /// </summary>
        public string ControlGroup { get; set; }
        /// <summary>
        /// Config Group
        /// </summary>
        public string ConfigGroup { get; set; }
        /// <summary>
        /// Product Id
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// Product Name
        /// </summary>
        public string ProductName { get; set; }
    }

    /// <summary>
    /// UnitDesignation
    /// </summary>
    public class UnitDesignation
    {
        /// <summary>
        /// Designation
        /// </summary>
        public string Designation { get; set; }

        public string Description { get; set; }
    }
}