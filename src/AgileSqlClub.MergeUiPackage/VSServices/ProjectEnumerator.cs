using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

            try
            {
                var dte = MergeUiPackage.GetGlobalService(typeof (SDTE)) as DTE;

                if (dte == null || dte.ActiveSolutionProjects == null)
                    return descriptors;

                var projects = dte.ActiveSolutionProjects as System.Array;
                for (int i = 0; i < projects.Length; i++)
                {
                    var project = projects.GetValue(i) as EnvDTE.Project;
                    if (project.Kind != SsdtProject)
                        continue;

                    var dacpac = FindDacpacPath(project);
                    var preDeployScript = FindPreDeployScriptPath(project);
                    var postDeployScript = FindPostDeployScriptPath(project);
                    descriptors.Add(new ProjectDescriptor()
                    {
                        Name = project.UniqueName,
                        DacPath = dacpac,
                        PreDeployScriptPath = preDeployScript,
                        PostDeployScriptPath = postDeployScript
                    });
                }
            }catch(Exception e)
            {
                MessageBox.Show("MergeUi was unable to process the dacpacs, error: {0}", e.Message);
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
