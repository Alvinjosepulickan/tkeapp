using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// OZBookingRequest
    /// </summary>
    public class OzBookingRequest
    {
        public ProjectInformation ProjectInformation { get; set; }
        public List<Equipment> Equipment { get; set; }
        public RequestedDrawing RequestedDrawing { get; set; }
    }
}