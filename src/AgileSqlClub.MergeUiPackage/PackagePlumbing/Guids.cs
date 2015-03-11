// Guids.cs
// MUST match guids.h

using System;

namespace AgileSqlClub.MergeUI
{
    static class GuidList
    {
        public const string guidMergeUIPkgString = "78b76b0b-9e06-4f24-b20b-8a56ee968feb";
        public const string MergeUIPackageCmdSetString = "a97f3a02-b603-4583-a658-32f998e5a267";
        public const string guidToolWindowPersistanceString = "d6f5949a-8f37-4a19-98da-c39812573933";

        public static readonly Guid MergeUIPackageCmdSet = new Guid(MergeUIPackageCmdSetString);
    };
}