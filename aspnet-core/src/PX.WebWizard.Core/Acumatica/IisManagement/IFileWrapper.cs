using System.Collections;
using System.Collections.Generic;

namespace PX.WebWizard.Acumatica.IisManagement
{
    public interface IFileWrapper
    {
        string GetParentPath(string path);
        string GetVersion(string filepath);
        IEnumerable<string> GetChilds(string dirpath);
        bool IsExists(string path);
        bool IsFolder(string path);
        string GetFileName(string path);
    }
}
