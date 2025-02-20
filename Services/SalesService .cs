using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebAPITemplate.BusinessModel;
using WebAPITemplate.DatabaseEntities;
using WebAPITemplate.Interface;

public class SalesService : ISalesService
{
    private readonly List<SaleRecord> _sales = new();
    private readonly ILogger<SalesService> _logger;

    public SalesService(ILogger<SalesService> logger)
    {
        _logger = logger;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "salesdataTemplate", "sales_data.csv");

        if (!File.Exists(filePath))
        {
            _logger.LogError($"File not found at: {filePath}");
        }
        else
        {
            _sales = LoadSalesData(filePath);
        }
    }

    private List<SaleRecord> LoadSalesData(string filePath)
    {
        var sales = new List<SaleRecord>();

        try
        {
            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0)
            {
                _logger.LogError("Sales data file is empty!");
                return sales;
            }

            for (int i = 1; i < lines.Length; i++)  // Skip header row
            {
                var parts = lines[i].Split(',');

                if (parts.Length < 4)
                {
                    _logger.LogError($"Skipping invalid row: {lines[i]}");
                    continue;
                }

                try
                {
                    var sale = new SaleRecord
                    {
                        Date = DateTime.ParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        SKU = parts[1],
                        UnitPrice = decimal.Parse(parts[2], CultureInfo.InvariantCulture),
                        Quantity = int.Parse(parts[3]),
                    };
                    sale.TotalPrice = sale.UnitPrice * sale.Quantity;
                    sales.Add(sale);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error parsing row '{lines[i]}': {ex.Message}");
                }
            }

            _logger.LogInformation($"Loaded {sales.Count} sales records.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to read sales data: {ex.Message}");
        }

        return sales;
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetTotalSales()
    {
        decimal total = 0;
        foreach (var sale in _sales)
        {
            total += sale.TotalPrice;
        }

        return CreateResponse(true, "Total Sales Retrieved", total);
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetMonthlySales()
    {
        var monthlySales = new Dictionary<string, decimal>();

        foreach (var sale in _sales)
        {
            string key = $"{sale.Date.Year}-{sale.Date.Month:D2}";  // YYYY-MM format

            if (!monthlySales.ContainsKey(key))
                monthlySales[key] = 0;

            monthlySales[key] += sale.TotalPrice;
        }

        return CreateResponse(true, "Monthly Sales Retrieved", monthlySales);
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetMostPopularItemEachMonth()
    {
        var monthlyItems = new Dictionary<string, Dictionary<string, int>>();

        foreach (var sale in _sales)
        {
            string monthKey = $"{sale.Date.Year}-{sale.Date.Month:D2}";

            if (!monthlyItems.ContainsKey(monthKey))
                monthlyItems[monthKey] = new Dictionary<string, int>();

            if (!monthlyItems[monthKey].ContainsKey(sale.SKU))
                monthlyItems[monthKey][sale.SKU] = 0;

            monthlyItems[monthKey][sale.SKU] += sale.Quantity;
        }

        var mostPopularItems = new Dictionary<string, string>();

        foreach (var month in monthlyItems)
        {
            string mostPopularSKU = "";
            int maxQty = 0;

            foreach (var item in month.Value)
            {
                if (item.Value > maxQty)
                {
                    mostPopularSKU = item.Key;
                    maxQty = item.Value;
                }
            }

            mostPopularItems[month.Key] = mostPopularSKU;
        }

        return CreateResponse(true, "Most Popular Item Each Month Retrieved", mostPopularItems);
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetTopRevenueItemsEachMonth()
    {
        var revenueItems = new Dictionary<string, Dictionary<string, decimal>>();

        foreach (var sale in _sales)
        {
            string monthKey = $"{sale.Date.Year}-{sale.Date.Month:D2}";

            if (!revenueItems.ContainsKey(monthKey))
                revenueItems[monthKey] = new Dictionary<string, decimal>();

            if (!revenueItems[monthKey].ContainsKey(sale.SKU))
                revenueItems[monthKey][sale.SKU] = 0;

            revenueItems[monthKey][sale.SKU] += sale.TotalPrice;
        }

        var topRevenueItems = new Dictionary<string, string>();

        foreach (var month in revenueItems)
        {
            string topSKU = "";
            decimal maxRevenue = 0;

            foreach (var item in month.Value)
            {
                if (item.Value > maxRevenue)
                {
                    topSKU = item.Key;
                    maxRevenue = item.Value;
                }
            }

            topRevenueItems[month.Key] = topSKU;
        }

        return CreateResponse(true, "Top Revenue Items Each Month Retrieved", topRevenueItems);
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetPopularItemStats()
    {
        var monthlyItems = new Dictionary<string, Dictionary<string, int>>();

        // Step 1: Collect item sales per month
        foreach (var sale in _sales)
        {
            string monthKey = $"{sale.Date.Year}-{sale.Date.Month:D2}";

            if (!monthlyItems.ContainsKey(monthKey))
                monthlyItems[monthKey] = new Dictionary<string, int>();

            if (!monthlyItems[monthKey].ContainsKey(sale.SKU))
                monthlyItems[monthKey][sale.SKU] = 0;

            monthlyItems[monthKey][sale.SKU] += sale.Quantity;
        }

        var popularItemStats = new Dictionary<string, object>();

        // Step 2: Determine the most popular item each month and calculate min, max, avg
        foreach (var month in monthlyItems)
        {
            string mostPopularSKU = "";
            int maxQty = 0;

            // Find the most popular item for the month
            foreach (var item in month.Value)
            {
                if (item.Value > maxQty)
                {
                    mostPopularSKU = item.Key;
                    maxQty = item.Value;
                }
            }

            if (string.IsNullOrEmpty(mostPopularSKU))
            {
                popularItemStats[month.Key] = "No sales data";
                continue;
            }

            // Step 3: Calculate min, max, avg for the most popular item
            var orders = _sales
                .Where(s => $"{s.Date.Year}-{s.Date.Month:D2}" == month.Key && s.SKU == mostPopularSKU)
                .Select(s => s.Quantity)
                .ToList();

            if (orders.Count > 0)
            {
                popularItemStats[month.Key] = new
                {
                    Item = mostPopularSKU,
                    Min = orders.Min(),
                    Max = orders.Max(),
                    Avg = orders.Average()
                };
            }
        }

        return CreateResponse(true, "Monthly Popular Item Stats Retrieved", popularItemStats);
    }


    private (bool, ClsReturnResult) CreateResponse(bool isSuccess, string message, object data)
    {
        var result = new ClsReturnResult
        {
            RequestDTm = DateTime.Now,
            statusCode = isSuccess ? 200 : 500,
            isSuccess = isSuccess,
            StatusMessage = message,
            ReturnLst = data,
            TotalRecords = data is ICollection<object> list ? list.Count.ToString() : "1",
            ResponseDTm = DateTime.Now
        };
        result.ResponseDuration = result.ResponseDTm.Subtract(result.RequestDTm);
        return (isSuccess, result);
    }
}
