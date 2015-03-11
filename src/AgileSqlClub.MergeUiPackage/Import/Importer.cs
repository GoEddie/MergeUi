using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUI.Import;
using AgileSqlClub.MergeUI.Merge;
using AgileSqlClub.MergeUI.Metadata;

namespace AgileSqlClub.MergeUI
{
    public class Importer
    {
        public void GetData(ITable table)
        {
            var wizrd = new ImportData(table);
            wizrd.ShowDialog();
            
        }
    }
}
