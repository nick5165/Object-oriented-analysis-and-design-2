namespace SalesSystem.Core.Products;
using SalesSystem.Core.Models;
using System.Collections.Generic;
using System;
using System.Text.Json;
using System.IO;

public class WholesaleContract: IProofOfPurchase
{
    public List<Product> Items { get; set; }
    public WholesaleCustomer Customer { get; set; }
    public Owner Seller { get; set; }
    public Supplier SelectedSupplier { get; set; }
    public DeliveryInfo? Delivery { get; set; }

    public WholesaleContract(Owner owner, WholesaleCustomer customer, Supplier supplier, List<Product> items, DeliveryInfo? delivery)
    {
        Seller = owner;
        Customer = customer;
        SelectedSupplier = supplier;
        Items = items;
        Delivery = delivery;
    }
    
    public string GetDocumentData()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "WholesaleContract.json");
        string json = File.ReadAllText(filePath);

        var template = JsonSerializer.Deserialize<ContractTemplateData>(json);
        if (template == null) return "Ошибка загрузки шаблона";
        
        string result = template.Header + "\n\n" + template.Body;

        result = result
        .Replace("{{DocNumber}}", DateTime.Now.Ticks.ToString().Substring(10))
        .Replace("{{CurrentDate}}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
        .Replace("{{OwnerName}}", Seller.Name)
        .Replace("{{DirectorName}}", Seller.DirectorName)
        .Replace("{{AccountantName}}", Seller.AccountantName)
        .Replace("{{DirectorSignature}}", Seller.DirectorSignature)
        .Replace("{{StampPath}}", Seller.StampPath)
        .Replace("{{CustomerName}}", Customer.Name)
        .Replace("{{CustomerINN}}", Customer.INN)
        .Replace("{{CustomerSignature}}", Customer.Signature)
        .Replace("{{SupplierName}}", SelectedSupplier.Name)
        .Replace("{{SupplierINN}}", SelectedSupplier.INN);

        if (Delivery != null)
        {
            string deliveryInfo = template.DeliveryTemplate
                .Replace("{{CourierName}}", Delivery.CourierName)
                .Replace("{{VehicleNumber}}", Delivery.VehicleNumber)
                .Replace("{{DeliveryPrice}}", Delivery.DeliveryPrice.ToString());
            result = result.Replace("{{DeliverySection}}", deliveryInfo);
        }
        else
        {
            result = result.Replace("{{DeliverySection}}", template.SelfDeliveryTemplate);
        }

        var tableGenerator = new TableGenerator();
        var (itemsTable, total) = tableGenerator.GenerateTable(Items, template.ItemRowTemplate);
        
        result = result.Replace("{{ItemsTable}}", itemsTable);
        result = result.Replace("{{TotalSum}}", total.ToString("N2"));

        return result;
    }

    private class ContractTemplateData
    {
        public string Header { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string DeliveryTemplate { get; set; } = string.Empty;
        public string SelfDeliveryTemplate { get; set; } = string.Empty;
        public string ItemRowTemplate { get; set; } = string.Empty;
    }
}