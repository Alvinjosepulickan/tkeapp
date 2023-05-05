using Configit.Configurator.Server.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
	public interface IObom
	{
		Task<ResponseMessage> GETOBOMResponse(ConfigurationDetails configurationDetails, string sessionId);
		Task<ResponseMessage> GenerateExcelForStatusReport(ConfigurationDetails configurationDetails);
		Task<ObomVariableAssignment> GetBuildingVariableAssignmentsForOBOM(ConfigurationDetails configurationDetails, string sessionId);
		ConfigurationRequest CreateConfigurationRequestWithTemplate(string packageName);
		Task<ResponseMessage> OBOMPAckageCall(string packagePath, List<ObomVariables> obomVariableAssignments, List<VariableAssignment> variableAssignments, Line lineVariable, string sessionId, int numberOfFloors, bool isObom);
	}
}