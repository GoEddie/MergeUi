﻿namespace AgileSqlClub.MergeUi.DacServices
{
    public class DacParserBuilder
    {
        public virtual DacParser Build(string path)
        {
            return new DacParser(path);
        }
    }
}