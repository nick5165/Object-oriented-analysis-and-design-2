using SalesSystem.Core.Models;
using System.Collections.Generic;

namespace SalesSystem.Core.Services;

public interface IDocumentProcessor
{
    (string PurchaseFile, string WarrantyFile) Process(OrderRequest request, Owner owner, List<Product> selectedProducts);
}