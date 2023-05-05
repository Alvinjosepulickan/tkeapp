using Configit.Configurator.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.UnitTests.DataAccess.DataAccessStubServices
{
    class ObomStubDL : IObom
    {
        public ConfigurationRequest CreateConfigurationRequestWithTemplate(string packageName)
        {
            throw new NotImplementedException();
        }

        public async Task<ObomVariableAssignment> GetBuildingVariableAssignmentsForOBOM(ConfigurationDetails configurationDetails, string sessionId)
        {
            if (configurationDetails.GroupId != 0)
            {
                var getResponse = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.GETVARIABLEASSIGNMENTSOBOMRESPONSE));
                var response= Utility.DeserializeObjectValue<ObomVariableAssignment>(getResponse);
                return response;
            }
            else
            {
                throw new CustomException(new ResponseMessage { StatusCode = Constant.BADREQUEST, Message = "Invalid GroupId" });
            }
            
        }

        public Task<ResponseMessage> GETOBOMResponse(ConfigurationDetails configurationDetails, string sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage> OBOMPAckageCall(string packagePath, List<ObomVariables> obomVariableAssignments, List<VariableAssignment> variableAssignments, Line lineVariable, string sessionId, int numberOfFloors,bool isObom)
        {
            throw new NotImplementedException();
        }
    }
}
