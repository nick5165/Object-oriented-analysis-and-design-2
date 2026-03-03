using Microsoft.AspNetCore.Mvc;
using SalesSystem.Core.Models;
using SalesSystem.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesSystem.Api.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly StorageService _storage;
    private readonly IDocumentProcessor _processor;

    public DocumentsController(StorageService storage, IDocumentProcessor processor)
    {
        _storage = storage;
        _processor = processor;
    }

    [HttpPost("generate")]
    public IActionResult GenerateOrder([FromBody] OrderRequest request)
    {
        var owner = _storage.StoreOwner;
        var selectedProducts = new List<Product>();

        foreach (var itemRequest in request.Items)
        {
            var dbProduct = _storage.Inventory.FirstOrDefault(p => p.Name.Equals(itemRequest.ProductName, StringComparison.OrdinalIgnoreCase));
            if (dbProduct == null) return BadRequest($"Товар '{itemRequest.ProductName}' не найден.");
            if (dbProduct.Count < itemRequest.Quantity) return BadRequest($"Недостаточно товара '{dbProduct.Name}'.");

            selectedProducts.Add(new Product { Name = dbProduct.Name, Price = dbProduct.Price, Count = itemRequest.Quantity });
        }

        // Вызываем процессор (какой именно - решит Program.cs)
        var (purchase, warranty) = _processor.Process(request, owner, selectedProducts);

        return Ok(new { PurchaseFile = purchase, WarrantyFile = warranty });
    }
}