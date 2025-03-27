﻿using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;

public class ListCartsRequest
{    
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? OrderBy { get; set; }
    public Dictionary<string, string[]> Filters { get; set; } = new Dictionary<string, string[]>();

}

