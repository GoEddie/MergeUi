using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace AgileSqlClub.MergeUI.VSServices
{
    public class ProjectEnumerator
    {
        private const string SsdtProject = "{00d1a9c2-b5f0-4af3-8072-f6c62b433612}";
        private const string DacpacExtension = ".dacpac";

        private List<ProjectDescriptor> GetSubProjects(Project project)
        {
            var descriptors = new List<ProjectDescriptor>();

            if (project.Kind == SsdtProject)
            {
                var dacpac = FindDacpacPath(project);
                var preDeployScript = FindPreDeployScriptPath(project);
                var postDeployScript = FindPostDeployScriptPath(project);
                descriptors.Add(new ProjectDescriptor
                {
                    Name = project.UniqueName,
                    DacPath = dacpac,
                    PreDeployScriptPath = preDeployScript,
                    PostDeployScriptPath = postDeployScript
                });
            }
            else
            {
                for (var i = 1; i <= project.ProjectItems.Count; i++)
                {
                    if (project.ProjectItems.Item(i).SubProject != null)
                        descriptors.AddRange(GetSubProjects(project.ProjectItems.Item(i).SubProject));
                }
            }

            return descriptors;
        }

        public virtual List<ProjectDescriptor> EnumerateProjects()
        {
            var descriptors = new List<ProjectDescriptor>();

            try
            {
                var dte = Package.GetGlobalService(typeof (SDTE)) as DTE;
                for (var i = 1; i <= dte.Solution.Projects.Count; i++)
                {
                    descriptors.AddRange(GetSubProjects(dte.Solution.Projects.Item(i)));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("MergeUI was unable to process the dacpacs, error: {0}", e.Message),
                    "MergeUI");
            }

            return descriptors;
        }

        private string FindPreDeployScriptPath(Project project)
        {
            return GetFilesWithBuildAction("PreDeploy", project).FirstOrDefault();
        }

        private string FindPostDeployScriptPath(Project project)
        {
            return GetFilesWithBuildAction("PostDeploy", project).FirstOrDefault();
        }

        private string FindDacpacPath(Project project)
        {
            var outputFolders = new HashSet<string>();
            var builtGroup =
                project.ConfigurationManager.ActiveConfiguration.OutputGroups.OfType<OutputGroup>()
                    .First(x => x.CanonicalName == "Built");

            try
            {
                if (builtGroup.FileURLs == null)
                    return null;
            }
            catch (Exception e)
            {
                return null;
            }

            foreach (var strUri in ((object[]) builtGroup.FileURLs).OfType<string>())
            {
                var uri = new Uri(strUri, UriKind.Absolute);
                var filePath = uri.LocalPath;

                if (filePath.EndsWith(DacpacExtension, StringComparison.OrdinalIgnoreCase))
                    return filePath;
            }

            return null;
        }

        public List<string> GetFilesWithBuildAction(string property, Project project)
        {
            if (project == null)
                return null;

            var items = GetChildObjectsWithBuildAction(project.ProjectItems, property);

            return items;
        }

        private List<string> GetChildObjectsWithBuildAction(ProjectItems items, string buildAction)
        {
            var foundItems = new List<string>();
            foreach (ProjectItem item in items)
            {
                if (item.ProjectItems != null)
                    foundItems.AddRange(GetChildObjectsWithBuildAction(item.ProjectItems, buildAction));


                if (item.Properties != null)
                {
                    var fullPath = String.Empty;
                    var isMatch = false;

                    foreach (Property property in item.Properties)
                    {
                        if (property.Name == "BuildAction" && property.Value.ToString() == buildAction)
                        {
                            isMatch = true;
                        }

                        if (property.Name == "FullPath")
                        {
                            fullPath = property.Value.ToString();
                        }
                    }

                    if (isMatch)
                        foundItems.Add(fullPath);
                }
            }

            return foundItems;
        }
    }
}