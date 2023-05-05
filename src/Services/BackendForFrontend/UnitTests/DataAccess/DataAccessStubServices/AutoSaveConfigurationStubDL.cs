using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class AutoSaveConfigurationStubDL : IAutoSaveConfigurationDL
    {
        public int AutoSaveConfiguration(AutoSaveConfiguration autoSaveRequest)
        {
            if (autoSaveRequest == null)
            {
                throw new NotImplementedException();
            }
            return 0;
        }

        public int DeleteAutoSaveConfigurationByUser(string userName)
        {
            if (userName == null)
            {
                throw new NotImplementedException();
            }
            else if(userName == "test")
            {
                return 0;
            }
            return 1;
        }

        public AutoSaveConfiguration GetAutoSaveConfigurationByUser(string userName)
        {
            if (userName == null)
            {
                return null;
            }
            else if (userName == "c2duser")
            {
                AutoSaveConfiguration resAutoSave1 = new AutoSaveConfiguration();
                resAutoSave1.UserName = "Test";
                resAutoSave1.CreatedOn = Convert.ToDateTime("2 / 1 / 0001 12:00:00 AM");
                return resAutoSave1;
            }
            AutoSaveConfiguration resAutoSave = new AutoSaveConfiguration();
            resAutoSave.UserName = "Test";
            return resAutoSave;
        }
    }
}
