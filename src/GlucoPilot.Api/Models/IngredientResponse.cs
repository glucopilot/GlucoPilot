﻿using GlucoPilot.Data.Enums;
using System;

namespace GlucoPilot.Api.Models
{
    public class IngredientResponse
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required int Carbs { get; set; }
        public required int Protein { get; set; }
        public required int Fat { get; set; }
        public required int Calories { get; set; }
        public required UnitOfMeasurement Uom { get; set; }
    }
}