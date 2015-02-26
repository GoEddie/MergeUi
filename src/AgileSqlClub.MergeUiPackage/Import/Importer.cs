using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.Import;

namespace AgileSqlClub.MergeUi
{
    public class Importer
    {
        public DataTable GetData()
        {
            var wizrd = new ImportData();
            wizrd.ShowDialog();
            return null;
        }
    }
}
