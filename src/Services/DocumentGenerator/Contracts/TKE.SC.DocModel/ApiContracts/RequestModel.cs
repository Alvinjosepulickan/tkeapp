using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKE.SC.DocModel.ApiContracts
{
    public class RequestModel<T>
    {
        public T DocumentModel { get; set; }
    }
}
