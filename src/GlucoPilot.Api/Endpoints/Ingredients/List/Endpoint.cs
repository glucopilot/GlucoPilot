﻿using FluentValidation;
using GlucoPilot.Api.Endpoints.Ingredients.GetIngredients;
using GlucoPilot.Data.Entities;
using GlucoPilot.Data.Repository;
using GlucoPilot.Identity.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GlucoPilot.Api.Endpoints.Ingredients.List;

internal static class Endpoint
{
    internal static async Task<Results<Ok<ListIngredientsResponse>, UnauthorizedHttpResult, ValidationProblem>> HandleAsync(
        [AsParameters] ListIngredientsRequest request,
        [FromServices] IValidator<ListIngredientsRequest> validator,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IRepository<Ingredient> repository,
        CancellationToken cancellationToken)
    {
        if (await validator.ValidateAsync(request).ConfigureAwait(false) is
            { IsValid: false } validation)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var userId = currentUser.GetUserId();

        var ingredients = repository.Find(i => i.UserId == userId, new FindOptions { IsAsNoTracking = true })
            .OrderByDescending(i => i.Created)
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new GetIngredientResponse
            {
                Id = i.Id,
                Created = i.Created,
                Name = i.Name,
                Carbs = i.Carbs,
                Protein = i.Protein,
                Fat = i.Fat,
                Calories = i.Calories,
                Uom = i.Uom,
                Updated = i.Updated
            })
            .ToList();

        var totalMeals = await repository.CountAsync(i => i.UserId == userId, cancellationToken).ConfigureAwait(false);
        var response = new ListIngredientsResponse
        {
            NumberOfPages = (int)Math.Ceiling(totalMeals / (double)request.PageSize),
            Ingredients = ingredients,
        };

        return TypedResults.Ok(response);
    }
}
