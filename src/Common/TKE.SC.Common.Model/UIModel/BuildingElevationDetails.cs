using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
    public class BuildingElevationDetails
    {
        public string FloorNumber { get; set; }
        public decimal ElevationFeet { get; set; }
        public decimal ElevationInch { get; set; }
        public decimal FloorToFloorHeightFeet { get; set; }
        public decimal FloorToFloorHeightInch { get; set; }
    }
}
