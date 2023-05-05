using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
    public class OpeningLocationDetails
    {
        public int UnitId { get; set; }
        public decimal TravelFeet { get; set; }
        public decimal TravelInch { get; set; }
        public string FloorNumber { get; set; }
        public string OccupiedSpaceBelow { get; set; }
        public bool Front { get; set; }
        public bool Rear { get; set; }
        public string FrontOpening { get; set; }
        public string RearOpening { get; set; }
    }
}
