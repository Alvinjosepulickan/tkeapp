/************************************************************************************************************
************************************************************************************************************
    File Name     :   ListOfConfiguration.cs
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// ListOfConfiguration
    /// </summary>
    public class ListOfConfiguration
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// BuildingName
        /// </summary>
        public string BuildingName { get; set; }
        /// <summary>
        /// BuildingEquipmentStatus
        /// </summary>
        public string BuildingEquipmentStatus { get; set; }
        /// <summary>
        /// BuildingStatus
        /// </summary>
        public Status BuildingStatus { get; set; }
        /// <summary>
        /// ConflictsStatus
        /// </summary>
        public ConflictsStatus ConflictsStatus { get; set; }
        /// <summary>
        /// Groups
        /// </summary>
        public List<GrupConfiguration> Groups { get; set; }
        /// <summary>
        /// Building Permissions
        /// </summary>
        public List<string> Permissions { get; set; }
        public enum bldStatus
        {
            [Description("Not Applicable")]
            NotApplicable,
            [Description("Pending")]
            Pending,
            [Description("Completed")]
            Completed,
            [Description("Not Applicable")]
            BLDGEQP_UA,
            [Description("Pending")]
            BLDGEQP_AV,
            [Description("Completed")]
            BLDGEQP_COM
        };
    }

    /// <summary>
    /// GrupConfiguration
    /// </summary>
    public class GrupConfiguration
    {
        /// <summary>
        /// GroupId
        /// </summary>
        public int groupId { get; set; }
        /// <summary>
        /// GroupName
        /// </summary>
        public string groupName { get; set; }
        /// <summary>
        /// productCategory
        /// </summary>
        public string productCategory { get; set; }

        /// <summary>
        /// NeedsValidation
        /// </summary>
        public bool NeedsValidation { get; set; }
        /// <summary>
        /// ConflictsStatus
        /// </summary>
        public ConflictsStatus ConflictsStatus { get; set; }
        /// <summary>
        /// ConflictsStatus
        /// </summary>
        public Status groupStatus { get; set; }
        /// <summary>
        /// Units
        /// </summary>
        public List<Unit> Units { get; set; }
        /// <summary>
        /// Group Permissions
        /// </summary>
        public List<string> Permissions { get; set; }
    }

    /// <summary>
    /// Unit
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// UnitId
        /// </summary>
        public int unitId { get; set; }
        /// <summary>
        /// UnitName
        /// </summary>
        public string unitName { get; set; }
        /// <summary>
        /// ConflictsStatus
        /// </summary>
        public ConflictsStatus ConflictsStatus { get; set; }
        /// <summary>
        /// ProductName
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// productId
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Capacity
        /// </summary>
        public string capacity { get; set; }
        /// <summary>
        /// Capacity
        /// </summary>
        public string speed { get; set; }
        /// <summary>
        /// Landings
        /// </summary>
        public int Landings { get; set; }
        /// <summary>
        /// FrontOpenings
        /// </summary>
        public int FrontOpenings { get; set; }
        /// <summary>
        /// RearOpening
        /// </summary>
        public int RearOpening { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// status
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// SetId
        /// </summary>
        public int SetId { get; set; }
        /// <summary>
        /// SetName
        /// </summary>
        public string SetName { get; set; }
        /// <summary>
        /// UEID 
        /// </summary>
        public string UEID { get; set; }
        /// <summary>
        /// created date
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Unit Permissions
        /// </summary>
        public List<string> Permissions { get; set; }
        /// <summary>
        /// UnitPosition
        /// </summary>
        public string UnitPosition { get; set; }
       
        public Factory Factory { get; set; }
    }

    /// <summary>
    /// ConfigurationScreen
    /// </summary>
    public class ConfigurationScreen
    {
        /// <summary>
        /// IsViewUser
        /// </summary>
        public Boolean IsViewUser { get; set; }
        /// <summary>
        /// Permissions
        /// </summary>
        public List<string> Permissions { get; set; }
        /// <summary>
        /// ListOfConfiguration
        /// </summary>
        public List<ListOfConfiguration> ListOfConfiguration { get; set; }
    }

    /// <summary>
    /// Permissions
    /// </summary>
    public class Permissions
    {
        /// <summary>
        /// Entity
        /// </summary>
        public string Entity { get; set; }
        /// <summary>
        /// PermissionKey
        /// </summary>
        public string PermissionKey { get; set; }
        /// <summary>
        /// ProjectStage
        /// </summary>
        public string ProjectStage { get; set; }
        /// <summary>
        /// BuildingStatus
        /// </summary>
        public string BuildingStatus { get; set; }
        /// <summary>
        /// GroupStatus
        /// </summary>
        public string GroupStatus { get; set; }
        /// <summary>
        /// UnitStatus
        /// </summary>
        public string UnitStatus { get; set; }
    }
    public class Factory
    {
        /// <summary>
        /// FactoryJobId 
        /// </summary>
        public string FactoryJobId { get; set; }
        /// <summary>
        /// Is Read only Flag
        /// </summary>
        public bool IsReadOnly { get; set; }
    }
}