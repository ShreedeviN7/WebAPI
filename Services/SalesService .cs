using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        string filePath = Path.Combine(AppContext.BaseDirectory, "salesdataTemplate", "sales_data.csv");

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

            bool isFirstRow = true; // Skip the header row

            foreach (var line in lines)
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }

                var parts = line.Split(',');

                if (parts.Length < 4)
                {
                    _logger.LogError($"Skipping invalid row: {line}");
                    continue;
                }

                try
                {
                    decimal unitPrice = decimal.Parse(parts[2], CultureInfo.InvariantCulture);
                    int quantity = int.Parse(parts[3]);

                    sales.Add(new SaleRecord
                    {
                        Date = DateTime.ParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        SKU = parts[1],
                        UnitPrice = unitPrice,
                        Quantity = quantity,
                        TotalPrice = unitPrice * quantity
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error parsing row '{line}': {ex.Message}");
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
        decimal total = _sales.Sum(sale => sale.TotalPrice);
        return CreateResponse(true, "Total Sales Retrieved", total);
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetMonthlySales()
    {
        var monthlySales = _sales
            .GroupBy(s => new { s.Date.Year, s.Date.Month })
            .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, TotalSales = g.Sum(s => s.TotalPrice) })
            .ToList();

        return CreateResponse(true, "Monthly Sales Retrieved", monthlySales);
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetMostPopularItemEachMonth()
    {
        var mostPopularItems = _sales
            .GroupBy(s => new { s.Date.Year, s.Date.Month, s.SKU })
            .Select(g => new { g.Key.Year, g.Key.Month, g.Key.SKU, TotalQuantitySold = g.Sum(s => s.Quantity) })
            .GroupBy(x => new { x.Year, x.Month })
            .Select(g => g.OrderByDescending(x => x.TotalQuantitySold).First())
            .ToList();

        return CreateResponse(true, "Most Popular Item Each Month Retrieved", mostPopularItems);
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetTopRevenueItemsEachMonth()
    {
        var topRevenueItems = _sales
            .GroupBy(s => new { s.Date.Year, s.Date.Month, s.SKU })
            .Select(g => new { g.Key.Year, g.Key.Month, g.Key.SKU, TotalRevenue = g.Sum(s => s.TotalPrice) })
            .GroupBy(x => new { x.Year, x.Month })
            .Select(g => g.OrderByDescending(x => x.TotalRevenue).First())
            .ToList();

        return CreateResponse(true, "Top Revenue Items Each Month Retrieved", topRevenueItems);
    }

    public async Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetPopularItemStats()
    {
        var popularItemStats = _sales
            .GroupBy(s => new { s.Date.Year, s.Date.Month, s.SKU })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.SKU,
                MinOrders = g.Min(s => s.Quantity),
                MaxOrders = g.Max(s => s.Quantity),
                AvgOrders = g.Average(s => s.Quantity)
            })
            .ToList();

        return CreateResponse(true, "Popular Item Stats Retrieved", popularItemStats);
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
            TotalRecords = (data as IList<object>)?.Count.ToString() ?? "1",
            ResponseDTm = DateTime.Now
        };
        result.ResponseDuration = result.ResponseDTm.Subtract(result.RequestDTm);
        return (isSuccess, result);
    }
}
