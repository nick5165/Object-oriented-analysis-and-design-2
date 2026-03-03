using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Controls;
using SalesSystem.Core.Models;

namespace SalesSystem.Gui;

public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient = new HttpClient();

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnGenerateClick(object sender, RoutedEventArgs e)
    {
        StatusLabel.Text = "Обработка...";
        StatusLabel.Foreground = Brushes.Gray;

        var request = new OrderRequest
        {
            Name = CustomerNameBox.Text,
            IsWholesale = WholesaleCheck.IsChecked ?? false,
            IsDelivery = DeliveryCheck.IsChecked ?? false,
            TargetDocument = (DocTypeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Both",
            Items = ParseItems(ProductsBox.Text)
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5142/api/documents/generate", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                
                string fileToShow = "";
                if (result != null)
                {
                    if (result.ContainsKey("purchaseFile") && !string.IsNullOrEmpty(result["purchaseFile"]))
                        fileToShow = result["purchaseFile"];
                    else if (result.ContainsKey("warrantyFile") && !string.IsNullOrEmpty(result["warrantyFile"]))
                        fileToShow = result["warrantyFile"];
                }

                if (!string.IsNullOrEmpty(fileToShow))
                {
                    await PdfViewer.EnsureCoreWebView2Async();
                    PdfViewer.CoreWebView2.Navigate(fileToShow);
                }

                StatusLabel.Foreground = Brushes.Green;
                StatusLabel.Text = "Готово.";
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                StatusLabel.Foreground = Brushes.Red;
                StatusLabel.Text = error;
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Ошибка: {ex.Message}";
        }
    }

    private List<OrderItem> ParseItems(string input)
    {
        var list = new List<OrderItem>();
        var parts = input.Split(',');

        foreach (var part in parts)
        {
            var subParts = part.Split(':');
            string name = subParts[0].Trim();
            int qty = 1;

            if (subParts.Length > 1 && int.TryParse(subParts[1].Trim(), out int parsedQty))
            {
                qty = parsedQty;
            }

            list.Add(new OrderItem { ProductName = name, Quantity = qty });
        }
        return list;
    }
}