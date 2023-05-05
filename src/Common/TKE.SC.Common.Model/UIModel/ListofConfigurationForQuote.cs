using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Model.UIModel
{
	public class ListofConfigurationForQuote
	{
		public Configuration Configuration { get; set; }
		public ProjectDetail ProjectDetails { get; set; }
	}

    public class ConfigurationS
    {
        
        /// <summary>
        /// Permissions
        /// </summary>
        public List<string> Permissions { get; set; }
        /// <summary>
        /// ListOfConfiguration
        /// </summary>
        public ListofConfigurationForQuote ListOfConfiguration { get; set; }
    }
}
