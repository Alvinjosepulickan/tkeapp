using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
	public interface IObomDL
	{
		DataSet GetvariableAssignmentsForObom(int setId);

		QuoteStatusReport GetConfigurationDetailsForStatusReport(string quoteId);
	}
}
