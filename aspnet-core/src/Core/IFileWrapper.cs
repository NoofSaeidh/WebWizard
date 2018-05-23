using System.Collections;
using System.Collections.Generic;

namespace AcWebTool.Core
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
