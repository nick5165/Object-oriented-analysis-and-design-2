namespace SalesSystem.Core.Products;
using SalesSystem.Core.Models;
public class WholesaleQualityAssurance: IWarranty
{
    public WholesaleCustomer Customer { get; set; }
    public WholesaleQualityAssurance(WholesaleCustomer customer)
    {
        Customer = customer;
    }
    public string GetWarrantyTerms()
    {
        string text = "Гарантия качества предоставляется в соответствии с техническими условиями (ТУ). Приемка по качеству производится в течение 30 дней с момента подписания акта приема-передачи.";
        return text;
    }

    public string GetValidationData()
    {
        string text = "Документ верифицирован в системе ЭДО. Покупатель: {Customer.Name}. ЭЦП: {Customer.Signature}. Оттиск печати: {Customer.PrintPath}";
        return text;
    }
}