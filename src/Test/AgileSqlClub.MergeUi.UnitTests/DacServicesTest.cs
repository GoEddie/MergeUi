using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;

namespace AgileSqlClub.MergeUi.UnitTests
{
    [TestFixture]
    class DacServicesTests
    {
        [Test]
        public void Gets_Column_Definition()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "dacServicesTest.dac");
            if(File.Exists(path))
                File.Delete(path);
            
            using (TSqlModel model = new TSqlModel(SqlServerVersion.Sql110, new TSqlModelOptions {}))
            {
                model.AddObjects( "CREATE TABLE t1 (c1 NVARCHAR(30) NOT NULL, id int primary key clustered)", new TSqlObjectOptions());
                
                DacPackageExtensions.BuildPackage(path, model,
                    new PackageMetadata() {Description = "Test Package", Name = "ssss", Version = "1"});
            }

            var dac = new DacServices.DacServices(path);
            Assert.AreEqual(1, dac.TableDefinitions.Count);

            var table = dac.TableDefinitions.FirstOrDefault();

            Assert.AreEqual("c1", table.Columns.FirstOrDefault().Name.Value);
            Assert.AreEqual(LiteralType.String, table.Columns.FirstOrDefault().LiteralType);

            
        }

        [Test]
        public void Gets_KeyColumn_Definition()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "dacServicesTest.dac");
            if (File.Exists(path))
                File.Delete(path);

            using (TSqlModel model = new TSqlModel(SqlServerVersion.Sql110, new TSqlModelOptions { }))
            {
                model.AddObjects("CREATE TABLE t1 (c1 NVARCHAR(30) NOT NULL, id int primary key clustered)", new TSqlObjectOptions());

                DacPackageExtensions.BuildPackage(path, model,
                    new PackageMetadata() { Description = "Test Package", Name = "ssss", Version = "1" });
            }

            var dac = new DacServices.DacServices(path);
            Assert.AreEqual(1, dac.TableDefinitions.Count);

            var table = dac.TableDefinitions.FirstOrDefault();

            Assert.AreEqual(1, table.KeyColumns.Count);
            Assert.AreEqual("id", table.KeyColumns.FirstOrDefault());
            
        }

        [Test]
        public void Sets_KeyColumns_To_Empty_List_When_No_Primary_Key()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "dacServicesTest.dac");
            if (File.Exists(path))
                File.Delete(path);

            using (TSqlModel model = new TSqlModel(SqlServerVersion.Sql110, new TSqlModelOptions { }))
            {
                model.AddObjects("CREATE TABLE t1 (c1 NVARCHAR(30) NOT NULL, id int)", new TSqlObjectOptions());

                DacPackageExtensions.BuildPackage(path, model,
                    new PackageMetadata() { Description = "Test Package", Name = "ssss", Version = "1" });
            }

            var dac = new DacServices.DacServices(path);
            Assert.AreEqual(1, dac.TableDefinitions.Count);

            var table = dac.TableDefinitions.FirstOrDefault();

            Assert.AreEqual(0, table.KeyColumns.Count);
            

        }

    }
}
