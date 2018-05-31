using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PX.WebWizard.Acumatica.IisManagement
{
    public class FileWrapper : IFileWrapper, ISingletonDependency
    {
        public string GetVersion(string filepath)
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(filepath).ProductVersion;
            }
            catch
            {
                return null;
            }
        }

        public string GetParentPath(string filepath)
        {
            return new DirectoryInfo(filepath).Parent.FullName;
        }

        public IEnumerable<string> GetChilds(string dirpath)
        {
            return new DirectoryInfo(dirpath).EnumerateFileSystemInfos().Select(x => x.FullName);
        }

        public bool IsExists(string path)
        {
            try
            {
                return new DirectoryInfo(path).Exists
                    || new FileInfo(path).Exists;
            }
            catch
            {
                return false;
            }
        }

        public bool IsFolder(string path)
        {
            try
            {
                return new DirectoryInfo(path).Exists;
            }
            catch
            {
                return false;
            }
        }

        public string GetFileName(string path)
        {
            return new DirectoryInfo(path).Name;
        }
    }
}
