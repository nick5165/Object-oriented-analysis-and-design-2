namespace SalesSystem.Core.Factories;
using SalesSystem.Core.Products;
using SalesSystem.Core.Models;
using System.Collections.Generic;
public class WholeSalesFactory: ISalesFactory
{
    private List<Product> _items;
    private WholesaleCustomer _customer;
    private Supplier _supplier;
    private Owner _owner;
    private DeliveryInfo _deliveryInfo;

    public WholeSalesFactory(Owner owner, WholesaleCustomer customer, List<Product> items, Supplier supplier, DeliveryInfo deliveryInfo)
    {
        _owner = owner;
        _items = items;
        _customer = customer;
        _supplier = supplier;
        _deliveryInfo = deliveryInfo;
    }

    public IProofOfPurchase CreatePurchaseDocument()
    {
        return new WholesaleContract(_owner, _customer, _supplier, _items, _deliveryInfo);
    }
    
    public IWarranty CreateWarrantyDocument()
    {
        return new WholesaleQualityAssurance(_customer, _owner, _supplier);
    }
}