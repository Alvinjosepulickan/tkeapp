using System;
using System.Collections.Generic;
using Configit.Configurator.Server.Common;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// TP2Summary
    /// </summary>
    public class TP2Summary
    {
        public List<VariablesList> VariableAssignments { get; set; }
        public List<UnitDetailsForTP2> UnitDetails { get; set; }
        public VariableAssignment TravelVariableAssignments { get; set; }
        public List<UnitVariablesDetailsForTP2> UnitLevelVariables { get; set; }
        public int GroupId { get; set; }
        public List<OpeningVariables> OpeningVariableAssginments { get; set; }
        public ProjectInfo? projectInfo { get; set; }
        public List<ChangeLog> ChangedData { get; set; }
        public List<QuoteSummary> QuoteSummary { get; set; }
        public ProjectData ProjectData { get; set; }
        public List<UnitNames> GroupUnitInfo { get; set; }
        public string MainEgress { get; set; }
        public string AlternateEgress { get; set; }
        public int TopFloor { get; set; }
        public List<PriceSectionDetails> FloorMatrixTable { get; set; }
        public int NoOfInconRisersFront { get; set; }
        public int NoOfInconRisersRear { get; set; }
        public List<DiscountDataPerUnit> PriceAndDiscountData { get; set; }
        public List<CustomPriceLine> CustomPriceLine { get; set; }
    }

    /// <summary>
    /// OpeningVariables
    /// </summary>
    public class OpeningVariables
    {
        public ConfigVariable VariableAssigned { get; set; }
        public string OpeningsAssigned { get; set; }
        public int TotalOpenings { get; set; }
        public int PriceKey { get; set; }
    }

    /// <summary>
    /// UnitDetailsForTP2
    /// </summary>
    public class UnitDetailsForTP2
    {
        /// <summary>
        /// UnitId
        /// </summary>
        public int UnitId { get; set; }
        /// <summary>
        /// unitname
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// ueid 
        /// </summary>
        public string Ueid { get; set; }
        /// <summary>
        /// ProductName
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// FactoryJobID
        /// </summary>
        public string FactoryJobID { get; set; }
    }

    /// <summary>
    /// UnitVariablesDetailsForTP2
    /// </summary>
    public class UnitVariablesDetailsForTP2 : UnitDetailsForTP2
    {
        /// <summary>
        /// VariablesDetails
        /// </summary>
        public List<ConfigVariable> VariablesDetails { get; set; }
    }



    public class ProjectInfo
    {
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public string Branch { get; set; }
        public string BuildingName { get; set; }
        public string QuoteVersion { get; set; }
        public string PrimarySalesRep { get; set; }
        public string GroupName { get; set; }
        public string OracleProjectId { get; set; }
        public string PrimaryCoordinator { get; set; }
        public List<string> UnitName { get; set; }
        public string UnitMFGJobNo { get; set; }
        public string Status { get; set; }
        public string Source { get; set; }
        public string FrontOpenings { get; set; }
        public string RearOpenings { get; set; }
        public string Travel { get; set; }
        public string ProjectStatus { get; set; }
        public string QuoteId { get; set; }
    }
    public class ChangeLog
    {
        public string ChangeType { get; set; }
        public string User { get; set; }
        public DateTime ChangedDate { get; set; }
        public string Data { get; set; }
    }
    public class QuoteSummary
    {
        public string BuildingId { get; set; }
        public string BuildingName { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string UnitName { get; set; }
        public string NumberOfUnits { get; set; }
        public string OrderType { get; set; }
        public string ProductLine { get; set; }
        public string SetId { get; set; }
    }
    public class ProjectData
    {
        public string OpportunityId { get; set; }
        public string VersionId { get; set; }
        public string CustomerAccount { get; set; }
        public string Contact { get; set; }
        public string SalesStage { get; set; }
        public string ProjectStatus { get; set; }
        public string QuoteDate { get; set; }
        public string BookeDate { get; set; }
        public string Address { get; set; }
        public string QuoteStatus { get; set; }
    }

    public class DiscountDataPerUnit
    {
        public VariableAssignment VariableForUnit { get; set; }
        public int Unitid { get; set; }
    }
}
