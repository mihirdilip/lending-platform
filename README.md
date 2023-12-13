# Notes

* Assuming that the input values will be of correct format. 
    * Would add trivial validations for input values.
* Assuming total loan value metric is calculated only for successful applications.
* Assuming mean average Loan to Value metric is calculated only for successful applications.
* Emulating persistence of loan applications and summary data by saving them to local json files. 
    * This can be SQL/NOSQL database in production.
* Using pub-sub messaging of events to make things loosely coupled and scalable. 
    * Collection & reporting of loan metrics summary is utilising this. 
    * This is in memory for now but can be over the network using Azure Service Bus or something similar.
* Add unit tests and integration tests as required.
* Use clean architechture principles for organising the code structure if this service needs expose more features.
* Have metrics collected by some telemetry platform like Azure App Insights/Monitor, Grafana, Prometheus, etc.
* Add comments for all the classes, properties, methods, etc.
* Use Pull Requests and CI validations before merging into main branch.
* Convert the console app into micro-service / rest api and host it on some cloud service like azure api app, AKS, depending on the requirements.

## Phase 1
[Phase 1 tag/branch](https://github.com/mihirdilip/lending-platform/tree/phase-1)

Implemented minimal basic console app with business logic. Nothing fancy here.


## Phase 2
[Phase 2 tag/branch](https://github.com/mihirdilip/lending-platform/tree/phase-2)

* Extracted collection of input values into a method.
* Added `LoanApplicationRequest` class to hold input values and pass to other methods/classes.
* Extracted validation logic to `DefaultLoanApplicationValidator` class implementing `ILoanApplicationValidator` interface. 
* Extracted code to run validation, logic to calculate metrics and printing of the metrics to `LoanApplicationWriterForConsole` class implementing `ILoanApplicationWriter` interface.
* Added a test project for testing the validation logic.


## Phase 3
[Phase 3 tag/branch](https://github.com/mihirdilip/lending-platform/tree/phase-3)

* Added below repositories to emulate persistence of loan applications and summary data by saving them to local json files.  
    * `LoanApplicationJsonFileRepository` class implementing `ILoanApplicationRepository` interface and a supporting class `LoanApplicationResult`.
    * `LoanMetricsJsonFileRepository` class implementing `ILoanMetricsRepository` interface and a supporting class `LoanMetricsSummary`.
    * This can be SQL/NOSQL database in production.
* Introduced IoC and using DI.


## Phase 4
[Phase 4 tag/branch](https://github.com/mihirdilip/lending-platform/tree/phase-4)

* Added LoanApplication & LoanMetrics folders and moved relavent files.
* Added in-memory pub-sub messaging using MediatR to make things event driven, loosely coupled and scalable.
    * This can be over the network using Azure Service Bus or something similar.
* Added abstract `DomainEvent` class implementing `IDomainEvent` interface.
* Added `LoanApplicationAppliedEvent` which is published when loan application is submitted.
* Added `LoanApplicationAppliedHandler` which subscribes to `LoanApplicationAppliedEvent` for updating loan metrics summary.
* Added `LoanMetricsSummaryUpdatedEvent` which is published when loan metrics summary is updated.
* Added `LoanMetricsSummaryUpdatedConsoleLogHandler` which subscribes to `LoanMetricsSummaryUpdatedEvent` for reporting/printing loan metrics summary on the console.

-------------------------------------------------

# Challenge - Lending Platform
Build a simple Console application using the technology of your choice (preferably C#) that enables the writing and reporting of loans as per the requirements below. This should be approached as a way that can demonstrate your process to solving problems (any required infrastructure can simply be mocked), and does not need to be built to a production standard. Instead the exercise should be timeboxed to no longer than an hour. Notes can be taken of any assumptions made, and also any other considerations or improvements that you might make if this was a production application.

## Requirements
### User inputs that the application should require:
* Amount that the loan is for in GBP
* The value of the asset that the loan will be secured against
* The credit score of the applicant (between 1 and 999)

### Metrics that the application should output:
* Whether or not the applicant was successful
* The total number of applicants to date, broken down by their success status
* The total value of loans written to date
* The mean average Loan to Value of all applications received to date
    * Loan to Value (LTV) is the amount that the loan is for, expressed as a percentage of the value of the asset that the loan will be secured against.

### Business rules used to derive whether or not the applicant was successful:
* If the value of the loan is more than £1.5 million or less than £100,000 then the application must be declined
* If the value of the loan is £1 million or more then the LTV must be 60% or less and the credit score of the applicant must be 950 or more
* If the value of the loan is less than £1 million then the following rules apply:
    * If the LTV is less than 60%, the credit score of the applicant must be 750 or more
    * If the LTV is less than 80%, the credit score of the applicant must be 800 or more
    * If the LTV is less than 90%, the credit score of the applicant must be 900 or more
    * If the LTV is 90% or more, the application must be declined