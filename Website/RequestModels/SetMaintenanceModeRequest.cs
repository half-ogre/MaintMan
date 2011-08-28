using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MaintMan
{
    public class SetMaintenanceModeRequest
    {
        [Required]
        [DisplayName("Create Build URL")]
        [DataType(DataType.Url)]
        public string AppBuildUrl { get; set; }

        [StringLength(2048)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Maintenance Mode Message")]
        public string Message { get; set; }
    }
}