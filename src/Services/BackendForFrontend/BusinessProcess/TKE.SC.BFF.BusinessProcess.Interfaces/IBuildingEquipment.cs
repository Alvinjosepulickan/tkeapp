using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IBuildingEquipment
    {
        public Task<ResponseMessage> StartBuildingEquipmentConfigure(JObject variableAssignments, int buildingId, string sessionId);

        public Task<ResponseMessage> SaveAssignGroups(int buildingId, List<BuildingEquipmentData> variableAssignments, string sessionId, int Is_Saved);

        public Task<ResponseMessage> DuplicateBuildingEquipmentConsole(int buildingId, int consoleId, string sessionId);

        public Task<ResponseMessage> DeleteBuildingEquipmentConsole(int buildingId, int consoleId, string sessionId);

        public Task<ResponseMessage> SaveBuildingEquipmentConfiguration(int buildingId, JObject variableAssignments, string sessionId, int Is_Saved, bool saveDraft);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <param Name="SessionId"></param>
        /// <param Name="fixtureSelected"></param>
        /// <param Name="entranceConsole"></param>
        /// <param Name="isSave"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartBuildingEquipmentConsole(int consoleId, int buildingId, string SessionId, BuildingEquipmentData objEntranceConfiguration = null, bool isSave = false, bool editFlag = false);

    }
}
