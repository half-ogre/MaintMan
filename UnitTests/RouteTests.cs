using System.Web;
using System.Web.Routing;
using Moq;
using Xunit;

namespace MaintMan
{
    public class RouteTests
    {
        [Fact]
        public void The_Home_route_will_route_the_maintenance_mode_controller_and_set_maintenance_mode_action()
        {
            var routes = GetRoutes();
            var httpContext = CreateHttpContext("~/");

            var routeData = routes.GetRouteData(httpContext);

            Assert.Equal(MaintenanceModeController.Name, routeData.Values["controller"]);
            Assert.Equal(ActionName.TurnMaintenanceModeOn, routeData.Values["action"]);
        }

        [Fact]
        public void The_TurnMaintenanceModeOn_route_will_route_the_maintenance_mode_controller_and_turn_maintenance_mode_on_action()
        {
            var routes = GetRoutes();
            var httpContext = CreateHttpContext("~/turn-maintenance-mode-on");

            var routeData = routes.GetRouteData(httpContext);

            Assert.Equal(MaintenanceModeController.Name, routeData.Values["controller"]);
            Assert.Equal(ActionName.TurnMaintenanceModeOn, routeData.Values["action"]);
        }

        [Fact]
        public void The_SendPayload_route_will_route_the_maintenance_mode_controller_and_send_payload_action()
        {
            var routes = GetRoutes();
            var httpContext = CreateHttpContext("~/payload.tar.gz");

            var routeData = routes.GetRouteData(httpContext);

            Assert.Equal(MaintenanceModeController.Name, routeData.Values["controller"]);
            Assert.Equal(ActionName.SendPayload, routeData.Values["action"]);
        }

        [Fact]
        public void The_TurnMaintenanceModeOff_route_will_route_the_maintenance_mode_controller_and_turn_maintenance_mode_off_action()
        {
            var routes = GetRoutes();
            var httpContext = CreateHttpContext("~/turn-maintenance-mode-off");

            var routeData = routes.GetRouteData(httpContext);

            Assert.Equal(MaintenanceModeController.Name, routeData.Values["controller"]);
            Assert.Equal(ActionName.TurnMaintenanceModeOff, routeData.Values["action"]);
        }

        [Fact]
        public void The_FindCreateBuildUrl_route_will_route_the_maintenance_mode_controller_and_find_create_build_url_action()
        {
            var routes = GetRoutes();
            var httpContext = CreateHttpContext("~/find-create-build-url");

            var routeData = routes.GetRouteData(httpContext);

            Assert.Equal(MaintenanceModeController.Name, routeData.Values["controller"]);
            Assert.Equal(ActionName.FindCreateBuildUrl, routeData.Values["action"]);
        }

        static HttpContextBase CreateHttpContext(string appRelativeCurrentExecutionFilePath)
        {
            var moqHttpContext = new Mock<HttpContextBase>();
            var moqHttpRequest = new Mock<HttpRequestBase>();

            moqHttpRequest.Setup(x => x.AppRelativeCurrentExecutionFilePath)
                .Returns(appRelativeCurrentExecutionFilePath);
            moqHttpRequest.Setup(x => x.ApplicationPath)
                .Returns("/");
            moqHttpContext.Setup(x => x.Request)
                .Returns(moqHttpRequest.Object);

            return moqHttpContext.Object;
        }

        static RouteCollection GetRoutes()
        {
            var routes = new RouteCollection();

            new RouteRegistrar().RegisterRoutes(routes);

            return routes;
        }
    }
}
