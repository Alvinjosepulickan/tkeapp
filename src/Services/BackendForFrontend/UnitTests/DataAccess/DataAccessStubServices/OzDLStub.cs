using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    class OzDLStub : IOzDL
    {
        public async Task<ResponseMessage> BookCoOrdination(string quoteId, string sessionId, OzBookingRequest ozBookingRequest)
        {
            if (string.IsNullOrEmpty(quoteId) && ozBookingRequest == null)
            {
                return new ResponseMessage
                {
                    StatusCode = BFF.BusinessProcess.Helpers.Constant.BADREQUEST,
                    Description = "The Book coordination is not saved"
                };
            }
            else
            {
                return new ResponseMessage
                {
                    StatusCode = BFF.BusinessProcess.Helpers.Constant.SUCCESS,
                    Description = "The Book coordination saved"
                };
            }
                    
        }

        public int GetBranchId(string name)
        {
            if (name == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public EquipmentandDrawing GetEquipmentAndDrawingForOZ(string opportunityId)
        {
            if (string.IsNullOrEmpty(opportunityId))
            {
                throw new NotImplementedException();
            }
            EquipmentandDrawing eq = new EquipmentandDrawing();
            List<Equipment> equipment = new List<Equipment>();
            //Assigning equipment
            Equipment equip = new Equipment();
            equip.DesignOnDemand = new DesignOnDemand()
            {
                IsDoD = true,
                ForFinal = true,
                ForReviseResubmit = true,
                ReceivedDate = DateTime.Today.ToLongTimeString(),
                OutForApproval = true,
                SentDate = DateTime.Today.ToLongDateString()
            };
            equip.EstimateIdentifier = new EstimateIdentifier() { LineId = "50" };
            equip.General = new General()
            {
                Product = new Product() {ProductLineIdName = "EVO_200" },
                Designation = "Sales",
                Model = new Model() { ProductModel = "EVO_200" },
                Units = 6
            };
            equipment.Add(equip);
            eq.equipment = equipment;

            //Assiggning unit variable values
            List<UnitVariablesAssignmentValue> unitVariables = new List<UnitVariablesAssignmentValue>();
            UnitVariablesAssignmentValue unitValue = new UnitVariablesAssignmentValue();
            unitValue.ProductName = "CE_Geared";
            unitValue.RearDoorSelected = true;
            unitValue.SetId = 1;
            unitValue.VariableAssignments = new List<Configit.Configurator.Server.Common.VariableAssignment>() { new Configit.Configurator.Server.Common.VariableAssignment() {
                Priority = 1,Exclude = false,Value="check",VariableId ="435abcde" } };
           // unitVariables.Add(unitValue);
            eq.SetConfigurationDetails = unitVariables;
            
            RequestedDrawing requestedDrawing = new RequestedDrawing();
            eq.requestedDrawing = requestedDrawing;

            Dictionary<int, List<string>> unitDictionary = new Dictionary<int, List<string>>();
            List<string> listValues = new List<string>() { "Elevator1", "50", "Elevator3" };
            unitDictionary.Add(222, listValues);
            eq.UnitDictionary = unitDictionary;
            return eq;

        }

        public Task<string> GetOzToken(string sessionId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetProjectIdVersionId(string quoteId)
        {
            Dictionary<string, string> versionId = new Dictionary<string, string>();
            if (quoteId == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                versionId.Add("OpportunityId", quoteId);
                versionId.Add("VersionId", "123");
                return versionId;
            }
        }

        public int SaveSentToCoordinationWorkflowstatusforQuote(string quoteId)
        {
            throw new NotImplementedException();
        }
    }
}
