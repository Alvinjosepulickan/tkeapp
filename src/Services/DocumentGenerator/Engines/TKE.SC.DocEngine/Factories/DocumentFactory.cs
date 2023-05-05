using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.DocModel.ApiContracts;

namespace TKE.CPQ.DocEngine.Factories
{
    //public class DocumentFactory : IDocumentFactory
    //{
    //    private readonly IServiceProvider _serviceProvider;

    //    public DocumentFactory(IServiceProvider serviceProvider)
    //    {
    //        _serviceProvider = serviceProvider;
    //    }
    //    public Stream CreatePdf(RequestModel requestModel)
    //    {
    //        switch (requestModel.Type)
    //        {
    //            case DocumentType.OrderForm:
    //               return ((OrderFormFactory)GetFactory(typeof(OrderFormFactory))).CreatePdf(requestModel);
    //            case DocumentType.InvoiceOrder:
    //            default:
    //                return null;
    //        }
    //    }

    //    private object GetFactory(Type type)
    //    {
    //        var factory = _serviceProvider.GetService(type);
    //        if( factory is null)
    //        {
    //            throw new NotImplementedException($"Requested Document type cant be generated.");
    //        }
    //        return factory;
    //    }
    //}
}
