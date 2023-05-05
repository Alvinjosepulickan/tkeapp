using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
    public class DAResponseModel
    {
        public string Message { get; set; }
        public string DaJobStatus { get; set; }
        public int StatusCode { get; set; }
        public string JobId { get; set; }

    }
}
