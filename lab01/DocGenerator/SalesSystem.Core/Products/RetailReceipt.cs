namespace SalesSystem.Core.Products;
using System.Collections.Generic;
using SalesSystem.Core.Models;
using System;

public class RetailReceipt: IProofOfPurchase
{
    public List<Product> Items { get; set; }
    public RetailReceipt(List<Product> items)
    {
        Items = items;
    }
    public string GetDocumentData()
    {
        string fullText = "Мой магазин\n" + "ТОВАРНЫЙ ЧЕК\n";
        fullText += $"{DateTime.Now}\n";
        decimal totalSum = 0;
        foreach (Product item in Items)
        {
            decimal positionSum = item.Price * item.Count;
            fullText += $"{item.Name} --- {item.Count} шт. x {item.Price} = {positionSum}\n";
            totalSum += positionSum;
        }
        fullText += $"----------------\nИТОГО: {totalSum}";
        return fullText;
    }
}