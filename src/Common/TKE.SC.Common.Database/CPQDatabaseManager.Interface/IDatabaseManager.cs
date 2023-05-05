using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Database.CPQDatabaseManager.Interface
{
    public interface IDatabaseManager
    {
        /// <summary>
        /// Run Query Related to IDataBase Manager Bl
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="environment"></param>
        /// <param Name="query"></param>
        /// <param Name="parameters"></param>
        /// <returns></returns>
        T RunQuery<T>(string environment, string query, IDictionary<string, object> parameters = null);
    }
}
