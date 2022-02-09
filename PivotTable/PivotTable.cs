//Added by NS
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Reflection;

//Create table using matrix
public static class PivotExtensions
{
    public static DataTable ToPivotTable<T, TColumn, TRow, TData>(
        this IEnumerable<T> source,
        Func<T, TColumn> columnSelector,
        Expression<Func<T, TRow>> rowSelector,
        Func<IEnumerable<T>, TData> dataSelector)
    {
        DataTable table = new DataTable();
        var rowNames = GetMemberNames(rowSelector);
        rowNames.ToList().ForEach(x => table.Columns.Add(new DataColumn(x)));
        var columns = source.Select(columnSelector).Distinct();

        foreach (var column in columns)
            table.Columns.Add(new DataColumn(column.ToString()));

        var rows = source.GroupBy(rowSelector.Compile())
            .Select(rowGroup => new
            {
                Key = rowGroup.Key,
                Values = columns.GroupJoin(
                            rowGroup,
                            c => c,
                            r => columnSelector(r),
                            (c, columnGroup) => dataSelector(columnGroup))
            });

        foreach (var row in rows)
        {
            var dataRow = table.NewRow();
            var items = row.Values.Cast<object>().ToList();
            string[] keyRow = row.Key.ToString().Split(',');
            int index = 0;
            foreach (var key in keyRow)
            {
                string keyValue = key.Replace("}", "").Split('=')[1].Trim();
                items.Insert(index, keyValue);
                index++;
            }
            dataRow.ItemArray = items.ToArray();
            table.Rows.Add(dataRow);
        }
        
        return table;

    }
    public static IEnumerable<string> GetMemberNames<T1, T2>(Expression<Func<T1, T2>> expression)
    {
        var memberExpression = expression.Body as MemberExpression;
        if (memberExpression != null)
        {
            return new[] { memberExpression.Member.Name };
        }
        var memberInitExpression = expression.Body as MemberInitExpression;
        if (memberInitExpression != null)
        {
            return memberInitExpression.Bindings.Select(x => x.Member.Name);
        }
        var newExpression = expression.Body as NewExpression;
        if (newExpression != null)
        {
            return newExpression.Arguments.Select(x => (x as MemberExpression).Member.Name);
        }

        throw new ArgumentException("expression");
    }

}
