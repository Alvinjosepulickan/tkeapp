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
    /// Projects
    /// </summary>
    public class Projects
    {
        /// <summary>
        /// Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Location
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// No Of Units
        /// </summary>
        public int noOfUnits { get; set; } // not present in DB table. calculate it from SP
        /// <summary>
        /// Client
        /// </summary>
        public Client client { get; set; }
        /// <summary>
        /// Total Price
        /// </summary>
        public TotalPrice totalPrice { get; set; }
        /// <summary>
        /// Complete Date
        /// </summary>
        public DateTime completedDate { get; set; }
        /// <summary>
        /// Bid Date
        /// </summary>
        public DateTime bidDate { get; set; }
        /// <summary>
        /// Book Date
        /// </summary>
        public DateTime bookDate { get; set; }
        /// <summary>
        /// Created By
        /// </summary>
        public User createdBy { get; set; }
        /// <summary>
        /// Created On
        /// </summary>
        public DateTime createdOn { get; set; }
        /// <summary>
        /// Modified By
        /// </summary>
        public User modifiedBy { get; set; }
        /// <summary>
        /// Modified On
        /// </summary>
        public DateTime modifiedOn { get; set; }
    }
}