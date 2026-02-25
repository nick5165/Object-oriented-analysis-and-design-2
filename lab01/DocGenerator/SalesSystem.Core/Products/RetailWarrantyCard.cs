namespace SalesSystem.Core.Products;
public class RetailWarrantyCard: IWarranty
{
    public string GetWarrantyTerms()
    {
        string text = "Гарантийный срок составляет 12 месяцев. Возврат товара возможен в течение 14 дней при условии сохранения товарного вида и упаковки (Закон о защите прав потребителей).";
        return text;    
    }

    public string GetSignatureData()
    {
        string text = "Подпись продавца: [_________] М.П.";
        return text;
    }
}