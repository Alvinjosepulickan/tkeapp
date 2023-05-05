using System.Collections.Generic;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IReleaseInfoDL
    {
        GroupDetailsForRelease GetProjectReleaseInfo(string projectId);
        DetailsForReleaseToManufacture GetGroupReleaseInfo(int groupId, string sessionId);
        List<ResultGroupReleaseResponse> SaveUpdatReleaseInfoDetailsDL(int groupid, List<ReleaseInfoSetUnitDetails> listOfDetails, List<ReleaseInfoQuestions> listOfQueries, string userId, string actionFlag);
        List<Permissions> GetPermissionForReleaseInfo(string quoteId, string roleName,string Entity);
    }
}