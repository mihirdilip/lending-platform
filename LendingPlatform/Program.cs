
int _totalApplicationsAccepted = 0;
int _totalApplicationsDeclined = 0;
decimal _totalLoanAmount = 0;
decimal _totalLoanToValue = 0;

/*
 * Assuming that the input values will be of correct format. 
 *      Would add trivial validations for input values.
 * Assuming total loan value metric is calculated only for successful applications.
 * Assuming mean average Loan to Value metric is calculated only for successful applications.
 * Collection of input values, validation logic, logic to calculate metrics and, printing of the metrics 
 *      Each of this can be extracted out to a method or a separate class to follow SOLID principles and make code more it more testable.
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
 * Convert the console app into micro-service / rest api and host it on some cloud service like azure api app, AKS, depending on the requirements.
 */

Console.WriteLine("Welcome to the Lending Platform!");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("------------------------------------------");

    Console.WriteLine();
    Console.WriteLine("New Loan Application - input below values,");

    Console.Write("Loan Amount (decimal; GBP): ");
    var loanAmount = Convert.ToDecimal(Console.ReadLine());

    Console.Write("Asset Value (decimal; GBP): ");
    var assetValue = Convert.ToDecimal(Console.ReadLine());

    Console.Write("Credit Score (1 - 999): ");
    var creditScore = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine();

    var isloadApplicationValid = IsLoanApplicationValid(loanAmount, assetValue, creditScore);
    if (isloadApplicationValid)
    {
        Console.WriteLine("Loan application was successful");
        _totalApplicationsAccepted++;
        _totalLoanAmount += loanAmount;
        _totalLoanToValue += (loanAmount / assetValue) * 100;
    }
    else
    {
        Console.WriteLine("Loan application was unsuccessful");
        _totalApplicationsDeclined++;
    }

    Console.WriteLine();
    Console.WriteLine("Metrics Summary, ");
    Console.WriteLine($"Total declined applications: {_totalApplicationsDeclined}");
    Console.WriteLine($"Total accepted applications: {_totalApplicationsAccepted}");
    Console.WriteLine($"Total value of accepted loans: {_totalLoanAmount:C}");
    Console.WriteLine($"Mean average Loan to Value for accepted loans: {(_totalLoanToValue / _totalApplicationsAccepted):0.##}%");

    Console.WriteLine("------------------------------------------");
    Console.WriteLine();

    Console.Write("Do you want to continue with next application? Press enter to default to Y. [Y/N]: ");
    var key = Console.ReadKey();
    Console.WriteLine();
    if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.N)
        break;
}

static bool IsLoanApplicationValid(decimal loanAmount, decimal assetValue, int creditScore)
{
    /*
    * The credit score of the applicant must be between 1 and 999
    * If the value of the loan is more than £1.5 million or less than £100,000 then the application must be declined
    * If the value of the loan is £1 million or more then the LTV must be 60% or less and the credit score of the applicant must be 950 or more
    * If the value of the loan is less than £1 million then the following rules apply:
    *      If the LTV is less than 60%, the credit score of the applicant must be 750 or more
    *      If the LTV is less than 80%, the credit score of the applicant must be 800 or more
    *      If the LTV is less than 90%, the credit score of the applicant must be 900 or more
    *      If the LTV is 90% or more, the application must be declined
    */

    if (loanAmount < 100000 || loanAmount > 1500000)
        return false;

    if (assetValue < 100000)
        return false;

    if (creditScore < 1 || creditScore > 999)
        return false;

    var loanToValuePercentage = (loanAmount / assetValue) * 100;

    if (loanAmount >= 1000000)
    {
        if (loanToValuePercentage > 60)
            return false;

        if (creditScore < 950)
            return false;
    }
    else
    {
        if (loanToValuePercentage >= 90)
            return false;

        if (loanToValuePercentage >= 80)
            if (creditScore < 900)
                return false;

        if (loanToValuePercentage >= 60)
            if (creditScore < 800)
                return false;

        if (creditScore < 750)
            return false;
    }

    return true;
}