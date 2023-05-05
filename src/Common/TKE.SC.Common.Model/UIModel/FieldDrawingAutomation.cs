using System;
using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// GroupDetailsForDrawingDetails
    /// </summary>
    public class GroupDetailsForDrawingDetails
    {
        public List<FieldDrawingBuildingDetails> GroupDetailsForDrawings { get; set; }
        public Nullable<bool> SendToCoordination { get; set; }
        public Nullable<bool> IsPrimaryQuote { get; set; }
    }

    /// <summary>
    /// FieldDrawingBuildingDetails
    /// </summary>
    public class FieldDrawingBuildingDetails
    {
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public List<FieldDrawingGroupDetails> GroupDetails { get; set; }
    }

    /// <summary>
    /// FieldDrawingGroupDetails
    /// </summary>
    public class FieldDrawingGroupDetails
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string ManualInfoMessage { get; set; }
        public string ProductKey { get; set; }
        public List<FieldDrawingUnitData> Units { get; set; }
        public Status GroupStatus { get; set; }
        public Status DrawingStatus { get; set; }
        public Status DesignAutomationStatus { get; set; }
        public List<string> Permissions { get; set; }
    }

    /// <summary>
    /// Status
    /// </summary>
    public class Status 
    {
        public int StatusId { get; set; }
        public string StatusKey { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// FieldDrawingUnitData
    /// </summary>
    public class FieldDrawingUnitData
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
    }

    /// <summary>
    /// DrawingGenerationDetails
    /// </summary>
    public class DrawingGenerationDetails
    {
        public string DrawingGenerationMethod { get; set; }
        public bool IsManualDrawingGeneration { get; set; }
        public string DrawingGenerationInfoMessage { get; set; }
        public string LatestDrawingStatus { get; set; }
        public bool IsGroupEditable { get; set; }
        public bool EditLayout { get; set; }
        public bool GroupLocked { get; set; }
    }

    /// <summary>
    /// RequestHistory
    /// </summary>
    public class RequestHistory
    {
        public List<RequestQueue> requestHistory { get; set; }
    }

    /// <summary>
    /// RequestQueue
    /// </summary>
    public class RequestQueue
    {
        public int Id { get; set; }
        public string StatusKey { get; set; }
        public string StatusName { get; set; }
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<FieldDrawingUnitData> Units { get; set; }
        public List<string> SelectedOutputTypes { get; set; }
        public List<string> SelectedDrawingTypes { get; set; }
        public string LastModified { get; set; }
        public int Version { get; set; }
        public string ModifiedBy { get; set; }
    }

    /// <summary>
    /// RequestBody
    /// </summary>
    public class RequestBody
    {
        public string TargetLanguage { get; set; }
        public List<string> OutputTypes { get; set; }
        public int PlotStyle { get; set; }
        public string LDLayoutXML { get; set; }
        public string ExternalSystemIdentifier { get; set; }
    }

    /// <summary>
    /// RequestLayoutInfo
    /// </summary>
    public class RequestLayoutInfo
    {
        public string ReferenceId { get; set; }
        public int FieldDrawingIntegrationMasterId { get; set; }
    }

    /// <summary>
    /// Reference
    /// </summary>
    public class Reference
    {
        public int IntegratedProcessId { get; set; }
        public string IntegratedSystemRef { get; set; }
    }

    /// <summary>
    /// RecurringJobData
    /// </summary>
    public class RecurringJobData
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
    }

    /// <summary>
    /// LaytouchDetails
    /// </summary>
    public class LaytouchDetails
    {
        public bool Laytouch { get; set; }
    }

    public class ProjectDet 
    {
        public string OpportunityId { get; set; }
        public int VersionId { get; set; }
    }
}
