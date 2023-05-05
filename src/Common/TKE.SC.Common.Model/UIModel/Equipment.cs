
namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// Equipment
    /// </summary>
    public class Equipment
    {
        public DesignOnDemand DesignOnDemand { get; set; }
        public General General { get; set; }
        public EstimateIdentifier EstimateIdentifier { get; set; }
    }

    /// <summary>
    /// DesignOnDemand
    /// </summary>
    public class DesignOnDemand
    {
        public bool IsDoD { get; set; }
        public bool OutForApproval { get; set; }
        public bool ForFinal { get; set; }
        public bool ForReviseResubmit { get; set; }
        public string SentDate { get; set; } = string.Empty;
        public string ReceivedDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// General
    /// </summary>
    public class General
    {
        public string Designation { get; set; }
        public int Units { get; set; }
        public Product Product { get; set; }
        public Model Model { get; set; }

    }

    /// <summary>
    /// Product
    /// </summary>
    public class Product
    {
        public string ProductLineIdName { get; set; }
    }

    /// <summary>
    /// Model
    /// </summary>
    public class Model
    {
        public string ProductModel { get; set; }
    }

    /// <summary>
    /// EstimateIdentifier
    /// </summary>
    public class EstimateIdentifier
    {
        public string LineId { get; set; }
    }
}