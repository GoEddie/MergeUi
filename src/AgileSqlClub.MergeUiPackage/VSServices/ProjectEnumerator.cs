using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.Metadata;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace AgileSqlClub.MergeUi.VSServices
{
    public class ProjectEnumerator
    {
        private const string SsdtProject = "{00d1a9c2-b5f0-4af3-8072-f6c62b433612}";
        private const string DacpacExtension = ".dacpac";

        public virtual List<ProjectDescriptor> EnumerateProjects()
        {
            var descriptors = new List<ProjectDescriptor>();

            var dte = MergeUiPackage.GetGlobalService(typeof (SDTE)) as DTE;

            var projects = dte.ActiveSolutionProjects as System.Array;
            for (int i = 0; i < projects.Length; i++)
            {
                var project = projects.GetValue(i) as EnvDTE.Project;
                if (project.Kind != SsdtProject)
                    continue;

                var dacpac = FindDacpacPath(project);

                descriptors.Add(new ProjectDescriptor(){Name = project.UniqueName, DacPath = dacpac});
            }

            return descriptors;
        }

        private string FindDacpacPath(Project project)
        {
            
            var outputFolders = new HashSet<string>();
            var builtGroup = project.ConfigurationManager.ActiveConfiguration.OutputGroups.OfType<EnvDTE.OutputGroup>().First(x => x.CanonicalName == "Built");

            try
            {
                if (builtGroup.FileURLs == null)
                    return null;
            }
            catch (Exception e)
            {
                return null;
            }

            foreach (var strUri in ((object[])builtGroup.FileURLs).OfType<string>())
            {
                var uri = new Uri(strUri, UriKind.Absolute);
                var filePath = uri.LocalPath;

                if (filePath.EndsWith(DacpacExtension, StringComparison.OrdinalIgnoreCase))
                    return filePath;
            }

            return null;
        }
    }
}
