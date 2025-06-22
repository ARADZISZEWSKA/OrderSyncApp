using System.Text.Json;

namespace ProjektMaui.Views;

public partial class ReportListPage : ContentPage
{
    private readonly string _token;
    private readonly HttpClient _httpClient;
    private List<ReportDto> _allReports = new();

    public ReportListPage(string token)
    {
        InitializeComponent();
        _token = token;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net/api/")
        };
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        FromDatePicker.Date = DateTime.Now.AddMonths(-1);
        ToDatePicker.Date = DateTime.Now;

        LoadReportsAsync();
    }

    private async Task LoadReportsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("report");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                _allReports = JsonSerializer.Deserialize<List<ReportDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ReportCollection.ItemsSource = _allReports;
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê pobraæ zg³oszeñ", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", ex.Message, "OK");
        }
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        if (_allReports == null || !_allReports.Any())
        {
            DisplayAlert("Brak danych", "Nie ma zg³oszeñ do filtrowania.", "OK");
            return;
        }

        var fromDate = FromDatePicker.Date;
        var toDate = ToDatePicker.Date.AddDays(1).AddSeconds(-1);

        var filtered = _allReports
            .Where(r => r.CreatedAt >= fromDate && r.CreatedAt <= toDate)
            .ToList();

        ReportCollection.ItemsSource = filtered;
    }

    public class ReportDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
