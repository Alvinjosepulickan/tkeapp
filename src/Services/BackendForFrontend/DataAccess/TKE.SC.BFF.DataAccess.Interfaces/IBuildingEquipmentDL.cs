using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
   public  interface IBuildingEquipmentDL
    {
        public List<BuildingEquipmentData> GetBuildingEquipmentConsoles(int buildingId, string userName, string sessionId);

        List<ConfigVariable> GetBuildingEquipmentConfigurationByBuildingId(int buildingId, DataTable configVariables);

        public List<Result> SaveAssignGroups(int buildingId, int consoleId, BuildingEquipmentData buildingEquipmentConfigurationData, string userId, int is_Saved, List<LogHistoryTable> historyTable);

        public List<Result> DuplicateBuildingEquipmentConsole(int buildingId, int consoleId, string userId);

        public List<Result> DeleteBuildingEquipmentConsole(int buildingId, int consoleId, string userId,List<LogHistoryTable> historyTable);

        public List<Result> SaveBuildingEquipmentConfiguration(int buildingId, List<ConfigVariable> buildingEquipmentConfigurationData, string userId, int is_Saved);
    }
}
