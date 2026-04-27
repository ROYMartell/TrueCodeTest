using System.Text;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using TrueCodeTest.CurrencyFetcher;
using TrueCodeTest.CurrencyFetcher.Cbr;
using TrueCodeTest.CurrencyFetcher.Options;
using TrueCodeTest.Finance.Infrastructure.Persistence;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<CurrencyFetcherOptions>(
    builder.Configuration.GetSection(CurrencyFetcherOptions.SectionName));

var financeConnection = builder.Configuration.GetConnectionString("FinanceDb")
    ?? throw new InvalidOperationException("ConnectionStrings:FinanceDb is not configured");
builder.Services.AddDbContext<FinanceDbContext>(o => o.UseNpgsql(financeConnection));

builder.Services.AddHttpClient<CbrClient>((sp, client) =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<CurrencyFetcherOptions>>().Value;
    client.Timeout = TimeSpan.FromSeconds(options.HttpTimeoutSeconds);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));

builder.Services.AddHostedService<CurrencyFetchWorker>();

var host = builder.Build();
await host.RunAsync();
