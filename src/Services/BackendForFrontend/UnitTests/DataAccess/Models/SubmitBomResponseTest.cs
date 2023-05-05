using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.BFF.UnitTests.DataAccess.Models
{
    public class SubmitBomResponseTest
    {

        public List<SubmitBomResponseLineTest> Items { get; set; }


    }
    public class SubmitBomResponseLineTest
    {

        public string MaterialName { get; set; }
        public int ConfigurationId { get; set; }
        public AutomationTaskDetailsTest TaskDetails { get; set; }
    }
    public class AutomationTaskDetailsTest
    {

        [JsonIgnore]
        public string Id { get; set; } = "";
        [JsonIgnore]
        public DateTime Created { get; set; } = new DateTime();
        [JsonIgnore]
        public DateTime? Started { get; set; } = new DateTime();
        [JsonIgnore]
        public DateTime? Finished { get; set; } = new DateTime();
        public string JobName { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public double PercentageCompleted { get; set; }
        public JobStatusTest Status { get; set; }
        public string Message { get; set; }

    }
    public enum JobStatusTest
    {
        Aborted = 65,
        Created = 67,
        Failed = 70,
        InProgress = 73,
        Failing = 76,
        Pending = 80,
        Succeeded = 83,
        Aborting = 84
    }
    

}
