using System.Collections.Generic;

namespace TKE.SC.Common.Model.UIModel
{
    /// <summary>
    /// SendToCoordinationData
    /// </summary>
    public class SendToCoordinationData
    {
        public int groupId { get; set; }
        public List<CoordinationQuestions> questions { get; set; }
    }

    /// <summary>
    /// CoordinationQuestions
    /// </summary>
    public class CoordinationQuestions
    {
        public string id { get; set; }
        public string name { get; set; }
        public string questionType { get; set; }
        public List<option> options { get; set; }
        public string value { get; set; }
        public List<propObject> properties { get; set; }
    }

    /// <summary>
    /// propObject
    /// </summary>
    public class propObject
    {
        public string id { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    /// <summary>
    /// option
    /// </summary>
    public class option
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}