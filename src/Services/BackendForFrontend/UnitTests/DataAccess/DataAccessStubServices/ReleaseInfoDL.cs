using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    class ReleaseInfoStubDL : IReleaseInfoDL
    {
        public DetailsForReleaseToManufacture GetGroupReleaseInfo(int groupId, string sessionId)
        {
            if (groupId==45)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
            if (groupId == 0)
            {
                throw new NotImplementedException();
            }
            DetailsForReleaseToManufacture detailsReleaseManaufacture = new DetailsForReleaseToManufacture();
            detailsReleaseManaufacture.EnrichedData = JObject.Parse(File.ReadAllText(Constant.UNITENRICHEDDATA)); ;
            GroupDetailsReleaseToManufacture grp = new GroupDetailsReleaseToManufacture();
            grp.GroupId = groupId;
            grp.GroupName = "Elevator_Group";
            grp.ReadyToReleaseCheck = true;
            detailsReleaseManaufacture.GroupReleaseToManufacture = new GroupDetailsReleaseToManufacture() {GroupId = groupId,GroupName ="Elevator_Group"};
            detailsReleaseManaufacture.Permissions = new List<string>() { "Role", "Admin", "User" };
            return detailsReleaseManaufacture;
        }

        public List<Permissions> GetPermissionForReleaseInfo(string quoteId, string roleName, string Entity)
        {
            List<Permissions> permissionList = new List<Permissions>();
            Permissions pt = new Permissions();
            pt.BuildingStatus = "Checked";
            pt.Entity = "Elevator";
            pt.GroupStatus = "Checked";
            pt.PermissionKey = "Elevator_Key";
            pt.ProjectStage = "Completed";
            pt.UnitStatus = "NameStatus";
            permissionList.Add(pt);
            return permissionList;
        }

        public GroupDetailsForRelease GetProjectReleaseInfo(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new NotImplementedException();
            }
            else
            {
                List<ReleaseInfoBuildingDetails> listReleaseInfo = new List<ReleaseInfoBuildingDetails>();
                ReleaseInfoBuildingDetails relInfo = new ReleaseInfoBuildingDetails();
                relInfo.BuildingId = Convert.ToInt32(projectId);
                relInfo.BuildingName = "Elevator";
                //Groupdetails
                List<ReleaseInfoGroupDetails> groupDetails = new List<ReleaseInfoGroupDetails>();
                ReleaseInfoGroupDetails rlInfo = new ReleaseInfoGroupDetails();
                rlInfo.GroupId = 1;
                rlInfo.GroupName = "Elevator_Group";
                rlInfo.GroupStatus = new Status() { Description ="Checked",DisplayName="Status",StatusId =1,StatusKey ="Elevator_Key",StatusName ="NameStatus" };
                rlInfo.Permissions = new List<string>() {"Admin","User","Role"};
                rlInfo.ReleaseToManufacturing = true;
                rlInfo.UnitLength = 16;
                groupDetails.Add(rlInfo);
                relInfo.GroupDetails = new List<ReleaseInfoGroupDetails>();
                listReleaseInfo.Add(relInfo);
                GroupDetailsForRelease groupRelease = new GroupDetailsForRelease();
                groupRelease.GroupDetailsForReleaseInfo = listReleaseInfo;
                return groupRelease;
            }
        }

        public List<ResultGroupReleaseResponse> SaveUpdatReleaseInfoDetailsDL(int groupid, List<ReleaseInfoSetUnitDetails> listOfDetails, List<ReleaseInfoQuestions> listOfQueries, string userId, string actionFlag)
        {
            return new List<ResultGroupReleaseResponse> { new ResultGroupReleaseResponse { Result=1,Message="Release Info Updated"} };
        }
    }
}
