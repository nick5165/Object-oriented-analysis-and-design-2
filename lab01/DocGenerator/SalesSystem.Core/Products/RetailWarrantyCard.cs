namespace SalesSystem.Core.Products;
using SalesSystem.Core.Models;
using System.Text.Json;
using System.IO;
using System;

public class RetailWarrantyCard: IWarranty
{
    public Owner Owner { get; set; }

    public RetailWarrantyCard(Owner owner)
    {
        Owner = owner;
    }

    public string GetWarrantyTerms()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "RetailWarrantyCard.json");
        string json = File.ReadAllText(filePath);

        var template = JsonSerializer.Deserialize<WarrantyTemplateData>(json);
        if (template == null) return "Ошибка загрузки шаблона";

        string result = template.Header + "\n\n" + template.Body;
        
        result = result
        .Replace("{{DocNumber}}", DateTime.Now.Ticks.ToString().Substring(10))
        .Replace("{{OwnerName}}", Owner.Name)
        .Replace("{{CurrentDate}}", DateTime.Now.ToString("F"));
        
        return result;
    }

    public string GetValidationData()
    {
        string text = $"Документ выдан магазином {Owner.Name}. Подпись продавца: _________ [М.П.]";
        return text;
    }
    
    private class WarrantyTemplateData
    {
        public string Header { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}