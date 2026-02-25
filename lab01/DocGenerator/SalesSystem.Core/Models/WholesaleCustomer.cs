namespace SalesSystem.Core.Models;
public class WholesaleCustomer: BaseCustomer
{
    public string INN { get; set; }
    public string Signature { get; set; }
    public string PrintPath { get; set; }
}