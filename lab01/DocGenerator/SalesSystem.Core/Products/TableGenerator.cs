namespace SalesSystem.Core.Products;
using SalesSystem.Core.Models;
using System.Collections.Generic;

public class TableGenerator
{
    public (string TableText, decimal TotalSum) GenerateTable(List<Product> items, string rowTemplate)
    {
        string resultTable = "";
        decimal total = 0;
        int index = 1;

        foreach (var item in items)
        {
            decimal sum = item.Price * item.Count;
            total += sum;

            resultTable += rowTemplate
                .Replace("{{Index}}", index.ToString())
                .Replace("{{ItemName}}", item.Name)
                .Replace("{{ItemCount}}", item.Count.ToString())
                .Replace("{{ItemPrice}}", item.Price.ToString("C"))
                .Replace("{{ItemSum}}", sum.ToString("C"));
            index++;
        }

        return (resultTable, total);
    }
}