﻿using GlucoPilot.Api.Models;
using System;
using System.Collections.Generic;

namespace GlucoPilot.Api.Endpoints.Meals.GetMeal;

public record GetMealResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
    public required int TotalCalories { get; set; }
    public required int TotalCarbs { get; set; }
    public required int TotalProtein { get; set; }
    public required int TotalFat { get; set; }
    public List<MealIngredientResponse>? MealIngredients { get; set; } = new();
}
