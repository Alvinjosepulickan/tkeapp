using Microsoft.Extensions.DependencyInjection;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.PIPO;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public static class BusinessProcessServiceCollectionExtensions
    {
        public static void AddBusinessServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IProduct, ProductBL>()
                             .AddSingleton<IProject, ProjectsBL>()
                             .AddSingleton<IGenerateToken, GenerateTokenBL>()
                             .AddSingleton<IConfigure, ConfigureBL>()
                             .AddSingleton<IConfigureServices, ConfigureServiceBL>()
                             .AddSingleton<IBuildingConfiguration, BuildingConfigurationBL>()
                             .AddSingleton<IGroupConfiguration, GroupConfigurationBL>()
                             .AddSingleton<IUnitConfiguration, UnitConfigurationBL>()
                             .AddSingleton<IAuth, AuthBL>()
                             .AddSingleton<IOpeningLocation, OpeningLocationBL>()
                             .AddSingleton<IAutoSaveConfiguration, AutoSaveConfigurationBL>()
                             .AddSingleton<IGroupLayout, GroupLayoutBL>()
                             .AddSingleton<IProductSelection, ProductSelectionBL>()
                             .AddSingleton<IBuildingEquipment, BuildingEquipmentBL>()
                             .AddSingleton<IFieldDrawingAutomation, FieldDrawingAutomationBL>()
                             .AddSingleton<ILogHistory, LogHistoryBL>()
                             .AddSingleton<IOzBL, OzBL>()
                             .AddSingleton<IReleaseInfo, ReleaseInfoBL>()
                             .AddSingleton<IDocument, DocumentBL>()
                             .AddSingleton<IObom, ObomBL>()
                             .AddSingleton<IDesignAutomation, DesignAutomationBL>()
                             .AddSingleton<IPipoConnector, Connector>();
        }
    }
}
