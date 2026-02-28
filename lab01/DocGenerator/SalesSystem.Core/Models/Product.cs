namespace SalesSystem.Core.Models;
public class Product
{
    public string Name { get; set; }  = string.Empty;
    public decimal Price { get; set; }
    public int Count { get; set; }
}