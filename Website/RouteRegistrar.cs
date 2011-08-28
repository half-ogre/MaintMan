using System.Web.Mvc;
using System.Web.Routing;
using AnglicanGeek.Mvc;

namespace MaintMan
{
    public class RouteRegistrar : IRouteRegistrar
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                RouteName.Home,
                "",
                new { controller = MaintenanceModeController.Name, action = ActionName.TurnMaintenanceModeOn });

            routes.MapRoute(
                RouteName.FindCreateBuildUrl,
                ActionName.FindCreateBuildUrl,
                new { controller = MaintenanceModeController.Name, action = ActionName.FindCreateBuildUrl });
            
            routes.MapRoute(
                RouteName.TurnMaintenanceModeOn,
                ActionName.TurnMaintenanceModeOn,
                new { controller = MaintenanceModeController.Name, action = ActionName.TurnMaintenanceModeOn });

            routes.MapRoute(
                RouteName.TurnMaintenanceModeOff,
                ActionName.TurnMaintenanceModeOff,
                new { controller = MaintenanceModeController.Name, action = ActionName.TurnMaintenanceModeOff });

            routes.MapRoute(
                RouteName.Payload,
                ActionName.SendPayload,
                new { controller = MaintenanceModeController.Name, action = ActionName.SendPayload });
        }
    }
}