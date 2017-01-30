using WebBasedInstaller.Models;
using Microsoft.OData.Edm;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace WebBasedInstaller
{
    class WebApiConfig
    {
        #region Register
        public static void Register(HttpConfiguration config)
        {
            // OData routes
            // These must be configured before the WebAPI routes 
            config.MapHttpAttributeRoutes();
            config.MapODataServiceRoute(
               routeName: "ODataRoute",
               routePrefix: "odata",
               model: GenerateEntityDataModel());

            // This is required to make OData paging and filtering work
            config.Select().Expand().Filter().OrderBy().MaxTop(null).Count();

            // Web API routes 
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
        #endregion

        #region GenerateEntityDataModel
        private static IEdmModel GenerateEntityDataModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            // CurrentUser function returns User
            var CurrentUserFunction = builder.Function("CurrentUser");
            CurrentUserFunction.Returns<User>();

            // CurrentVersion function returns DTOVersion
            var CurrentVersionFunction = builder.Function("CurrentVersion");
            CurrentVersionFunction.Returns<DTOVersion>();

            // CurrentUser function returns DTOStatus
            var UpdateDatabaseFunction = builder.Function("UpdateDatabase");
            UpdateDatabaseFunction.Returns<DTOStatus>();

            // Register ODataDTOConnectionSetting
            builder.EntitySet<DTOConnectionSetting>("ODataConnectionSetting");

            // Register ODataProducts
            builder.EntitySet<DTOProduct>("ODataProducts");

            return builder.GetEdmModel();
        }
        #endregion
    }
}