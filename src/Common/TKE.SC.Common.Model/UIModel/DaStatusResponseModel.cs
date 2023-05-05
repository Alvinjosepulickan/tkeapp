using Configit.TKE.DesignAutomation.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
    public class DaStatusResponseModel
    {
        public string DaJobStatus { get; set; }
        public int StatusCode { get; set; }
        public List<AutomationTaskDetailsReference> IndividualJobStatus { get; set; }
    }
}
