using Microservices.Application.Contracts;
using Microservices.Application.Models.Webhooks;
using Microservices.Banking.Core.Entities;
using Microservices.Banking.Core.Enums;
using Microservices.Banking.DataAccess.Contracts;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Microservices.Application.Jobs;

public sealed class SimulationJob : IJob
{
    private readonly ILogger<SimulationJob> _logger;
    private readonly ISimulationTaskRepository _simulationTaskRepository;
    private readonly IWebhookSender _webhookSender;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ISimulationTasksService _simulationTasksService;
    private readonly IWebhookConfigurationRepository _webhookConfigurationRepository;

    private static readonly TransactionStatus[] ProcessedStatuses =
    {
        TransactionStatus.Completed,
        TransactionStatus.Failed
    };
    
    public SimulationJob(
        ILogger<SimulationJob> logger,
        ISimulationTaskRepository simulationTaskRepository,
        IWebhookSender webhookSender,
        ITransactionRepository transactionRepository,
        ISimulationTasksService simulationTasksService,
        IWebhookConfigurationRepository webhookConfigurationRepository)
    {
        _logger = logger;
        _simulationTaskRepository = simulationTaskRepository;
        _webhookSender = webhookSender;
        _transactionRepository = transactionRepository;
        _simulationTasksService = simulationTasksService;
        _webhookConfigurationRepository = webhookConfigurationRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("{JobName} is fired.", nameof(SimulationJob));
        
        var pendingTasks = await _simulationTaskRepository.GetAll(DateTime.UtcNow);
        
        _logger.LogInformation("Found {Count} pending tasks.", pendingTasks.Count);

        if (!pendingTasks.Any())
        {
            return;
        }

        var webhookConfigurations = await _webhookConfigurationRepository.GetAll();

        var transactionTrackingIds = pendingTasks.Select(task => task.TransactionTrackingId).ToArray();

        var transactionsRaw = await _transactionRepository.GetAll(transactionTrackingIds);
        var transactions = transactionsRaw.ToDictionary(transaction => transaction.TrackingId);

        foreach (var simulationTask in pendingTasks)
        {
            var transaction =  transactions[simulationTask.TransactionTrackingId];
            transaction.Status = simulationTask.NewStatus;
            transaction.UsdFeeAmount = simulationTask.NewFeeAmount;
            await _transactionRepository.ReplaceOne(transaction);
        }
        
        foreach (var webhookConfiguration in webhookConfigurations)
        {
            foreach (var simulationTask in pendingTasks)
            {
                await SendForConfiguration(simulationTask, webhookConfiguration);
            }
        }

        foreach (var simulationTask in pendingTasks.Where(task => !ProcessedStatuses.Contains(task.NewStatus)))
        {
            var transaction = transactions[simulationTask.TransactionTrackingId];
            var newTask =_simulationTasksService.GenerateNext(transaction, transaction.Status);

            await _simulationTaskRepository.Insert(newTask);
        }
        
        var deleteResult = await _simulationTaskRepository.Delete(pendingTasks.Select(task => task.Id));
        _logger.LogInformation("{count} simulation tasks deleted.", deleteResult.DeletedCount);
    }

    private ValueTask SendForConfiguration(SimulationTask task, WebhookConfiguration configuration)
    {
        if (task.NewStatus is TransactionStatus.Processing && configuration.Type is WebhookType.TransactionInitiated ||
            task.NewStatus is TransactionStatus.Failed && configuration.Type is WebhookType.TransactionFailed ||
            task.NewStatus is TransactionStatus.Completed && configuration.Type is WebhookType.TransactionCompleted)
        {
            _webhookSender.Send(configuration.Url, configuration.Secret, new TransactionUpdatedWebhook()
            {
                TransactionId = task.TransactionTrackingId,
                Action = configuration.Type,
            });
        }

        return ValueTask.CompletedTask;
    }
}