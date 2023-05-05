using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKE.SC.DocModel.ApiContracts
{
    public class BaseModel: IDocModel
    {
        public string Culture { get; set; }
        public string Identifier { get; set; }
        public string DocumentType { get; set; }
        
    }
}
