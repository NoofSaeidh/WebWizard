using System;
using System.Collections.Generic;
using System.Text;

namespace PX.WebWizard.Acumatica.IisManagement
{
    public interface IIisManager
    {
        IisSite GetIisSite(string siteName);
    }
}
