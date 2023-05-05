/************************************************************************************************************
************************************************************************************************************
    File Name     :   FloorPlan.cs
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
    /// FloorPlan
    /// </summary>
    public class FloorPlan
    {
        /// <summary>
        /// GroupConfigurationId
        /// </summary>
        public string groupConfigurationId { get; set; }
        /// <summary>
        /// SelectedElevators
        /// </summary>
        public string selectedElevators { get; set; }
        /// <summary>
        /// CreatedBy
        /// </summary>
        public User createdBy { get; set; }
        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime createdOn { get; set; }
        /// <summary>
        /// ModifiedBy
        /// </summary>
        public User modifiedBy { get; set; }
        /// <summary>
        /// ModifiedOn
        /// </summary>
        public DateTime modifiedOn { get; set; }
        /// <summary>
        /// isDeleted
        /// </summary>
        public bool isDeleted { get; set; }
    }

    /// <summary>
    /// FloorPlanDetails
    /// </summary>
    public class FloorPlanDetails
    {
        /// <summary>
        /// GroupConfigurationId
        /// </summary>
        public string groupConfigurationId { get; set; }
        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool isDeleted { get; set; }
    }

    /// <summary>
    /// ResultOpeningLocation
    /// </summary>
    public class ResultOpeningLocation
    {
        /// <summary>
        /// Result
        /// </summary>
        public string result { get; set; }
    }

    /// <summary>
    /// ResultFloor
    /// </summary>
    public class ResultFloor
    {
        /// <summary>
        /// GroupConfigurationId
        /// </summary>
        public string groupConfigurationId { get; set; }
    }
}