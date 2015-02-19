using System.Collections.Generic;

namespace AgileSqlClub.MergeUi.Metadata
{
    public interface IProject
    {
        string GetScript(ScriptType type);
        void SetScript(ScriptType type, string script);

        ISchema GetSchema(string name);
        List<string> GetSchemas();
    }
}