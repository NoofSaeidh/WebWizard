using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun
{
    [AutoMapFrom(typeof(LongRunResult), MemberList = AutoMapper.MemberList.Source)]
    public class LongRunResultDto
    {
        public string Message { get; set; }
        public string ResultUrl { get; set; }
        public string LongRunId { get; set; }
        public string JobId { get; set; }
    }
}
