using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.BFF.DataAccess.Helpers;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class ProductSelectionStubDL : IProductSelectionDL
    {
        public int SaveProductSelection(int groupConfigurationId, ProductSelection productSelection, string businessLine, string country, string controLanding)
        {
            if (groupConfigurationId == 0)
            {
                return 0;
            }
            return 1;
        }

        public List<UnitDetails> GetUnitDetailsForProductSelection(int groupConfigurationId)
        {
            if (groupConfigurationId == 0)
            {
                return null;
            }
            List<UnitDetails> productSelectionList = new List<UnitDetails>();
            return productSelectionList;
        }

        public List<ConfigVariable> GetUnitVariableAssignments(List<int> unitId)
        {
            if (unitId.Count == 0)
            {
                throw new System.NotImplementedException();
            }
            List<ConfigVariable> result = new List<ConfigVariable>();
            ConfigVariable res = new ConfigVariable();
            res.VariableId = "id";
            res.Value = 1;
            result.Add(res);
            return result;
        }

        public int UnitSetValidation(List<int> unitId)
        {
            throw new System.NotImplementedException();
        }

        public int SaveProductSelection(int groupConfigurationId, ProductSelection productSelection, string businessLine, string country, string controlLanding, string fixtureStrategy)
        {

            if (groupConfigurationId == 0)
            {
                return 0;
            }
            List<ProductSelection> pSlection = new List<ProductSelection>();
            ProductSelection ps = new ProductSelection();
            string uHFDefaults = string.Empty;
            string entranceDefaults = string.Empty;
            if (productSelection.productSelected == TKE.SC.BFF.DataAccess.Helpers.Constant.EVO200)
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(TKE.SC.Common.Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, TKE.SC.BFF.DataAccess.Helpers.Constant.EVOLUTION200))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(TKE.SC.Common.Constants.ENTRANCESCONSOLEDEFAULTVALUES, TKE.SC.BFF.DataAccess.Helpers.Constant.EVOLUTION200))).ToString();

            }
           else if (productSelection.productSelected.Equals(TKE.SC.BFF.DataAccess.Helpers.Constant.ENDURA100))
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(TKE.SC.Common.Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, TKE.SC.Common.Constants.END100))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(TKE.SC.Common.Constants.ENTRANCESCONSOLEDEFAULTVALUES, TKE.SC.Common.Constants.END100))).ToString();

            }
            else if (productSelection.productSelected.Equals(TKE.SC.BFF.DataAccess.Helpers.Constant.EVO100))
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(TKE.SC.Common.Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, TKE.SC.Common.Constants.EVOLUTION100))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(TKE.SC.Common.Constants.ENTRANCESCONSOLEDEFAULTVALUES, TKE.SC.Common.Constants.EVOLUTION100))).ToString();
            }
            var defaultUHFVariables = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(uHFDefaults);
            var defaultEntranceConfigurationValues = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(entranceDefaults);
            if (productSelection.productSelected == null)
            {
                productSelection.productSelected = string.Empty;
                return 0;
            }
            return 1;
        }
        
           
        

        int IProductSelectionDL.UnitSetValidation(List<int> unitId)
        {
            int x = 0;
            if(unitId.Count>0)
            {
                return 1;
            }
            return x;
        }

        List<ConfigVariable> IProductSelectionDL.GetUnitVariableAssignments(List<int> unitId, string identifier)
        {
            List<ConfigVariable> x = null;
            if (unitId.Count > 0)
            {
                return new List<ConfigVariable> { new ConfigVariable { Value = 1, VariableId = "PRODUCTTYPE" } };
            }
            return x;
        }

        public int SaveProductSelection(int groupConfigurationId, ProductSelection productSelection, string businessLine, string country, string controlLanding, string fixtureStrategy, string supplyingFactory)
        {
            int result=0;
            if (groupConfigurationId>0)
            {
                result = 1;
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = BFF.BusinessProcess.Helpers.Constant.BADREQUEST
                });
            }
            return result;
        }
    }
}
