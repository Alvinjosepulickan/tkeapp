using Configit.TKE.OrderBom.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TKE.SC.BFF.UnitTests.DataAccess.Models
{
    public class OrderItemTest 
    {
        public List<string> OutputLocations { get; set; }
        public List<string> ExportTypes { get; set; }
        public string AssemblyFile { get; set; }
        public string Unit { get; set; }
        [JsonIgnore]
        public decimal Cost { get; set; }
        [JsonIgnore]
        public decimal Quantity { get; set; }
        public string Description { get; set; }
        public string MaterialNumber { get; set; }
        public List<OrderAssignment> Properties { get; set; }
        public List<OrderItemTest> Children { get; set; }
        public List<OrderAssignment> Characteristics { get; set; }
        public List<OrderAssignmentWithProps> UniqueAssignments { get; }
        public List<OrderAssignmentWithProps> MatchedAssignments { get; }
        public List<OrderAssignmentWithProps> Assignments { get; set; }
        public int Level { get; set; }
        public List<string> Categories { get; set; }
        [JsonIgnore]
        public int ConfigurationId { get; }
    }

    public class CreateBomResponseTest
    {

        public OrderBomTest[] Lines { get; set; }
    }

    public class OrderBomTest
    {

        public OrderItemTest[] Items { get; set; }
    }
}

