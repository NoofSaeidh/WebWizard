using System;
using System.Collections.Generic;
using System.Text;

namespace AcWebTool.Core.AcIIS.IISManagement
{
    public interface IIISManager
    {
        IISSite GetIISSite(string siteName);
    }
}
