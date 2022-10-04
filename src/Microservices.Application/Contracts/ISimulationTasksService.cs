using Microservices.Banking.Core.Entities;
using Microservices.Banking.Core.Enums;

namespace Microservices.Application.Contracts;

public interface ISimulationTasksService
{
    SimulationTask GenerateInitial(Transaction transaction);
    SimulationTask GenerateNext(Transaction transaction, TransactionStatus previousStatus);
}