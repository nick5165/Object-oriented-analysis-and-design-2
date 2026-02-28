namespace SalesSystem.Core.Factories;
using SalesSystem.Core.Products;
public interface ISalesFactory
{
    IProofOfPurchase CreatePurchaseDocument();
    
    IWarranty CreateWarrantyDocument();
}