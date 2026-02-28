namespace SalesSystem.Core.Products;
using SalesSystem.Core.Models;
using System.Collections.Generic;
using System;
using System.Text.Json;
using System.IO;

public class RetailReceipt: IProofOfPurchase
{
    public Owner Owner;
    public List<Product> Items { get; set; }


    public RetailReceipt(List<Product> items, Owner owner)
    {
        Items = items;
        Owner = owner;
    }

    public string GetDocumentData()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "RetailReceipt.json");
        string json = File.ReadAllText(filePath);

        var template = JsonSerializer.Deserialize<ReceiptTemplateData>(json);
        if (template == null) return "Ошибка загрузки шаблона";

        string result = template.Header + "\n\n" + template.Body;
        result = result.Replace("{{OwnerName}}", Owner.Name);
        result = result.Replace("{{CurrentDate}}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
        
        var tableGenerator = new TableGenerator();
        var (itemsTable, total) = tableGenerator.GenerateTable(Items, template.ItemRow);

        result = result.Replace("{{ItemsTable}}", itemsTable);
        result = result.Replace("{{TotalSum}}", total.ToString("N2"));

        return result;
    }
    
    private class ReceiptTemplateData
    {
        public string Header { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string ItemRow { get; set; } = string.Empty;
    }
}