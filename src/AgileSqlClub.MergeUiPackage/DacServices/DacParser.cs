using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgileSqlClub.MergeUi.Extensions;
using AgileSqlClub.MergeUi.Merge;
using AgileSqlClub.MergeUi.Metadata;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Table = AgileSqlClub.MergeUi.Metadata.Table;

namespace AgileSqlClub.MergeUi.DacServices
{
    public class DacParserBuilder
    {
        public virtual DacParser Build(string path)
        {
            return new DacParser(path);
        }
    }

    public class DacParser
    {
        private readonly string _dacPath;
        private readonly List<ITable> _tableDefinitions = new List<ITable>();

        /// <summary>
        /// only for mocking, do not use
        /// </summary>
        public DacParser()
        {
            
        }

        public DacParser(string dacPath)
        {
            _dacPath = dacPath;

            Enumerate();
        }

        public virtual string PreDeployScript { get; private set; }
        public virtual string PostDeployScript { get; private set; }

        public virtual List<ITable> GetTableDefinitions()
        {
            return _tableDefinitions;
        } 
        

        private void Enumerate()
        {
            using (var package = DacPackage.Load(_dacPath, DacSchemaModelStorageType.File))
            {
                if (package.PostDeploymentScript != null)
                    PostDeployScript = new StreamReader(package.PostDeploymentScript).ReadToEnd();
                else
                    PostDeployScript = "";

                if (package.PreDeploymentScript != null)
                    PreDeployScript = new StreamReader(package.PreDeploymentScript).ReadToEnd();
                else
                    PreDeployScript = "";
            }

            using (var model = new TSqlModel(_dacPath, DacSchemaModelStorageType.Memory))
            {
                foreach (var table in model.GetObjects(DacQueryScopes.All, ModelSchema.Table))
                {
                    _tableDefinitions.Add(
                        new Table
                        {
                            Columns = GetColumnDefinitions(table),
                            KeyColumns = GetKeyColumns(table),
                            Name = table.Name.GetName(),
                            SchemaName = table.Name.GetSchema()
                        }
                        );
                }
            }
        }

        private List<string> GetKeyColumns(TSqlObject table)
        {
            var keys = new List<string>();

            var primaryKey = table.GetChildren().FirstOrDefault(p => p.ObjectType == ModelSchema.PrimaryKeyConstraint);

            if (null == primaryKey)
                return keys;

            foreach (var column in primaryKey.GetReferencedRelationshipInstances(PrimaryKeyConstraint.Columns))
            {
                keys.Add(column.ObjectName.GetName());
            }

            return keys;
        }

        private List<ColumnDescriptor> GetColumnDefinitions(TSqlObject table)
        {
            var columns = new List<ColumnDescriptor>();

            foreach (var column in table.GetReferencedRelationshipInstances(Microsoft.SqlServer.Dac.Model.Table.Columns)
                )
            {
                columns.Add(CreateColumnDefinition(column));
            }

            return columns;
        }

        private ColumnDescriptor CreateColumnDefinition(ModelRelationshipInstance column)
        {
            var definition = new ColumnDescriptor();
            definition.Name = new Identifier {Value = column.ObjectName.GetName()};
            var type = column.Object.GetReferenced(Column.DataType).FirstOrDefault();

            definition.LiteralType = LiteralConverter.GetLiteralType(type.Name);
            return definition;
        }
    }
}