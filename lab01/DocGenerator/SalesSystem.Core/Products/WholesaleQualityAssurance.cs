namespace SalesSystem.Core.Products;
using SalesSystem.Core.Models;
using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
public class WholesaleQualityAssurance: IWarranty
{
    public Owner Owner;
    public Supplier Supplier;
    public WholesaleCustomer Customer { get; set; }

    public WholesaleQualityAssurance(WholesaleCustomer customer, Owner owner, Supplier supplier)
    {
        Customer = customer;
        Owner = owner;
        Supplier = supplier;
    }

    public string GetWarrantyTerms()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "WholesaleWarranty.json");
        string json = File.ReadAllText(filePath);

        var template = JsonSerializer.Deserialize<WarrantyTemplateData>(json);
        if (template == null) return "Ошибка загрузки шаблона";
        
        string result = template.Header + "\n\n" + template.Body + "\n\n" + template.Footer;

        result = result
        .Replace("{{DocNumber}}", "W-CONTRACT-" + DateTime.Now.Year)
        .Replace("{{OwnerName}}", Owner.Name)
        .Replace("{{DirectorName}}", Owner.DirectorName)
        .Replace("{{AccountantName}}", Owner.AccountantName)
        .Replace("{{DirectorSignature}}", Owner.DirectorSignature)
        .Replace("{{StampPath}}", Owner.StampPath)
        .Replace("{{CustomerName}}", Customer.Name)
        .Replace("{{CustomerINN}}", Customer.INN)
        .Replace("{{CustomerSignature}}", Customer.Signature)
        .Replace("{{SupplierName}}", Supplier.Name)
        .Replace("{{SupplierINN}}", Supplier.INN)
        .Replace("{{CurrentDate}}", DateTime.Now.ToString("F"));

        return result;
    }

    public string GetValidationData()
    {
        string text = $"Документ верифицирован в системе ЭДО. Покупатель: {Customer.Name}. ЭЦП: {Customer.Signature}. Оттиск печати: {Customer.PrintPath}";
        return text;
    }

    private class WarrantyTemplateData
    {
        public string Header { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Footer { get; set; } = string.Empty;
    }
}