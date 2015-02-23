using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Metadata;
using AgileSqlClub.MergeUi.VSServices;
using Moq;
using NUnit.Framework;

namespace AgileSqlClub.MergeUi.UnitTests
{
    [TestFixture]
    class SolutionTests
    {
        [Test]
        public void Finds_All_Projects()
        {
            var projectList = new List<ProjectDescriptor>();
            var project = new ProjectDescriptor();
            const string projectName = "abcdef";
            project.Name = projectName;
            const string path = "c:\\dac.dacpac";
            project.DacPath = path;

            projectList.Add(project);

            var enumerator = new Mock<ProjectEnumerator>();
            enumerator.Setup(p => p.EnumerateProjects()).Returns(projectList);

            var tables = new List<ITable>();
            tables.Add(new Table());

            var parser = new Mock<DacParser>();
            parser.Setup(p => p.PreDeployScript).Returns(() => null);
            parser.Setup(p => p.PostDeployScript).Returns(() => null);
            parser.Setup(p => p.GetTableDefinitions()).Returns(tables);

            var parserBuilder = new Mock<DacParserBuilder>();
            parserBuilder.Setup(p => p.Build(path)).Returns(parser.Object);


            var solution = new Solution(enumerator.Object, parserBuilder.Object);
            var actualProject = solution.GetProject(projectName);
            Assert.AreEqual(projectName, actualProject.GetName());
            Assert.AreEqual(1, solution.GetProjects().Count);
        }

    }
}
