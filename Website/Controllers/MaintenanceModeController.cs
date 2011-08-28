using System.Web.Mvc;
using System;
using System.IO;
using System.Web;
using System.Text;

namespace MaintMan
{
    [HandleError]
    public class MaintenanceModeController : Controller
    {
        public const string Name = "MaintenanceMode";

        public const string AppOfflineTemplate =
@"<!DOCTYPE html> 
<html> 
<head> 
  <title>Application Offline for Maintenance</title> 
  <style type=""text/css""> 
    body {{
      background-color: white;
      color: #333333;
      font-family: Arial, sans-serif;
      margin: 0;
      padding: 36px;
      line-height: 18px;
      font-size: 14px;
    }}
    .section {{
      margin-bottom: 36px;
      color: #7A987A;
    }}
    .section h1 {{
      font-size: 26px;
      background-color: #243239;
      padding: 18px 22px 15px 22px;
      margin: 0;
      overflow: hidden;
    }}
    .article {{
      border: 4px solid #243239 ;
      color: black;
      padding: 24px 18px 18px 18px;
      font-size: 14px;
    }}
  </style> 
</head> 
<body> 
    <div class=""section""> 
      <h1>Application Offline for Maintenance</h1> 
      <div class=""article""> 
        <p>{0}</p> 
      </div> 
    </div> 
</body> 
</html> ";
        public const string DefaultMessage = "This application is currently offline for maintenance.  Please visit again later.";

        readonly IConfiguration configuration;
        readonly IBuildExecutor buildExecutor;
        readonly IMaintenanceTarballCreator maintenanceTarballCreator;
        
        public MaintenanceModeController(
            IConfiguration configuration, 
            IBuildExecutor httpRequestExecutor,
            IMaintenanceTarballCreator maintenanceTarballCreator)
        {
            this.configuration = configuration;
            this.buildExecutor = httpRequestExecutor;
            this.maintenanceTarballCreator = maintenanceTarballCreator;
        }

        [ActionName(ActionName.FindCreateBuildUrl)]
        public ActionResult ShowFindCreateBuildUrlPage()
        {
            return View();
        }
        
        [ActionName(ActionName.TurnMaintenanceModeOff)]
        public ActionResult ShowTurnMaintenanceModeOffPage()
        {
            return View();
        }
        
        [ActionName(ActionName.TurnMaintenanceModeOn), HttpGet]
        public ActionResult ShowTurnMaintenanceModeOnForm()
        {   
            return View();
        }

        [ActionName(ActionName.TurnMaintenanceModeOn), HttpPost]
        public ActionResult TurnMantienanceModeOn(SetMaintenanceModeRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            // TODO: consider moving this validation to an attribute?
            Uri buildUri;
            if (!Uri.TryCreate(request.AppBuildUrl, UriKind.Absolute, out buildUri))
            {
                ModelState.AddModelError("AppBuildUrl", Message.InvalidBuildUrl);
                return View();
            }

            try
            {
                buildExecutor.Execute(buildUri, request.Message);
            }
            catch (BadUrlException)
            {
                ModelState.AddModelError("AppBuildUrl", Message.InvalidBuildUrl);
                return View();
            }

            TempData["success"] = Message.SetMaintenanceModeSuccess;

            return RedirectToRoute(RouteName.Home);
        }

        [ActionName(ActionName.SendPayload), HttpGet]
        public ActionResult SendPayload(string message)
        {
            var decodedMessage = DefaultMessage;
            if (message != null)
            {
                var decodedMessageBytes = HttpServerUtility.UrlTokenDecode(message);
                decodedMessage = Encoding.UTF8.GetString(decodedMessageBytes);
            }
            
            // TODO: consider caching
            var bytes = maintenanceTarballCreator.Create(string.Format(AppOfflineTemplate, decodedMessage));

            return File(
                bytes,
                "application/json",
                "payload.tar.gz");
        }
    }
}