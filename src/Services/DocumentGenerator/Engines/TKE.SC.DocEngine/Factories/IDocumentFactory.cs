using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.DocModel.ApiContracts;

namespace TKE.CPQ.DocEngine.Factories
{
    public interface IDocumentFactory<T>
    {
        Stream CreatePdf(RequestModel<T> requestModel);
    }
}
