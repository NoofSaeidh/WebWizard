using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun.Dto
{
    [AutoMapFrom(typeof(LongRunInfo))]
    public class LongRunInfoDto : EntityDto<string>
    {
        public LongRunStatus LongRunStatus { get; set; }
        public string JobId { get; set; }
        public string Args { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Summary { get; set; }
        public string Error { get; set; }
        public bool Abortable { get; set; }
    }
}
