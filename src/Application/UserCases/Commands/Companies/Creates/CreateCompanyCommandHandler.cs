using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Company.Create;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.Companies.Creates;
public class CreateCompanyCommandHandler
    (ICompanyRepository _companyRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateCompanyRequest> _validator) : ICommandHandler<CreateCompanyCommand>
{
    public async Task<Result.Success> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateCompanyRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var createCompanyRequest = request.CreateCompanyRequest;
        var company = Company.Create(createCompanyRequest);
        _companyRepository.Add(company);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
}
