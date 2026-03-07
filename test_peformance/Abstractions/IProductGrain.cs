// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using test_peformance.Domain.Entities;

namespace test_peformance.Abstractions;

public interface IProductGrain : IGrainWithStringKey
{
    Task<(bool IsAvailable, ProductState? ProductDetails)> TryTakeProductAsync(int quantity);

    Task ReturnProductAsync(int quantity);

    Task<int> GetProductAvailabilityAsync();

    Task CreateOrUpdateProductAsync(ProductState productDetails);

    Task<ProductState> GetProductDetailsAsync();
    Task<ProductState> ReturnStateAsync();
}
