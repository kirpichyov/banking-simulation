using FluentValidation;
using Microservices.Application.Contracts;
using Microservices.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.Banking.Host.Controllers;

public sealed class TransactionsController : ApiControllerBase
{
    private readonly IValidator<InitiateTransactionRequest> _transactionRequestValidator;
    private readonly ITransactionsService _transactionsService;

    public TransactionsController(
        IValidator<InitiateTransactionRequest> transactionRequestValidator,
        ITransactionsService transactionsService)
    {
        _transactionRequestValidator = transactionRequestValidator;
        _transactionsService = transactionsService;
    }

    [HttpPost("fortress/initiate-deposit")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> InitiateFortressDeposit([FromBody] InitiateTransactionRequest request)
    {
        _transactionRequestValidator.ValidateAndThrow(request);

        var result = await _transactionsService.InitiateFortressDeposit(request);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var response = await _transactionsService.Get(id);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(TransactionResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _transactionsService.GetAll();
        return Ok(response);
    }
}