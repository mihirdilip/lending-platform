using LendingPlatform.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LendingPlatform.LoanApplication
{
    internal interface ILoanApplicationWriter
    {
        public Task<LoanApplicationResult> ApplyAsync(LoanApplicationRequest request, CancellationToken cancellationToken);
    }

    internal class LoanApplicationWriterForConsole : ILoanApplicationWriter
    {
        private readonly ILogger<LoanApplicationWriterForConsole> _logger;
        private readonly IMediator _mediator;
        private readonly ILoanApplicationValidator _validator;
        private readonly ILoanApplicationRepository _repository;

        public LoanApplicationWriterForConsole(ILogger<LoanApplicationWriterForConsole> logger, IMediator mediator, ILoanApplicationValidator validator, ILoanApplicationRepository repository)
        {
            _logger = logger;
            _mediator = mediator;
            _validator = validator;
            _repository = repository;
        }

        public async Task<LoanApplicationResult> ApplyAsync(LoanApplicationRequest request, CancellationToken cancellationToken)
        {
            var result = new LoanApplicationResult(request);

            try
            {
                await _validator.ValidateAsync(request, CancellationToken.None);
            }
            catch (LoanApplicationException ex)
            {
                result.SetFailed(ex.Message, ex);
                _logger.LogTrace(ex, ex.Message);
            }
            catch (Exception ex)
            {
                result.SetFailed(ex.Message, ex);
                _logger.LogError(ex, ex.Message);
            }

            Console.WriteLine(result.ToString());
            Console.WriteLine();

            try
            {
                await _repository.SaveAsync(result, cancellationToken);
                await _mediator.Publish(new LoanApplicationAppliedEvent(result), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return result;
        }
    }
}
