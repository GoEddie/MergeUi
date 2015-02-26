using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.Import;
using AgileSqlClub.MergeUi.Merge;
using AgileSqlClub.MergeUi.Metadata;

namespace AgileSqlClub.MergeUi
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
