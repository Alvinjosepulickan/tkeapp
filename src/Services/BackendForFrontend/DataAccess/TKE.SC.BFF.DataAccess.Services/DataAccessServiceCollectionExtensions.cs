using Microsoft.Extensions.DependencyInjection;
using TKE.SC.BFF.DataAccess.Interfaces;

namespace TKE.SC.BFF.DataAccess.Services
{
    public static class DataAccessServiceCollectionExtensions
    {
        public static void AddDataAccessServices(this IServiceCollection service)
        {
            service.AddSingleton<IProductDL, ProductDL>()
                   .AddSingleton<IProjectsDL, ProjectsDL>()
                   .AddSingleton<IConfiguratorService, ConfiguratorServiceDL>()
                   .AddSingleton<IBuildingConfigurationDL, BuildingConfigurationDL>()
                   .AddSingleton<IGroupConfigurationDL, GroupConfigurationDL>()
                   .AddSingleton<IUnitConfigurationDL, UnitConfigurationDL>()
                   .AddSingleton<IAuthDL, AuthDL>()
                   .AddSingleton<IOpeningLocationDL, OpeningLocationDL>()
                   .AddSingleton<IAutoSaveConfigurationDL, AutoSaveConfigurationDL>()
                   .AddSingleton<IGroupLayoutDL, GroupLayoutDL>()
                   .AddSingleton<IUnitConfigurationDL, UnitConfigurationDL>()
                   .AddSingleton<IProductSelectionDL, ProductSelectionDL>()
                   .AddSingleton<IBuildingEquipmentDL, BuildingEquipmentDL>()
                   .AddSingleton<IFieldDrawingAutomationDL, FieldDrawingAutomationDL>()
                   .AddSingleton<IOzDL, OzDL>()
                   .AddSingleton<IReleaseInfoDL, ReleaseInfoDL>()
                   .AddSingleton<IDocumentDL, DocumentDL>()
                   .AddSingleton<IVaultDL, VaultDL>()
                   .AddSingleton<IObomDL,ObomDL>()
                   .AddSingleton<IDesignAutomationDL,DesignAutomationDL>();
        }
    }
}
