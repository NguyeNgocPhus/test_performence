using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ApiBackgroundJob : BackgroundService
{
    private readonly ILogger<ApiBackgroundJob> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiBackgroundJob(ILogger<ApiBackgroundJob> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                string url = "http://localhost:5001/api/Cronjob";

                HttpResponseMessage response = await client.GetAsync(url, stoppingToken);
                response.EnsureSuccessStatusCode();

                string body = await response.Content.ReadAsStringAsync(stoppingToken);

                _logger.LogInformation("API Response: {Body}", body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when calling API");
            }

            // chạy lại sau 1 phút
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}