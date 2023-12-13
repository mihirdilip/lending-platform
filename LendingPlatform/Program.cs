
using LendingPlatform;

/*
 * Assuming that the input values will be of correct format. 
 *      Would add trivial validations for input values.
 * Assuming total loan value metric is calculated only for successful applications.
 * Assuming mean average Loan to Value metric is calculated only for successful applications.
 * We can emulate persistence of loan applications and summary data by saving it to a local json file. 
 *      This can be SQL/NOSQL database in production.
 * We can introduce IoC and use dependency injections.
 * We can introduce pub-sub messaging of events to make things loosely coupled can scalable.
 *      Collection & reporting of metrics can be refactored to use this. 
 *      This can be in memory for now but can be over the network using Azure Service Bus or something similar.
 * Add unit tests and integration tests as required.
 * Use clean architechture principles for organising the code structure if this service needs expose more features.
 * Have metrics collected by some telemetry platform like Azure App Insights/Monitor, Grafana, Prometheus, etc.
 * Add comments for all the classes, properties, methods, etc.
 * Use Pull Requests and CI validations before merging into main branch.
 * Convert the console app into micro-service / rest api and host it on some cloud service like azure api app, AKS, depending on the requirements.
 */

Console.WriteLine("Welcome to the Lending Platform!");

var loanWriter = new LoanApplicationWriterForConsole(new DefaultLoanApplicationValidator());

while (true)
{
    Console.WriteLine();
    Console.WriteLine("------------------------------------------");

    await loanWriter.ApplyAsync(GetUserInput(), CancellationToken.None);

    Console.WriteLine("------------------------------------------");
    Console.WriteLine();

    Console.Write("Do you want to continue with next application? Press enter to default to Y. [Y/N]: ");
    var key = Console.ReadKey();
    Console.WriteLine();
    if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.N)
        break;
}

static LoanApplicationRequest GetUserInput()
{
    /*
     * Assuming that the input values will be of correct format. Would add trivial validations for input values.
     */

    Console.WriteLine();
    Console.WriteLine("New Loan Application - input below values,");

    Console.Write("Loan Amount (decimal; GBP): ");
    var loanAmount = Convert.ToDecimal(Console.ReadLine());

    Console.Write("Asset Value (decimal; GBP): ");
    var assetValue = Convert.ToDecimal(Console.ReadLine());

    Console.Write("Credit Score (1 - 999): ");
    var creditScore = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine();

    return new LoanApplicationRequest(loanAmount, assetValue, creditScore);
}