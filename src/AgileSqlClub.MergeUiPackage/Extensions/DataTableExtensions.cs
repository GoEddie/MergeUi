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

        public static void SetClean(this DataTable source)
        {
            source.ExtendedProperties[DataTablePropertyNames.DataChanged] = false;
        }

        public static void SetDoNotSave(this DataTable source)
        {
            source.ExtendedProperties[DataTablePropertyNames.DoNotSave] = true;
        }

        public static void SetCanSave(this DataTable source)
        {
            source.ExtendedProperties[DataTablePropertyNames.DoNotSave] = false;
        }

        public static bool CanSave(this DataTable source)
        {
            if (source.ExtendedProperties.ContainsKey(DataTablePropertyNames.DoNotSave))
            {
                return (bool)source.ExtendedProperties[DataTablePropertyNames.DoNotSave];
            }

            return false;
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
