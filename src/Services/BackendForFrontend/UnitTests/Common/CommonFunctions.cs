using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.IO;
using Newtonsoft.Json;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.Common.Model;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.Test.DataAccess.DataAccessStubServices;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using Microsoft.AspNetCore.Http;
using TKE.SC.BFF.Test.CPQCacheManager.CPQCacheMangerStubServices;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Hosting;
using TKE.SC.BFF.UnitTests.DataAccess.DataAccessStubServices;
//using Microsoft.Extensions.Hosting;

namespace TKE.SC.BFF.Test.Common
{
    public static class CommonFunctions
    {
        #region

        private static IConfigurationRoot _configurationRoot;
        private static IStringLocalizer<GenerateTokenBL> _localizer;

        #endregion

        public static IOptions<ParamSettings> InitialConfiguration()
        {
            _configurationRoot = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build();
            var services = new ServiceCollection().AddOptions();
            services.Configure<ParamSettings>(_configurationRoot.GetSection(Constant.PARAMSETTINGS));
            var options = services.BuildServiceProvider().GetService<IOptions<ParamSettings>>();
            return options;
        }

        public static IServiceCollection ServiceCollection()
        {
            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var services = new ServiceCollection().AddLogging();
            var option = Microsoft.Extensions.Options.Options.Create(new LocalizationOptions());  // you should not need any params here if using a StringLocalizer<T>
            var factory = new ResourceManagerStringLocalizerFactory(option, NullLoggerFactory.Instance);
            var localizer = new StringLocalizer<GenerateTokenBL>(factory);
            _localizer = localizer;
            _ = services.AddSingleton(_localizer)
                .AddSingleton<IAccess, AccessBL>()
                .AddSingleton<IAuth, AuthBL>()
                .AddSingleton<IBuildingConfiguration, BuildingConfigurationBL>()
                .AddSingleton<IConfigure, ConfigureBL>()
                .AddSingleton<IConfigureServices, ConfigureServiceBL>()
                .AddSingleton<IGenerateToken, GenerateTokenBL>()
                .AddSingleton<IGroupConfiguration, GroupConfigurationBL>()
                .AddSingleton<IGroupLayout, GroupLayoutBL>()
                .AddSingleton<IProduct, ProductBL>()
                .AddSingleton<IProductSelection, ProductSelectionBL>()
                .AddSingleton<IProject, ProjectsBL>()
                //.AddSingleton<IUnits, UnitsBL>()
                .AddSingleton<IOpeningLocation, OpeningLocationBL>()
                .AddSingleton<IUnitConfiguration, UnitConfigurationBL>()
                .AddSingleton<IAuthDL, AuthStubDL>()
                .AddSingleton<IAutoSaveConfigurationDL, AutoSaveConfigurationStubDL>()
                .AddSingleton<IBuildingConfigurationDL, BuildingConfigurationStubDL>()
                .AddSingleton<IConfiguratorService, ConfigurationServicesStubDL>()
                .AddSingleton<IGroupConfigurationDL, GroupConfigurationStubDL>()
                .AddSingleton<IGroupLayoutDL, GroupLayoutStubDL>()
                .AddSingleton<IProjectsDL, ProjectStubDL>()
                .AddSingleton<IProductDL, ProductStubDL>()
                .AddSingleton<IProductSelectionDL, ProductSelectionStubDL>()
                .AddSingleton<IUnitsDL, UnitsStubDL>()
                .AddSingleton<IAutoSaveConfiguration, AutoSaveConfigurationBL>()
                .AddSingleton<IOpeningLocationDL, OpeningLocationStubDL>()
                .AddSingleton<IUnitConfigurationDL, UnitConfigurationStubDL>()
                .AddSingleton<IConfiguration>(_configurationRoot)
                .AddSingleton<IHostingEnvironment, TKE.SC.BFF.Tests.Common.MockHosting>()
                .AddSingleton(serializer)
                .AddSingleton<ICacheManager, CPQCacheMangerStubServicesTest>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IBuildingEquipment, BuildingEquipmentBL>()
                .AddSingleton<IBuildingEquipmentDL, BuildingEquipmentStubDL>()
                .AddSingleton<IFieldDrawingAutomationDL, FieldDrawingAutomationStubDL>()
                .AddSingleton<ILogHistory, LogHistoryBL>()
                .AddSingleton<IFieldDrawingAutomation, FieldDrawingAutomationBL>()
                .AddSingleton<IOzBL, OzBL>()
                .AddSingleton<IOzDL, OzDLStub>()
                .AddSingleton<IReleaseInfo, ReleaseInfoBL>()
                .AddSingleton<IReleaseInfoDL, ReleaseInfoStubDL>()
                .AddSingleton<IDocument, DocumentBL>()
                .AddSingleton<IDocumentDL, IDocumentStubDL>()
                .AddSingleton<IVaultDL, IVaultStubDL>()
                .AddSingleton<IObom, ObomStubDL>()
                .AddSingleton<IDesignAutomationDL, DaStubDL>();

            services.BuildServiceProvider();
            services.AddDistributedMemoryCache();
            return services;

        }
    }
}
