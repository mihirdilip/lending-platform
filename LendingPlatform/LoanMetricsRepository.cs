using System.Text.Json;

namespace LendingPlatform
{
    internal interface ILoanMetricsRepository
    {
        Task<LoanMetricsSummary> GetSummaryAsync(CancellationToken cancellationToken);
        Task SaveSummaryAsync(LoanMetricsSummary summary, CancellationToken cancellationToken);
    }

    /*
     * Just a mock up to emulate persistent storage by using local json file.
     * Just need to add the relavant implementation to use SQL/NOSQL DB and swap it on the IoC container.
     */
    internal class LoanMetricsJsonFileRepository : ILoanMetricsRepository
    {
        private const string _datasetFileName = "DataSet_LoanMetricsSummary.json";
        private readonly string _datasetfilePath = Path.Combine(Environment.CurrentDirectory, _datasetFileName);
        private LoanMetricsSummary _summary = new LoanMetricsSummary();

        public LoanMetricsJsonFileRepository()
        {
            if (!File.Exists(_datasetfilePath))
            {
                using var file = File.Create(_datasetfilePath);
            }
            else
            {
                var fileContent = File.ReadAllBytes(_datasetfilePath);
                if (fileContent != null && fileContent.Length > 0)
                {
                    var reader = new ReadOnlySpan<byte>(fileContent);
                    if (!reader.IsEmpty)
                        _summary = JsonSerializer.Deserialize<LoanMetricsSummary>(reader) ?? new LoanMetricsSummary();
                }
            }
        }

        public Task<LoanMetricsSummary> GetSummaryAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_summary);
        }

        public async Task SaveSummaryAsync(LoanMetricsSummary summary, CancellationToken cancellationToken)
        {
            _summary = summary;
            await File.WriteAllBytesAsync(_datasetfilePath, JsonSerializer.SerializeToUtf8Bytes(_summary), cancellationToken);
        }
    }
}
