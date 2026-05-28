using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using PAN.context.Models;
using Microsoft.EntityFrameworkCore;

namespace PAN.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly GeipanContext _context;

    [ObservableProperty]
    private ISeries[] series;

    [ObservableProperty]
    private int totalCases;

    public AdminViewModel(GeipanContext context)
    {
        _context = context;
        LoadData();
    }

    private void LoadData()
    {
        var stats = new List<(string Name, int Count)>();
        
        try
        {
            // Try to get counts from database grouped by Classement
            var dbStats = _context.Evenement
                .Include(e => e.IdClassementNavigation)
                .Where(e => e.IdClassement != null)
                .GroupBy(e => e.IdClassementNavigation.Nom)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToList();

            foreach (var item in dbStats)
            {
                stats.Add((item.Name ?? "Inconnu", item.Count));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database query failed: {ex.Message}");
            // Better to see the error in the app if it fails
            throw; 
        }

        var pieSeriesList = new List<ISeries>();
        int total = stats.Sum(s => s.Count);

        if (total > 0)
        {
            foreach (var stat in stats)
            {
                pieSeriesList.Add(new PieSeries<double>
                {
                    Values = new double[] { stat.Count },
                    Name = stat.Name,
                    InnerRadius = 80, // Donut shape
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                    DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue} Cas\n({(point.Coordinate.PrimaryValue / total * 100):N1}%)"
                });
            }
        }

        Series = pieSeriesList.ToArray();
        TotalCases = total;
    }
}
