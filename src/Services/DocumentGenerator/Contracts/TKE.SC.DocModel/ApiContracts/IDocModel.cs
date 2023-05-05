using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKE.SC.DocModel.ApiContracts
{
    public interface IDocModel
    {
        string Culture { get; set; }
        string Identifier { get; set; }
        string DocumentType { get; set; }
    }
}
