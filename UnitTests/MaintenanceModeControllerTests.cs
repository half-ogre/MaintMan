using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Xunit;
using System.Text;

namespace MaintMan
{
    public class MaintenanceModeControllerTests
    {
        public class The_ShowSetMaintenanceModeForm_action
        {
            [Fact]
            public void will_show_the_form()
            {
                var controller = CreateController();

                var result = controller.ShowTurnMaintenanceModeOnForm() as ViewResult;

                Assert.NotNull(result);
                Assert.Empty(result.ViewName);
            }
        }

        public class The_SetMaintenanceMode_action
        {
            [Fact]
            public void will_execute_a_build_with_the_app_build_url()
            {
                var buildExecutor = new Mock<IBuildExecutor>();
                var controller = CreateController(buildExecutor: buildExecutor);
                var request = new SetMaintenanceModeRequest { AppBuildUrl = "http://theBuildUrl/path" };
                
                controller.TurnMantienanceModeOn(request);

                buildExecutor.Verify(x => x.Execute(
                    new Uri("http://theBuildUrl/path"), null));
            }

            [Fact]
            public void will_pass_the_maintenance_mode_message_the_build_executor_when_specified()
            {
                var buildExecutor = new Mock<IBuildExecutor>();
                var controller = CreateController(buildExecutor: buildExecutor);
                var request = new SetMaintenanceModeRequest { AppBuildUrl = "http://theBuildUrl/path" };
                var messageBytes = Encoding.UTF8.GetBytes("theMessage");
                request.Message = HttpServerUtility.UrlTokenEncode(messageBytes);

                controller.TurnMantienanceModeOn(request);

                buildExecutor.Verify(x => x.Execute(
                    new Uri("http://theBuildUrl/path"), request.Message));
            }

            [Fact]
            public void will_show_the_form_with_errors_when_the_model_state_is_invalid()
            {
                var controller = CreateController();
                controller.ModelState.AddModelError(string.Empty, "An fake error.");

                var result = controller.TurnMantienanceModeOn(null) as ViewResult;

                Assert.NotNull(result);
                Assert.Empty(result.ViewName);
            }

            [Fact]
            public void will_invalidate_model_state_and_show_the_form_with_errors_when_the_build_url_is_not_a_valid_url()
            {
                var buildExecutor = new Mock<IBuildExecutor>();
                var controller = CreateController(buildExecutor: buildExecutor);
                var request = new SetMaintenanceModeRequest { AppBuildUrl = "anInvalidUrl" };

                var result = controller.TurnMantienanceModeOn(request) as ViewResult;

                Assert.NotNull(result);
                Assert.Empty(result.ViewName);
                Assert.False(controller.ModelState.IsValid);
                Assert.Equal(
                    Message.InvalidBuildUrl,
                    controller.ModelState["AppBuildUrl"].Errors[0].ErrorMessage);
            }

            [Fact]
            public void will_flash_success_after_executing_the_build()
            {
                var buildExecutor = new Mock<IBuildExecutor>();
                var controller = CreateController(buildExecutor: buildExecutor);
                var request = new SetMaintenanceModeRequest { AppBuildUrl = "http://theBuildUrl/path" };

                controller.TurnMantienanceModeOn(request);

                Assert.Equal(Message.SetMaintenanceModeSuccess, controller.TempData["success"]);
            }

            [Fact]
            public void will_invalidate_model_state_and_show_the_form_with_errors_when_the_a_bad_url_exception_is_thrown()
            {
                var buildExecutor = new Mock<IBuildExecutor>();
                buildExecutor.Setup(x => x.Execute(It.IsAny<Uri>(), It.IsAny<string>()))
                    .Throws(new BadUrlException(""));
                var controller = CreateController(buildExecutor: buildExecutor);
                var request = new SetMaintenanceModeRequest { AppBuildUrl = "anInvalidUrl" };

                var result = controller.TurnMantienanceModeOn(request) as ViewResult;

                Assert.NotNull(result);
                Assert.Empty(result.ViewName);
                Assert.False(controller.ModelState.IsValid);
                Assert.Equal(
                    Message.InvalidBuildUrl,
                    controller.ModelState["AppBuildUrl"].Errors[0].ErrorMessage);
            }
        }

        public class The_SendPayload_action
        {
            [Fact]
            public void will_use_the_specified_message_to_create_the_payload_tarball() {
                var maintenanceTarballCreator = new Mock<IMaintenanceTarballCreator>();
                var controller = CreateController(maintenanceTarballCreator: maintenanceTarballCreator);
                var messageBytes = Encoding.UTF8.GetBytes("theMessage");
                var message = HttpServerUtility.UrlTokenEncode(messageBytes);

                controller.SendPayload(message);

                maintenanceTarballCreator.Verify(x => x.Create(string.Format(MaintenanceModeController.AppOfflineTemplate, "theMessage")));
            }

            [Fact]
            public void will_use_the_default_message_to_create_the_payload_tarball_when_no_message_is_specified() {
                var maintenanceTarballCreator = new Mock<IMaintenanceTarballCreator>();
                var controller = CreateController(maintenanceTarballCreator: maintenanceTarballCreator);

                controller.SendPayload(null);

                maintenanceTarballCreator.Verify(x => x.Create(string.Format(MaintenanceModeController.AppOfflineTemplate, MaintenanceModeController.DefaultMessage)));
            }

            [Fact]
            public void will_send_the_payload_tarball()
            {
                var fakeTarballBytes = new byte[1] { 1 };
                var maintenanceTarballCreator = new Mock<IMaintenanceTarballCreator>();
                maintenanceTarballCreator.Setup(x => x.Create(It.IsAny<string>()))
                    .Returns(fakeTarballBytes);
                var controller = CreateController(maintenanceTarballCreator: maintenanceTarballCreator);
             
                var result = controller.SendPayload(null) as FileContentResult;

                Assert.NotNull(result);
                Assert.Equal(fakeTarballBytes, result.FileContents);
                Assert.Equal("application/json", result.ContentType);
                Assert.Equal("payload.tar.gz", result.FileDownloadName);
            }
        }

        public static MaintenanceModeController CreateController(
            Mock<IConfiguration> configuration = null,
            Mock<IBuildExecutor> buildExecutor = null,
            Mock<IMaintenanceTarballCreator> maintenanceTarballCreator = null)
        {
            if (configuration == null)
            {
                configuration = new Mock<IConfiguration>();
                configuration.Setup( x => x.BaseUrl).Returns("theBaseUrl");
            }

            buildExecutor = buildExecutor ?? new Mock<IBuildExecutor>();
            maintenanceTarballCreator = maintenanceTarballCreator ?? new Mock<IMaintenanceTarballCreator>();
            
            return new MaintenanceModeController(
                configuration.Object,
                buildExecutor.Object,
                maintenanceTarballCreator.Object);
        }
    }
}
