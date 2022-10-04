using Microservices.Application.Contracts;
using Microservices.Banking.Core.Entities;
using Microservices.Banking.Core.Enums;
using Microservices.Banking.Core.Options;
using Microsoft.Extensions.Options;

namespace Microservices.Application.Services;

public sealed class SimulationTasksService : ISimulationTasksService
{
    private readonly Random _random;
    private readonly SimulationOptions _simulationOptions;

    private static string[] FailReasons =
    {
        "Insufficient funds",
        "Card is expired",
        "Network failure",
        "Limit excesses",
        "Card is blocked",
        "Bank declined the transaction",
        "Cancelled by card owner",
        "Bank external service internal error"
    };

    public SimulationTasksService(IOptions<SimulationOptions> simulationOptions)
    {
        _random = Random.Shared;
        _simulationOptions = simulationOptions.Value;
    }

    public SimulationTask GenerateInitial(Transaction transaction)
    {
        if (IsFailed())
        {
            return GetFailed(transaction.TrackingId);
        }

        return new SimulationTask()
        {
            TransactionTrackingId = transaction.TrackingId,
            NewStatus = TransactionStatus.Processing,
            CanBeSentAtUtc =  GetSendDate(),
        };
    }

    public SimulationTask GenerateNext(Transaction transaction, TransactionStatus previousStatus)
    {
        if (previousStatus is TransactionStatus.Completed or TransactionStatus.Failed)
        {
            throw new InvalidOperationException("Transaction is already marked as processed.");
        }
        
        if (IsFailed())
        {
            return GetFailed(transaction.TrackingId);
        }
        
        return new SimulationTask()
        {
            TransactionTrackingId = transaction.TrackingId,
            NewStatus = TransactionStatus.Completed,
            CanBeSentAtUtc =  GetSendDate(),
            NewFeeAmount = GetRandomFee(),
        };
    }

    private SimulationTask GetFailed(Guid transactionTrackingId)
    {
        return new SimulationTask()
        {
            TransactionTrackingId = transactionTrackingId,
            NewStatus = TransactionStatus.Failed,
            CanBeSentAtUtc = GetSendDate(),
            FailReason = FailReasons[_random.Next(0, FailReasons.Length)],
            NewFeeAmount = GetRandomFee(),
        };
    }

    private bool IsFailed() => _random.Next(0, 100) < _simulationOptions.FailureChance;
    private decimal GetRandomFee() => ((decimal)_random.Next(0, 10) / 10);

    private DateTime GetSendDate()
    {
        var delay = _random.Next(_simulationOptions.MinDelayBeforeStatusChanged, _simulationOptions.MaxDelayBeforeStatusChanged+1);
        return DateTime.UtcNow.AddSeconds(delay);
    }
}