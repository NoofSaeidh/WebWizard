using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun.Dto
{
    [AutoMapFrom(typeof(LongRunAbortResult))]
    public class LongRunAbortResultDto
    {
        public LongRunAbortStatus AbortStatus { get; set; }
    }
}
