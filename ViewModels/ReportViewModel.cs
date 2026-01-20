using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using PeopleCounterDesktop.Services;
using PeopleCounterDesktop.Views.Popups;
using System.Collections.ObjectModel;

namespace PeopleCounterDesktop.ViewModels
{
    public partial class ReportViewModel : ObservableObject
    {
        private readonly PeopleCounterApiService _api;

        private static double _defaultMin;
        private static double _defaultMax;
        private bool _isDown;

        public ObservableCollection<string> DeviceList { get; } = new();
        public ObservableCollection<string> LocationList { get; } = new();

        public ObservableCollection<DateTimePoint> InValues { get; } = new();
        public ObservableCollection<DateTimePoint> OutValues { get; } = new();

        public ObservableCollection<ObservablePoint> BarInValues { get; } = new();
        public ObservableCollection<ObservablePoint> BarOutValues { get; } = new();
        public ObservableCollection<string> BarLabels { get; } = new();
        public ObservableCollection<DateTime> BarTimes { get; } = new();

        [ObservableProperty] private double minX;
        [ObservableProperty] private double maxX;
        [ObservableProperty] private double minXThumb;
        [ObservableProperty] private double maxXThumb;

        [ObservableProperty] private string selectedDevice;
        [ObservableProperty] private string selectedLocation;
        [ObservableProperty] private DateTime fromDate = DateTime.Today;
        [ObservableProperty] private DateTime toDate = DateTime.Today.AddDays(1);
        [ObservableProperty] private string selectedBucket = "hour";
        [ObservableProperty] private string selectedReportType = "Locations";
        [ObservableProperty] private bool isLineChartVisible = true;

        [ObservableProperty] private TimeSpan fromTime = DateTime.Now.TimeOfDay;
        [ObservableProperty] private TimeSpan toTime = DateTime.Now.TimeOfDay;

        private List<Models.SensorTrendDto> _lastResult;

        public string[] Buckets { get; } = { "hour", "day", "month" };
        public string[] ReportType { get; } = { "Locations", "Devices" };

        public ReportViewModel(PeopleCounterApiService api)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
        }

        public Func<double, string> TimeFormatter => value =>
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return string.Empty;

            var ticks = (long)value;

            if (ticks < DateTime.MinValue.Ticks || ticks > DateTime.MaxValue.Ticks)
                return string.Empty;

