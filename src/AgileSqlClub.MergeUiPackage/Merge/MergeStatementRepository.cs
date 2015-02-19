using System.Collections.Generic;

namespace AgileSqlClub.MergeUi.Merge
{
    public class MergeStatementRepository
    {
        private readonly MergeStatementFromMetadataBuilder _builder;

        private readonly Dictionary<string, MergeStatementDescriptor> _descriptors =
            new Dictionary<string, MergeStatementDescriptor>();

        public MergeStatementRepository(MergeStatementFromMetadataBuilder builder)
        {
            _builder = builder;
        }

        public MergeStatementDescriptor GetMergeStatement(string projectName, string schemaName, string tableName)
        {
            var key = BuildKey(projectName, schemaName, tableName);
            if (_descriptors.ContainsKey(key))
                return _descriptors[key];

            return _descriptors[key] = _builder.BuildFromMetaData(projectName, schemaName, tableName);
        }

        private string BuildKey(string projectName, string schemaName, string tableName)
        {
            return string.Format("{0}.{1}.{2}", projectName, schemaName, tableName);
        }
    }
}