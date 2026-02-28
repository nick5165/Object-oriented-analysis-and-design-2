namespace SalesSystem.Core.Factories;
using SalesSystem.Core.Products;
using SalesSystem.Core.Models;
using System.Collections.Generic;

public class RetailSalesFactory: ISalesFactory
{
    private Owner _owner;
    private List<Product> _items;

    public RetailSalesFactory(List<Product> items, Owner owner)
    {
        _owner = owner;
        _items = items;
    }
    
    public IProofOfPurchase CreatePurchaseDocument()
    {
        return new RetailReceipt(_items, _owner);
    }

    public IWarranty CreateWarrantyDocument()
    {
        return new RetailWarrantyCard(_owner);
    }
}