            try
            {
                var dateTime = new DateTime(ticks);
                return SelectedBucket switch
                {
                    "hour" => dateTime.ToString("dd MMM HH:mm"),    
                    "day" => dateTime.ToString("dd MMM"),            
                    "month" => dateTime.ToString("MMM yyyy"),        
                    _ => dateTime.ToString("HH:mm")
                };
            }
            catch
            {
                return string.Empty;
            }
        };

        public double TimeUnitWidth => SelectedBucket switch
        {
            "hour" => TimeSpan.FromHours(1).Ticks,        
            "day" => TimeSpan.FromDays(1).Ticks,           
            "month" => TimeSpan.FromDays(30).Ticks,        
            _ => TimeSpan.FromHours(1).Ticks
        };

        public double MinStep => SelectedBucket switch
        {
            "hour" => TimeSpan.FromHours(2).Ticks,         
            "day" => TimeSpan.FromDays(2).Ticks,           
            "month" => TimeSpan.FromDays(30).Ticks,        
            _ => TimeSpan.FromHours(1).Ticks
        };

        public DateTime EffectiveFromDate =>
        SelectedBucket == "hour"
        ? FromDate.Date + FromTime
        : SelectedBucket == "month"
            ? new DateTime(FromDate.Year, FromDate.Month, 1)
            : FromDate.Date;

        public DateTime EffectiveToDate =>
            SelectedBucket == "hour"
                ? ToDate.Date + ToTime
                : SelectedBucket == "month"
                    ? new DateTime(ToDate.Year, ToDate.Month,
                        DateTime.DaysInMonth(ToDate.Year, ToDate.Month))
                    : ToDate.Date.AddDays(1).AddTicks(-1);


        public async Task Init()
        {
            DeviceList.Clear();
            LocationList.Clear();
            var list = await _api.GetDevicesAsync();
            var listLocation = await _api.GetLocationAsync();

            foreach (var d in list)
            {
                DeviceList.Add(d);
            }
            foreach (var d in listLocation)
            {
                LocationList.Add(d);
            }
            SelectedDevice = DeviceList.FirstOrDefault() ?? "Select device";
            SelectedLocation = LocationList.FirstOrDefault() ?? "Select Location";
            SelectedReportType = ReportType.FirstOrDefault() ?? "Select Report";
        }

        [RelayCommand]
        private async Task OpenDevicePickerAsync()
        {
            var popup = new SelectionPopup(DeviceList);
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);

            if (result is string value)
                SelectedDevice = value;
        }

        [RelayCommand]
        private async Task OpenBucketPickerAsync()
        {
            var popup = new SelectionPopup(Buckets);
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);

            if (result is string value)
                SelectedBucket = value;
        }

        [RelayCommand]
        public void ToggleChartView()
        {
            IsLineChartVisible = !IsLineChartVisible;
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (SelectedReportType == "Devices" &&
                string.IsNullOrWhiteSpace(SelectedDevice))
                return;

            if (SelectedReportType == "Locations" &&
                string.IsNullOrWhiteSpace(SelectedLocation))
                return;


            _isDown = false;

            InValues.Clear();
            OutValues.Clear();

            BarInValues.Clear();
            BarOutValues.Clear();
            BarLabels.Clear();

            MinX = MaxX = MinXThumb = MaxXThumb = double.NaN;

            await Task.Delay(100);

            List<Models.SensorTrendDto> result;

            if (SelectedReportType == "Locations")
            {
                result = await _api.GetTrendLocationAsync(
                    SelectedLocation,
                    EffectiveFromDate,
                    EffectiveToDate,
                    SelectedBucket);
            }
            else
            {
                result = await _api.GetTrendAsync(
                    SelectedDevice,
                    EffectiveFromDate,
                    EffectiveToDate,
                    SelectedBucket);
            }

            if (result == null || result.Count == 0)
                return;

            _lastResult = result.ToList();

          
            //LINE CHART data
            foreach (var item in result)
            {
                InValues.Add(new DateTimePoint(item.Time, item.In));
                OutValues.Add(new DateTimePoint(item.Time, item.Out));
            }
            //Prepare BAR CHART data (limit recent points)
            var barData = result;
            //BAR CHART with numeric X

            int index = 0;

            foreach (var item in barData)
            {
                BarInValues.Add(new ObservablePoint(index, item.In));
                BarOutValues.Add(new ObservablePoint(index, item.Out));

                BarTimes.Add(item.Time);

                index++;
            }

            //Setup LINE CHART initial window (time-based)
            var start = result.First().Time.Ticks;
            var end = result.Last().Time.Ticks;

            MinX = start;

            MaxX = SelectedBucket switch
            {
                "hour" => Math.Min(start + TimeUnitWidth * 10, end),
                "day" => Math.Min(start + TimeUnitWidth * 7, end),
                "month" => Math.Min(start + TimeUnitWidth * 6, end),
                _ => Math.Min(start + TimeUnitWidth * 10, end)
            };


            if (BarInValues.Count > 0)
            {
                MinX = 0;

                MaxX = SelectedBucket switch
                {
                    "hour" => Math.Min(10, BarInValues.Count - 1),  
                    "day" => Math.Min(7, BarInValues.Count - 1),   
                    "month" => Math.Min(6, BarInValues.Count - 1),
                    _ => Math.Min(10, BarInValues.Count - 1)
                };
            }

            MinXThumb = MinX;
            MaxXThumb = MaxX;
        }

        public Func<double, string> BarTimeFormatter => value =>
        {
            int index = (int)Math.Round(value);

            if (index < 0 || index >= BarTimes.Count)
                return string.Empty;

            var dt = BarTimes[index];

            return SelectedBucket switch
            {
                "hour" => dt.ToString("dd MMM HH:mm"),   
                "day" => dt.ToString("dd MMM"),
                "month" => dt.ToString("MMM yyyy"),
                _ => dt.ToString("HH:mm")
            };
        };


        [RelayCommand]
        public void ChartUpdated(ChartCommandArgs args)
        {
            var chart = (CartesianChartEngine)args.Chart.CoreChart;
            var x = chart.XAxes.First();

            MinXThumb = x.MinLimit ?? MinX;
            MaxXThumb = x.MaxLimit ?? MaxX;
        }

        [RelayCommand]
        public void PointerDown(PointerCommandArgs args) => _isDown = true;

        [RelayCommand]
        public void PointerUp(PointerCommandArgs args) => _isDown = false;

        [RelayCommand]
        public void PointerMove(PointerCommandArgs args)
        {
            if (!_isDown) return;

            var chart = (ICartesianChartView)args.Chart;
            var pos = chart.ScalePixelsToData(args.PointerPosition);

            var range = MaxXThumb - MinXThumb;

            MinX = MinXThumb = pos.X - range / 2;
            MaxX = MaxXThumb = pos.X + range / 2;
        }



        [RelayCommand]
        private async Task ExportToExcelAsync()
        {
            if (_lastResult == null || _lastResult.Count == 0)
                return;

            try
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var ws = workbook.Worksheets.Add("Report");

                ws.Cell(1, 1).Value = "Time";
                ws.Cell(1, 2).Value = "In";
                ws.Cell(1, 3).Value = "Out";

                ws.Range(1, 1, 1, 3).Style.Font.Bold = true;

                var exportData = GetGroupedForExport();

                ws.Cell(1, 1).Value = SelectedBucket switch
                {
                    "hour" => "Hour",
                    "day" => "Day",
                    "month" => "Month",
                    _ => "Time"
                };

                ws.Cell(1, 2).Value = "In";
                ws.Cell(1, 3).Value = "Out";

                ws.Range(1, 1, 1, 3).Style.Font.Bold = true;

                int row = 2;

                foreach (var item in exportData)
                {
                    ws.Cell(row, 1).Value = item.Time;

                    ws.Cell(row, 1).Style.DateFormat.Format = SelectedBucket switch
                    {
                        "hour" => "yyyy-MM-dd HH:mm",
                        "day" => "yyyy-MM-dd",
                        "month" => "yyyy-MM",
                        _ => "yyyy-MM-dd HH:mm"
                    };

                    ws.Cell(row, 2).Value = item.In;
                    ws.Cell(row, 3).Value = item.Out;

                    row++;
                }


                ws.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                var result = await FileSaver.Default.SaveAsync(
                    fileName,
                    stream,
                    new CancellationToken());

                if (result.IsSuccessful)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Export",
                        $"Excel exported to:\n{result.FilePath}",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Export Failed",
                    ex.Message,
                    "OK");
            }
        }



        private List<(DateTime Time, int In, int Out)> GetGroupedForExport()
        {
            if (_lastResult == null || _lastResult.Count == 0)
                return new List<(DateTime, int, int)>();

            return SelectedBucket switch
            {
                "hour" => _lastResult
                    .GroupBy(x => new DateTime(x.Time.Year, x.Time.Month, x.Time.Day, x.Time.Hour, 0, 0))
                    .OrderBy(g => g.Key)
                    .Select(g => (
                        Time: g.Key,
                        In: g.Sum(x => x.In),
                        Out: g.Sum(x => x.Out)
                    ))
                    .ToList(),

                "day" => _lastResult
                    .GroupBy(x => x.Time.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => (
                        Time: g.Key,
                        In: g.Sum(x => x.In),
                        Out: g.Sum(x => x.Out)
                    ))
                    .ToList(),

                "month" => _lastResult
                    .GroupBy(x => new DateTime(x.Time.Year, x.Time.Month, 1))
                    .OrderBy(g => g.Key)
                    .Select(g => (
                        Time: g.Key,
                        In: g.Sum(x => x.In),
                        Out: g.Sum(x => x.Out)
                    ))
                    .ToList(),

                _ => new List<(DateTime, int, int)>()
            };
        }

    }
}