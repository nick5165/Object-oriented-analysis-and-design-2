namespace SalesSystem.Core.Models;
public class WholesaleCustomer: BaseCustomer
{
    public string INN { get; set; }  = string.Empty;
    public string Signature { get; set; }  = string.Empty;
    public string PrintPath { get; set; }  = string.Empty;
}