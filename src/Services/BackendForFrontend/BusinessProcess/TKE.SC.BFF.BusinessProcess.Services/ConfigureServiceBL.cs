/************************************************************************************************************
************************************************************************************************************
    File Name     :   ConfigureServiceBL.cs
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/


using Configit.Configurator.Server.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class ConfigureServiceBL : IConfigureServices
    {
        /// <summary>
        /// Cunstructor class for GetBaseConfigureRequest
        /// </summary>
        /// <param Name="baseConfigureRequest"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="model"></param>
        /// <returns></returns>
        public ConfigurationRequest GetBaseConfigureRequest(ConfigurationRequest baseConfigureRequest,
            string model = null, bool configServiceRequest = false,
            string machinePackagePath = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var configureRequest = new ConfigurationRequest()
            {
                Currency = baseConfigureRequest.Currency,
                CurrentLineId = baseConfigureRequest.CurrentLineId,
                Date = baseConfigureRequest.Date,
                GlobalArguments = baseConfigureRequest.GlobalArguments,
                Language = baseConfigureRequest.Language,
                PackagePath = baseConfigureRequest.PackagePath,
                PriceProcedureId = baseConfigureRequest.PriceProcedureId,
                Settings = baseConfigureRequest.Settings,
                ViewId = baseConfigureRequest.ViewId,
              
            };
            var line = new Line
            {
                Arguments = baseConfigureRequest.Line?.Arguments,
                IsComplete = baseConfigureRequest.Line?.IsComplete,
                PriceLineAssignments = baseConfigureRequest.Line?.PriceLineAssignments,
                PriceSheet = baseConfigureRequest.Line?.PriceSheet,
                Product = baseConfigureRequest.Line?.Product,
                ProductId = baseConfigureRequest.Line?.ProductId,
                Quantity = baseConfigureRequest.Line?.Quantity,
                StateHash = baseConfigureRequest.Line?.StateHash,
                Sublines = baseConfigureRequest.Line?.Sublines,
                VariableAssignments = new List<VariableAssignment>()
            };
            if (configServiceRequest)
            {
                //Adding model and model range in the config request
                var genModel = new VariableAssignment { VariableId = Constant.GEN_S_MODEL, Value = model };
                var genModelRange = new VariableAssignment { VariableId = Constant.GEN_S_MODEL_RANGE, Value = machinePackagePath };
                var variableAssignment = (new List<VariableAssignment>() { genModel, genModelRange });
                variableAssignment.AddRange(UpdateVariableAssignments(baseConfigureRequest));
                line.VariableAssignments = (variableAssignment);
                configureRequest.Line = line;
                //configureRequest.GlobalArguments =
                   // UpdateVariants(configureRequest.GlobalArguments);
                return configureRequest;
            }
            var variableAssignments = UpdateVariableAssignments(baseConfigureRequest);
            line.VariableAssignments = variableAssignments;
            configureRequest.Line = line;
            //configureRequest.GlobalArguments =
            //UpdateVariants(configureRequest.GlobalArguments);
            Utility.LogEnd(methodBeginTime);
            return configureRequest;
        }

        /// <summary>
        /// Updating country variable assignment
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        private static List<VariableAssignment> UpdateVariableAssignments(ConfigureRequest configureRequest)
        {
            var methodBeginTime = Utility.LogBegin();
            var variableAssignments =  new List<VariableAssignment>(configureRequest.Line?.VariableAssignments);
            Utility.LogEnd(methodBeginTime);
            return variableAssignments;
        }

        /// <summary>
        /// Constructor class for Update Variants
        /// </summary>
        /// <param Name="globalArgumentsReq"></param>
        /// <param Name="modelNumber"></param>
        /// <returns></returns>
        private static Dictionary<string, object> UpdateVariants(IDictionary<string, object> globalArgumentsReq,
            string modelNumber)
        {
            var variants = new List<string>
            {
                modelNumber
            };
            return globalArgumentsReq.ToDictionary(globalArgument => globalArgument.Key,
                globalArgument => Utility.CheckEquals(globalArgument.Key, Constant.VARIANTS)
                    ? variants
                    : globalArgument.Value);
        }

        /// <summary>
        /// This method verifies the completeness of configuration and returns is complete or not
        /// </summary>
        /// <param Name="configureResponse"></param>
        /// <returns>returns whether the configuration  is complete or not</returns>
        public bool IsComplete(ConfigureResponse configureResponse)
        {
            var methodBeginTime = Utility.LogBegin();
            var requiredProperties = from section in configureResponse.Sections
                                     from variable in section.Variables
                                     from property in variable.Properties
                                     where Utility.CheckEquals(property.Id, Constant.REQUIRED) && property.Value is true
                                     select variable;
            var configurationVariables = requiredProperties as ConfigurationVariable[] ?? requiredProperties.ToArray();
            var assignedVariables = from val in configurationVariables
                                    from value in val.Values
                                    where value.Assigned != null
                                    select value;
            Utility.LogEnd(methodBeginTime);
            return configurationVariables.Length == assignedVariables.Count();
        }

    }
}
