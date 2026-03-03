namespace SalesSystem.Core.Services;
using SalesSystem.Core.Models;
using System.Text.Json;
using System.IO;

public class StorageService
{
    private readonly string _productsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "inventory.json");
    private readonly string _customersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "customers.json");
    private readonly string _suppliersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "suppliers.json");
    private readonly string _ownerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "owner.json");
    private readonly string _deliveryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "delivery.json");
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public List<Product> Inventory { get; set; } = new();
    public List<WholesaleCustomer> RegisteredCustomers { get; set; } = new();
    public List<Supplier> Suppliers { get; set; } = new();
    public Owner StoreOwner { get; set; } = new();
    public List<DeliveryInfo> DeliveryFleet { get; set; } = new();

    public StorageService()
    {
        LoadAll();
    }

    public void LoadAll()
    {
        Inventory = LoadFromFile<List<Product>>(_productsPath) ?? new();
        RegisteredCustomers = LoadFromFile<List<WholesaleCustomer>>(_customersPath) ?? new();
        Suppliers = LoadFromFile<List<Supplier>>(_suppliersPath) ?? new();
        StoreOwner = LoadFromFile<Owner>(_ownerPath) ?? new();
        DeliveryFleet = LoadFromFile<List<DeliveryInfo>>(_deliveryPath) ?? new();
    }

    public void SaveAll()
    {
        SaveToFile(_productsPath, Inventory);
        SaveToFile(_customersPath, RegisteredCustomers);
        SaveToFile(_suppliersPath, Suppliers);
        SaveToFile(_ownerPath, StoreOwner);
        SaveToFile(_deliveryPath, DeliveryFleet);
    }

    private T? LoadFromFile<T>(string path)
    {
        if (!File.Exists(path)) return default;
        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<T>(json);
    }

    private void SaveToFile<T>(string path, T data)
    {
        string json = JsonSerializer.Serialize(data, _options);
        File.WriteAllText(path, json);
    }
}