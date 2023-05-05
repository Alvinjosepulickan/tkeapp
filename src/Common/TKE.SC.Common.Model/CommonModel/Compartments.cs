using System.Collections.Generic;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// Compartment
    /// </summary>
    public class Compartment
    {
        /// <summary>
        /// Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// value
        /// </summary>
        public string value { get; set; }
    }

    public class CompartmentsData
    {
        /// <summary>
        /// compartments
        /// </summary>
        public List<Compartment> compartments { get; set; }
    }
}