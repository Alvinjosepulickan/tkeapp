using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// GroupDetailsForSendToCoordination
    /// </summary>
    public class GroupDetailsForSendToCoordination
    {
        public List<FieldDrawingBuildingDetail> groupDetailsForSendToCoordination { get; set; }
        public bool enableSendToCoordination { get; set; }
    }

    /// <summary>
    /// FieldDrawingBuildingDetail
    /// </summary>
    public class FieldDrawingBuildingDetail
    {
        public int buildingId { get; set; }
        public string buildingName { get; set; }
        public List<FieldDrawingGroupDetail> groupDetails { get; set; }
    }

    /// <summary>
    /// FieldDrawingGroupDetail
    /// </summary>
    public class FieldDrawingGroupDetail
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public bool isGroupSaved { get; set; }
        public List<CoordinationQuestions> questions { get; set; }
    }
}