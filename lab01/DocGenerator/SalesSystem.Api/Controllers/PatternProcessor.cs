using SalesSystem.Core.Factories;
using SalesSystem.Core.Models;
using SalesSystem.Core.Products;
using System;
using System.Collections.Generic;
using System.IO;

namespace SalesSystem.Core.Services;

public class PatternProcessor : IDocumentProcessor
{
    private readonly StorageService _storage;
    private readonly DocumentGenerator _generator;

    public PatternProcessor(StorageService storage, DocumentGenerator generator)
    {
        _storage = storage;
        _generator = generator;
    }

    public (string PurchaseFile, string WarrantyFile) Process(OrderRequest request, Owner owner, List<Product> selectedProducts)
    {
        ISalesFactory factory;
        if (request.IsWholesale)
        {
            var customer = _storage.RegisteredCustomers.Find(c => c.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase));
            var supplier = _storage.Suppliers[0];
            DeliveryInfo? delivery = request.IsDelivery ? _storage.DeliveryFleet.Find(d => d.Status == "Свободен") : null;
            factory = new WholeSalesFactory(owner, customer!, selectedProducts, supplier, delivery!);
        }
        else
        {
            factory = new RetailSalesFactory(owner, selectedProducts);
        }

        string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedDocuments");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string pFile = "", wFile = "";

        if (request.TargetDocument == "Both" || request.TargetDocument == "Purchase")
        {
            pFile = Path.Combine(folderPath, $"Pattern_Purchase_{DateTime.Now.Ticks}.pdf");
            _generator.GenerateDocument(factory.CreatePurchaseDocument().GetDocumentData(), pFile);
        }

        if (request.TargetDocument == "Both" || request.TargetDocument == "Warranty")
        {
            wFile = Path.Combine(folderPath, $"Pattern_Warranty_{DateTime.Now.Ticks}.pdf");
            var wDoc = factory.CreateWarrantyDocument();
            _generator.GenerateDocument(wDoc.GetWarrantyTerms() + "\n\n" + wDoc.GetValidationData(), wFile);
        }

        return (pFile, wFile);
    }
}