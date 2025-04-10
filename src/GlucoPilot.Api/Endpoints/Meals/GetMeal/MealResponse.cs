﻿using GlucoPilot.Api.Models;
using System;
using System.Collections.Generic;

namespace GlucoPilot.Api.Endpoints.Meals.GetMeal;

public record MealResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset Created { get; set; }
    public List<MealIngredientResponse>? MealIngredients { get; set; } = new();
}
