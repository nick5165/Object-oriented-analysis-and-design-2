namespace SalesSystem.Core.Products;
using SalesSystem.Core.Models;
using System.Collections.Generic;
using System;

public class WholesaleContract: IProofOfPurchase
{
    public List<Product> Items { get; set; }
    public WholesaleCustomer Customer { get; set; }
    public WholesaleContract(WholesaleCustomer customer, List<Product> items)
    {
        Customer = customer;
        Items = items;
    }
    public string GetDocumentData()
    {
        Random rnd = new Random();
        
        string fullText = $"ДОГОВОР № {rnd.Next(1, 1000)}\n";
        fullText += $"Настоящий ДОГОВОР ПОСТАВКИ заключен между ООО 'ГлобалСервис' (Продавец) и {Customer.Name} (Покупатель), ИНН: {Customer.INN}.\n";
        fullText += $"Продавец обязуется передать, а Покупатель принять следующий перечень ТМЦ:\n";
        decimal totalSum = 0; 
        
        foreach (Product item in Items)
        {
            decimal posSum = item.Price * item.Count;
            fullText += $"Поз: {item.Name} | Кол-во: {item.Count} | Цена: {item.Price} | Сумма: {posSum}\n";
            totalSum += posSum;
        }
        
        fullText += $"------------------------------------------\n";
        fullText += $"ИТОГО К ОПЛАТЕ: {totalSum}\n";
        fullText += $"Срок оплаты: 5 рабочих дней. Условия поставки: самовывоз со склада Продавца.\n";
        fullText += $"СО СТОРОНЫ ПОКУПАТЕЛЯ: Документ подписан ЭЦП (хэш): {Customer.Signature}\n";
        return fullText; 
    }
}