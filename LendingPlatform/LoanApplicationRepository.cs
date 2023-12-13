using System.Collections.Concurrent;
using System.Text.Json;

namespace LendingPlatform
{
    internal interface ILoanApplicationRepository
    {
        Task<List<LoanApplicationResult>> GetAllAsync(CancellationToken cancellationToken);
        Task SaveAsync(LoanApplicationResult result, CancellationToken cancellationToken);
    }

    /*
     * Just a mock up to emulate persistent storage by using local json file.
     * Just need to add the relavant implementation to use SQL/NOSQL DB and swap it on the IoC container.
     */
    internal class LoanApplicationJsonFileRepository : ILoanApplicationRepository
    {
        private const string _datasetFileName = "DataSet_LoanApplicationResult.json";
        private readonly string _datasetfilePath = Path.Combine(Environment.CurrentDirectory, _datasetFileName);
        private readonly ConcurrentQueue<LoanApplicationResult> _results = new ConcurrentQueue<LoanApplicationResult>();

        public LoanApplicationJsonFileRepository()
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
                        _results = new ConcurrentQueue<LoanApplicationResult>(JsonSerializer.Deserialize<List<LoanApplicationResult>>(reader) ?? new List<LoanApplicationResult>());
                }
            }
        }

        public Task<List<LoanApplicationResult>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_results.ToList());
        }

        public async Task SaveAsync(LoanApplicationResult result, CancellationToken cancellationToken)
        {
            _results.Enqueue(result);
            await File.WriteAllBytesAsync(_datasetfilePath, JsonSerializer.SerializeToUtf8Bytes(_results), cancellationToken);
        }
    }
}
