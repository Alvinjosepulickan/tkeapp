///************************************************************************************************************
//************************************************************************************************************
//    File Name     :   UnitsBLTests.cs 
//    Created By    :   Infosys LTD
//    Created On    :   01-JAN-2020
//    Modified By   :
//    Modified On   :
//    Version       :   1.0  
//************************************************************************************************************
//************************************************************************************************************/
//using Microsoft.Extensions.DependencyInjection;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using TKE.SC.BFF.BusinessProcess.Helpers;
//using Microsoft.Extensions.Logging;
//using TKE.SC.BFF.BusinessProcess.Services;
//using TKE.SC.BFF.DataAccess.Interfaces;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Test.BusinessProcess.Services
//{
//    public class UnitsBLTests
//    {

//        #region Variables
//        private UnitsBL _unitsbl;
//        private ILogger<UnitsBL> _unitsBLLogger;
//        #endregion

//        #region PrivateMethods
//        [SetUp]
//        public void InitialConfiguration()
//        {
//            CommonFunctions.InitialConfiguration();
//            var services = CommonFunctions.ServiceCollection();
//            _unitsBLLogger = services.BuildServiceProvider().GetService<ILogger<UnitsBL>>();
//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
//            var iproduct = (IUnitsDL)servicesProvider.GetService(typeof(IUnitsDL));
//            var ilogger = (ILogger)servicesProvider.GetService(typeof(ILogger));
//            _unitsbl = new UnitsBL(_unitsBLLogger, iproduct);
//        }
//        #endregion

//        /// <summary>
//        /// GetProjectBasedUnits1
//        /// </summary>
//        [Test]
//        public void GetProjectBasedUnits1()
//        {
//            var res = _unitsbl.GetProjectBasedUnits1("1");
//            Assert.IsNotNull(res);
//        }

//        /// <summary>
//        /// GetListOfUnitsForProject
//        /// </summary>
//        [Test]
//        public void GetListOfUnitsForProject()
//        {
//            var res = _unitsbl.GetListOfUnitsForProject("1");
//            Assert.IsNotNull(res);
//        }

//        /// <summary>
//        /// GetUnitConfigurationDetailsBL
//        /// </summary>
//        [Test]
//        public void GetUnitConfigurationDetailsBL()
//        {
//            var res = _unitsbl.GetUnitConfigurationDetailsBL("1");
//            Assert.IsNotNull(res);
//        }

//    }
//}
