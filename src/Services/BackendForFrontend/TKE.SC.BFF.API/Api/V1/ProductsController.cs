using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TKE.SC.BFF.Helper;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model.UIModel;
using Microsoft.Extensions.Logging;
using TKE.SC.BFF.Controllers;
using Newtonsoft.Json.Linq;
using TKE.SC.Common;

namespace TKE.SC.BFF.APIController
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        #region Variables
        private readonly IProductSelection _productSelection;
        private readonly IConfigure _configure;
        #endregion

        /// <summary>
        /// ProductSelectionController
        /// </summary>
        /// <param Name="productSelection"></param>
        /// <param Name="configure"></param>
        /// <param Name="logger"></param>
        public ProductsController(IProductSelection productSelection, IConfigure configure, ILogger<ProductsController> logger)
        {
            _productSelection = productSelection;
            _configure = configure;
            ProductSelection productSelectionRequestBody = new ProductSelection();
            productSelectionRequestBody.UnitId = new List<int>();
            Utility.SetLogger(logger);
        }

        /// <summary>
        ///  Product Selection api
        /// </summary>
        /// <param Name="modelNumber"></param>
        /// <param Name="parentCode"></param>
        /// <param Name="locale"></param>
        /// <param Name="UnitId"></param>
        [Route("initiate")]
        [HttpPost]
        public async Task<IActionResult> ProductSelection(string modelNumber, string parentCode, [FromBody] List<int> unitId, string locale = null)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            _ = await _productSelection.UnitSetValidation(unitId);
            var unitDetails= await _productSelection.GetUnitVariableAssignments(unitId, sessionId,Constants.PRODUCTSELECTION).ConfigureAwait(false);
            JObject variableAssignments = unitDetails.Response;

            var result = await _configure.GetAvailableProducts(variableAssignments, sessionId, locale, unitId, modelNumber).ConfigureAwait(false);
           
            Utility.LogEnd(methodBegin);
            return Ok(JObject.FromObject(result));
        }

        /// <summary>
        ///  save Product Selection api
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productSelectionRequestBody"></param>
        [HttpPost]
        public async Task<IActionResult> SaveProductSelection([FromRoute] int groupConfigurationId, [FromBody] ProductSelection productSelectionRequestBody)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _productSelection.SaveProductSelection(groupConfigurationId, productSelectionRequestBody, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.ResponseArray);
        }
    }
}
