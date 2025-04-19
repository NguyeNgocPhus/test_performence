﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using test_peformance.Entities;

namespace test_peformance.Abstractions;

public interface IProductGrain : IGrainWithStringKey
{
    Task<(bool IsAvailable, ProductDetails? ProductDetails)> TryTakeProductAsync(int quantity);

    Task ReturnProductAsync(int quantity);

    Task<int> GetProductAvailabilityAsync();

    Task CreateOrUpdateProductAsync(ProductDetails productDetails);

    Task<ProductDetails> GetProductDetailsAsync();
    Task<ProductDetails> ReturnStateAsync();

}
