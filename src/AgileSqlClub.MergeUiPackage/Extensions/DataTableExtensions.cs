using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.Merge;

namespace AgileSqlClub.MergeUi.Extensions
{
    public static class DataTableExtensions
    {
        public static void EnableDirtyWatcher(this DataTable source)
        {
            source.RowChanged += (sender, args) =>
            {
                var dataTable = sender as DataTable;
                dataTable.SetDirty();
            };

            source.TableNewRow += (sender, args) =>
            {
                var dataTable = sender as DataTable;
                dataTable.SetDirty();
            };

            source.RowDeleted += (sender, args) =>
            {
                var dataTable = sender as DataTable;
                dataTable.SetDirty();
            };
        }

        public static void SetDirty(this DataTable source)
        {
            source.ExtendedProperties[DataTablePropertyNames.DataChanged] = true;
        }

        public static bool IsDirty(this DataTable source)
        {
            if (source.ExtendedProperties.ContainsKey(DataTablePropertyNames.DataChanged))
            {
                return (bool) source.ExtendedProperties[DataTablePropertyNames.DataChanged];
            }

            return false;
        }

    }
}
