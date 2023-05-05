/************************************************************************************************************
************************************************************************************************************
    File Name     :   AccessBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/


using Microsoft.Extensions.DependencyInjection;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.Test.Common;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class AccessBLTests
    {

        #region Variables
        private AccessBL _accessBL;
        #endregion

        //#region PrivateMethods
        //[SetUp]

        //public void InitialConfiguration()
        //{
        //    CommonFunctions.InitialConfiguration();
        //    var services = CommonFunctions.ServiceCollection();
        //    var servicesProvider = services.BuildServiceProvider().GetService<System.IServiceProvider>();
        //    var iBuilding = (IAccessDL)servicesProvider.GetService(typeof(IAccessDL));
        //    var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
        //    _accessBL = new BuildingConfigurationBL(iBuilding, iConfigure);
        //}
    }
}
