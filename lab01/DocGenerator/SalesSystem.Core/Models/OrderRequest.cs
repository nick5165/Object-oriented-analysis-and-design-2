using System.Collections.Generic;

namespace SalesSystem.Core.Models;

public class OrderItem
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class OrderRequest
{
    public string Name { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public bool IsWholesale { get; set; }
    public bool IsDelivery { get; set; }
    public string TargetDocument { get; set; } = "Both";
}