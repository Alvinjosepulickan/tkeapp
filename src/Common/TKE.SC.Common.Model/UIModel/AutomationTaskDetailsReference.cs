using Configit.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
    public class AutomationTaskDetailsReference
    {

        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Finished { get; set; }
        public string JobName { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public double PercentageCompleted { get; set; }
        public JobStatus Status { get; set; }
        public string Message { get; set; }
        public string StatusMessage { get; set; }
        public string PackageName { get; set; }
        public string UEID { get; set; }

    }
}
