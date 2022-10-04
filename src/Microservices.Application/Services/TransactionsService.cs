using Microservices.Application.Contracts;
using Microservices.Application.Mapping;
using Microservices.Application.Models;
using Microservices.Banking.Core.Entities;
using Microservices.Banking.Core.Enums;
using Microservices.Banking.Core.Exceptions;
using Microservices.Banking.DataAccess.Contracts;

namespace Microservices.Application.Services;

public sealed class TransactionsService : ITransactionsService
{
    private readonly IMapper _mapper;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ISimulationTasksService _simulationTasksService;
    private readonly ISimulationTaskRepository _simulationTaskRepository;

    public TransactionsService(
        IMapper mapper,
        ITransactionRepository transactionRepository,
        ISimulationTasksService simulationTasksService,
        ISimulationTaskRepository simulationTaskRepository)
    {
        _mapper = mapper;
        _transactionRepository = transactionRepository;
        _simulationTasksService = simulationTasksService;
        _simulationTaskRepository = simulationTaskRepository;
    }

    public async Task<TransactionResponse> InitiateFortressDeposit(InitiateTransactionRequest request)
    {
        var transaction = new Transaction()
        {
            Status = TransactionStatus.Created,
            Type = TransactionType.FortressDeposit,
            CardFrom = request.CardFrom,
            UsdAmount = request.UsdAmount,
            FailReason = null,
            TrackingId = Guid.NewGuid(),
        };

        await _transactionRepository.Insert(transaction);

        var simulationTask = _simulationTasksService.GenerateInitial(transaction);
        await _simulationTaskRepository.Insert(simulationTask);

        return _mapper.Map(transaction);
    }

    public async Task<TransactionResponse> Get(Guid trackingId)
    {
        var transaction = await _transactionRepository.GetFirst(entity => entity.TrackingId == trackingId);

        if (transaction is null)
        {
            throw new ItemNotFoundException("Transaction is not found.");
        }
        
        return _mapper.Map(transaction);
    }

    public async Task<IReadOnlyCollection<TransactionResponse>> GetAll()
    {
        var transactions = await _transactionRepository.GetAll();
        return _mapper.MapCollection(transactions, _mapper.Map);
    }
}