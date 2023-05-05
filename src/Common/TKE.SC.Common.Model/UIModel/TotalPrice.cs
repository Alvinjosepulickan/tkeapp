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

namespace TKE.SC.Common.Model.UIModel
{
    public class TotalPrice
    {
        /// <summary>
        /// Currency
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        public decimal price { get; set; }
    }
}