﻿using FluentValidation;
using GlucoPilot.Api.Models;
using Microsoft.Extensions.Options;

namespace GlucoPilot.Api.Endpoints.Meals.List;

public sealed record ListMealsRequest : PagedRequest
{
    public sealed class ListMealsValidator : AbstractValidator<ListMealsRequest>
    {
        public ListMealsValidator(IOptions<ApiSettings> apiSettings)
        {
            Include(new PagedRequestValidator(apiSettings));
        }
    }
}
