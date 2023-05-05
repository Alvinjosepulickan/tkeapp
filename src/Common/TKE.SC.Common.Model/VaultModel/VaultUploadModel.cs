using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.VaultModel
{
    public class VaultUploadModel
    {
        public VaultFileUploadInfo[] Payload { get; set; }
    }

    public class VaultFileUploadInfo
    {
        public string ProjectViewID { get; set; }
        public string QuoteID { get; set; }
        public Estimate[] Estimates { get; set; }
        public Layouttype[] LayoutTypes { get; set; }
        public string MFClassName { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string HasErrors { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Estimate
    {
        public string ExternalEstimateId { get; set; }
    }

    public class Layouttype
    {
        public string LayoutType { get; set; }
    }
}
