using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgileSqlClub.MergeUi.Extensions;
using AgileSqlClub.MergeUi.Merge;
using AgileSqlClub.MergeUi.Metadata;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Table=AgileSqlClub.MergeUi.Metadata.Table;


namespace AgileSqlClub.MergeUi.DacServices
{
    
    public class DacServices
    {
        private readonly string _dacPath;

        public DacServices(string dacPath)
        {
            _dacPath = dacPath;

            Enumerate();
        }

        private string _preDeployScript;
        private string _postDeployScript;
        private readonly List<ITable> _definitions = new List<ITable>();


        private void Enumerate()
        {
            using (var package = DacPackage.Load(_dacPath, DacSchemaModelStorageType.File))
            {
                _postDeployScript = new StreamReader(package.PostDeploymentScript).ReadToEnd();
                _preDeployScript = new StreamReader(package.PreDeploymentScript).ReadToEnd();
            }

            using (var model = new TSqlModel(_dacPath, DacSchemaModelStorageType.Memory))
            {
                foreach (var table in model.GetObjects(DacQueryScopes.All, ModelSchema.Table))
                {
                    _definitions.Add(
                        new Table()
                            {
                                Columns = GetColumnDefinitions(table),
                                KeyColumns = GetKeyColumns(table),
                                Name = table.Name.GetName()
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
           
            foreach (var column in table.GetReferencedRelationshipInstances(Microsoft.SqlServer.Dac.Model.Table.Columns))
           {
               columns.Add(CreateColumnDefinition(column));
           }

            return columns;
        }

        private ColumnDescriptor CreateColumnDefinition(ModelRelationshipInstance column)
        {
            var definition = new ColumnDescriptor();
            definition.Name = new Identifier() {Value = column.ObjectName.GetName()};
            var type = column.Object.GetReferenced(Column.DataType).FirstOrDefault();

            definition.LiteralType = LiteralConverter.GetLiteralType(type.Name);
            return definition;
        }

    }

}
