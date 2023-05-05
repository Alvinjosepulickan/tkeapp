using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
    public class DaJobDetails
    {
        public string DaJobId { get; set; }
        public string PackageName { get; set; }
        public string  DaJobStatus { get; set; }
        public string  PackageError { get; set; }
    }

    public enum DaStatus
    {
        DA_FAIL=70,
        DA_ABT = 65,
        DA_INPRO = 73,
        DA_PEN = 80,
        DA_SCS = 83,
        DA_INI_ST=45,
        DA_INI_CMP=50
    }
}
