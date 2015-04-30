using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Metadata;
using AgileSqlClub.MergeUi.UI;
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


            var solution = new SolutionParser(enumerator.Object, parserBuilder.Object, new DummyStatus());
            var actualProject = solution.GetProject(projectName);
            Assert.AreEqual(projectName, actualProject.GetName());
            Assert.AreEqual(1, solution.GetProjects().Count);
        }

        [Test]
        public void Finds_Can_Add_Tables_After_Initial_Load()
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
            tables.Add(new Table(){Name = "oooh", SchemaName = "dbo"});

            var parser = new Mock<DacParser>();
            parser.Setup(p => p.PreDeployScript).Returns(() => null);
            parser.Setup(p => p.PostDeployScript).Returns(() => null);
            parser.Setup(p => p.GetTableDefinitions()).Returns(tables);

            var parserBuilder = new Mock<DacParserBuilder>();
            parserBuilder.Setup(p => p.Build(path)).Returns(parser.Object);
            
            var solution = new SolutionParser(enumerator.Object, parserBuilder.Object, new DummyStatus());
            var schema = solution.GetProject("abcdef").GetSchema("dbo");
            Assert.AreEqual(1, schema.GetTables().Count);
            
            solution.AddTable("abcdef", "dbo", "erm", new Table() { Name = "erm", SchemaName = "dbo" });
            Assert.AreEqual(2, schema.GetTables().Count);
            var table = solution.GetTable("abcdef", "dbo", "erm");
            Assert.IsNotNull(table);
            
            
            

        }
        
    }

    public class DummyStatus : IStatus
    {
        public void SetStatus(string message)
        {
            
        }
    }

    [TestFixture]
    public class ProjectTests
    {
        [Test]
        public void Finds_Schemas()
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
            tables.Add(new Table() { SchemaName = "one", Name = "table" });
            tables.Add(new Table() { SchemaName = "one", Name = "table2" });
            tables.Add(new Table() { SchemaName = "two", Name = "table" });

            var parser = new Mock<DacParser>();
            parser.Setup(p => p.PreDeployScript).Returns(() => null);
            parser.Setup(p => p.PostDeployScript).Returns(() => null);
            parser.Setup(p => p.GetTableDefinitions()).Returns(tables);

            var parserBuilder = new Mock<DacParserBuilder>();
            parserBuilder.Setup(p => p.Build(path)).Returns(parser.Object);

            var solution = new SolutionParser(enumerator.Object, parserBuilder.Object, new DummyStatus());
            var actualProject = solution.GetProject(projectName);
            Assert.AreEqual(2, actualProject.GetSchemas().Count);
        }

        [Test]
        public void GetTable_Returns_Correct_Table()
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
            tables.Add(new Table() { SchemaName = "one", Name = "table" });
            tables.Add(new Table() { SchemaName = "one", Name = "table2" });
            tables.Add(new Table() { SchemaName = "two", Name = "table" });

            var parser = new Mock<DacParser>();
            parser.Setup(p => p.PreDeployScript).Returns(() => null);
            parser.Setup(p => p.PostDeployScript).Returns(() => null);
            parser.Setup(p => p.GetTableDefinitions()).Returns(tables);

            var parserBuilder = new Mock<DacParserBuilder>();
            parserBuilder.Setup(p => p.Build(path)).Returns(parser.Object);

            var solution = new SolutionParser(enumerator.Object, parserBuilder.Object, new DummyStatus());

            var table = solution.GetTable("abcdef", "one", "table2");
            Assert.AreEqual("one", table.SchemaName);
            Assert.AreEqual("table2", table.Name);
        }

    }

}
