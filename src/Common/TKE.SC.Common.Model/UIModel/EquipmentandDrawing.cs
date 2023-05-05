using Configit.Configurator.Server.Common;
using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// EquipmentandDrawing
    /// </summary>
    public class EquipmentandDrawing
    {
        public List<Equipment> equipment { get; set; }
        public RequestedDrawing requestedDrawing { get; set; }
        public List<UnitVariablesAssignmentValue> SetConfigurationDetails { get; set; }
        public Dictionary<int,List<string>> UnitDictionary { get; set; }
    }
    public class UnitVariablesAssignmentValue
    {
        public int SetId { get; set; }
        public bool RearDoorSelected { get; set; }
        public string ProductName { get; set; }
        public List<VariableAssignment> VariableAssignments { get; set; }
    }
}