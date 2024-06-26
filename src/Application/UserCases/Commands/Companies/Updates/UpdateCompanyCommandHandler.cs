using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Company.Updates;
using Domain.Abstractions.Exceptions;
using Domain.Exceptions.Companies;
using FluentValidation;

namespace Application.UserCases.Commands.Companies.Updates;

public sealed class UpdateCompanyCommandHandler(
    ICompanyRepository _companyRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateCompanyRequest> _validator) : ICommandHandler<UpdateCompanyCommand>
{
    public async Task<Result.Success> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var updateCompanyRequest = request.UpdateCompanyRequest;
        var validationResult = await _validator.ValidateAsync(updateCompanyRequest);
        if (validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var company = await _companyRepository.GetByIdAsync(updateCompanyRequest.Id)
            ?? throw new CompanyNotFoundException(updateCompanyRequest.Id);

        company.Update(updateCompanyRequest);
        _companyRepository.Update(company);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Update();
    }
}

