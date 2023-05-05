using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TKE.CPQ.DocEngine.Factories;
using TKE.SC.DocModel.ApiContracts;

namespace TKE.CPQ.DocGateway.Controllers
{
    [ApiController]
    public class OrderFormController : ControllerBase
    {
        private readonly IDocumentFactory<OrderFormModel> _docFactory;

        public OrderFormController(IDocumentFactory<OrderFormModel> docFactory)
        {
            _docFactory = docFactory;
        }
        [Route("api/orderforms/unit")]
        [HttpPost]
        public IActionResult GenerateOrderForm(RequestModel<OrderFormModel> docModel)
        {
                var fileStream = _docFactory.CreatePdf(docModel);
                return new FileStreamResult(fileStream, "application/pdf");
        }
    }
}
