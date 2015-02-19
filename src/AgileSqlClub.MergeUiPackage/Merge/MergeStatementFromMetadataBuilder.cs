using System;
using AgileSqlClub.MergeUi.Metadata;

namespace AgileSqlClub.MergeUi.Merge
{
    public class MergeStatementFromMetadataBuilder
    {
        private readonly ISolution _solution;
        
        internal MergeStatementFromMetadataBuilder()
        {
            
        }

        public MergeStatementFromMetadataBuilder(ISolution solution)
        {
            _solution = solution;
        }
        
        public virtual MergeStatementDescriptor BuildFromMetaData(string projectName, string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

    }
}