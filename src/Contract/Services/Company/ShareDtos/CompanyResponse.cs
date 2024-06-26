﻿using Contract.Services.Company.Shared;

namespace Contract.Services.Company.ShareDtos;

public record CompanyResponse
(
    Guid Id,
    string Name,
    string Address,
    string DirectorName,
    string DirectorPhone,
    string Email,
    CompanyType CompanyType,
    string CompanyTypeDescription
    );
