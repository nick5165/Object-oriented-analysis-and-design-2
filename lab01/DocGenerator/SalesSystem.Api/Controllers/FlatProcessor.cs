using SalesSystem.Core.Models;
using SalesSystem.Core.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SalesSystem.Core.Services;

public class FlatProcessor : IDocumentProcessor
{
    private readonly StorageService _storage;
    private readonly DocumentGenerator _generator;
    private readonly TableGenerator _tableGenerator = new();

    public FlatProcessor(StorageService storage, DocumentGenerator generator)
    {
        _storage = storage;
        _generator = generator;
    }

    public (string PurchaseFile, string WarrantyFile) Process(OrderRequest request, Owner owner, List<Product> selectedProducts)
    {
        string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedDocuments");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string purchasePath = "";
        string warrantyPath = "";

        // --- ЛОГИКА ДОКУМЕНТА ПОКУПКИ (ЧЕК ИЛИ ДОГОВОР) ---
        if (request.TargetDocument == "Both" || request.TargetDocument == "Purchase")
        {
            purchasePath = Path.Combine(folderPath, $"Flat_Purchase_{DateTime.Now.Ticks}.pdf");
            string finalText = "";

            if (request.IsWholesale)
            {
                // ЛОГИКА ДОГОВОРА (КОПИРУЕМ ВСЁ ИЗ WHOLESALE_CONTRACT)
                string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "WholesaleContract.json"));
                var template = JsonSerializer.Deserialize<JsonElement>(json);
                finalText = template.GetProperty("Header").GetString() + "\n\n" + template.GetProperty("Body").GetString();

                var customer = _storage.RegisteredCustomers.FirstOrDefault(c => c.Name == request.Name);
                var supplier = _storage.Suppliers.FirstOrDefault();
                
                finalText = finalText
                    .Replace("{{DocNumber}}", DateTime.Now.Ticks.ToString().Substring(10))
                    .Replace("{{CurrentDate}}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                    .Replace("{{OwnerName}}", owner.Name)
                    .Replace("{{DirectorName}}", owner.DirectorName)
                    .Replace("{{AccountantName}}", owner.AccountantName)
                    .Replace("{{DirectorSignature}}", owner.DirectorSignature)
                    .Replace("{{StampPath}}", owner.StampPath)
                    .Replace("{{CustomerName}}", customer?.Name ?? "---")
                    .Replace("{{CustomerINN}}", customer?.INN ?? "---")
                    .Replace("{{CustomerSignature}}", customer?.Signature ?? "---")
                    .Replace("{{SupplierName}}", supplier?.Name ?? "---")
                    .Replace("{{SupplierINN}}", supplier?.INN ?? "---");

                if (request.IsDelivery)
                {
                    var delivery = _storage.DeliveryFleet.FirstOrDefault(d => d.Status == "Свободен");
                    string deliveryInfo = template.GetProperty("DeliveryTemplate").GetString()
                        .Replace("{{CourierName}}", delivery?.CourierName)
                        .Replace("{{VehicleNumber}}", delivery?.VehicleNumber)
                        .Replace("{{DeliveryPrice}}", delivery?.DeliveryPrice.ToString());
                    finalText = finalText.Replace("{{DeliverySection}}", deliveryInfo);
                }
                else
                {
                    finalText = finalText.Replace("{{DeliverySection}}", template.GetProperty("SelfDeliveryTemplate").GetString());
                }

                var (table, total) = _tableGenerator.GenerateTable(selectedProducts, template.GetProperty("ItemRowTemplate").GetString());
                finalText = finalText.Replace("{{ItemsTable}}", table).Replace("{{TotalSum}}", total.ToString("N2"));
            }
            else
            {
                // ЛОГИКА ЧЕКА (КОПИРУЕМ ВСЁ ИЗ RETAIL_RECEIPT)
                string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "RetailReceipt.json"));
                var template = JsonSerializer.Deserialize<JsonElement>(json);
                finalText = template.GetProperty("Header").GetString() + "\n\n" + template.GetProperty("Body").GetString();

                finalText = finalText
                    .Replace("{{OwnerName}}", owner.Name)
                    .Replace("{{CurrentDate}}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

                var (table, total) = _tableGenerator.GenerateTable(selectedProducts, template.GetProperty("ItemRow").GetString());
                finalText = finalText.Replace("{{ItemsTable}}", table).Replace("{{TotalSum}}", total.ToString("N2"));
            }

            _generator.GenerateDocument(finalText, purchasePath);
        }

        // --- ЛОГИКА ГАРАНТИИ (ТАЛОН ИЛИ ТЕХ.РЕГЛАМЕНТ) ---
        if (request.TargetDocument == "Both" || request.TargetDocument == "Warranty")
        {
            warrantyPath = Path.Combine(folderPath, $"Flat_Warranty_{DateTime.Now.Ticks}.pdf");
            string finalText = "";

            if (request.IsWholesale)
            {
                // ЛОГИКА ОПТОВОЙ ГАРАНТИИ
                string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "WholesaleWarranty.json"));
                var template = JsonSerializer.Deserialize<JsonElement>(json);
                finalText = template.GetProperty("Header").GetString() + "\n\n" + template.GetProperty("Body").GetString() + "\n\n" + template.GetProperty("Footer").GetString();

                var customer = _storage.RegisteredCustomers.FirstOrDefault(c => c.Name == request.Name);
                var supplier = _storage.Suppliers.FirstOrDefault();

                finalText = finalText
                    .Replace("{{DocNumber}}", "W-" + DateTime.Now.Ticks.ToString().Substring(12))
                    .Replace("{{OwnerName}}", owner.Name)
                    .Replace("{{DirectorName}}", owner.DirectorName)
                    .Replace("{{AccountantName}}", owner.AccountantName)
                    .Replace("{{DirectorSignature}}", owner.DirectorSignature)
                    .Replace("{{StampPath}}", owner.StampPath)
                    .Replace("{{CustomerName}}", customer?.Name ?? "---")
                    .Replace("{{CustomerINN}}", customer?.INN ?? "---")
                    .Replace("{{CustomerSignature}}", customer?.Signature ?? "---")
                    .Replace("{{SupplierName}}", supplier?.Name ?? "---")
                    .Replace("{{SupplierINN}}", supplier?.INN ?? "---")
                    .Replace("{{CurrentDate}}", DateTime.Now.ToString("F"));
            }
            else
            {
                // ЛОГИКА РОЗНИЧНОЙ ГАРАНТИИ
                string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "RetailWarranty.json"));
                var template = JsonSerializer.Deserialize<JsonElement>(json);
                finalText = template.GetProperty("Header").GetString() + "\n\n" + template.GetProperty("Body").GetString();

                finalText = finalText
                    .Replace("{{DocNumber}}", DateTime.Now.Ticks.ToString().Substring(12))
                    .Replace("{{OwnerName}}", owner.Name)
                    .Replace("{{CurrentDate}}", DateTime.Now.ToString("F"));
            }

            _generator.GenerateDocument(finalText, warrantyPath);
        }

        return (purchasePath, warrantyPath);
    }
}