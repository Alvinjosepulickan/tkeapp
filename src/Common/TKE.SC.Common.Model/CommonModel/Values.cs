namespace TKE.SC.Common.Model
{
    /// <summary>
    /// Values
    /// </summary>
    public class Values
    {
        public string id { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public object value { get; set; }
        /// <summary>
        /// Assigned
        /// </summary>
        public string Assigned { get; set; }
        /// <summary>
        /// InCompatible
        /// </summary>
        public bool InCompatible { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Justification
        /// </summary>
        public string Justification { get; set; }
        /// <summary>
        /// lower
        /// </summary>
        public string? lower { get; set; }
        /// <summary>
        /// upper
        /// </summary>
        public string? upper { get; set; }

    }
}
