using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Egee.Proxy.Sappel
{
    public sealed class SappelWrapper
    {
        public string Test()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            List<Assembly> assemblies = new List<Assembly>();
            foreach (StorageFile file in folder.GetFilesAsync().GetResults())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    var name = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
                    Assembly assembly = Assembly.Load(new AssemblyName() { Name = name });
                    assemblies.Add(assembly);
                }
            }

            return string.Join(", ", assemblies.Select(x => x.FullName));
        }
    }
}
