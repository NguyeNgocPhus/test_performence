// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Orleans.Runtime;
using test_peformance.Abstractions;
using test_peformance.Domain.Entities;

namespace test_peformance.Grains;

internal class ProductGrain(
    [PersistentState(
            stateName: "Product",
            storageName: "shopping-cart")]
        IPersistentState<ProductGrainState> product) : Grain, IProductGrain
{
    private readonly IPersistentState<ProductGrainState> _product = product;

    private static ProductState ToProductState(ProductGrainState s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Description = s.Description,
        Quantity = s.Quantity,
        UnitPrice = s.UnitPrice,
        DetailsUrl = s.DetailsUrl,
        ImageUrl = s.ImageUrl,
    };

    private static ProductGrainState FromProductState(ProductState s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Description = s.Description,
        Quantity = s.Quantity,
        UnitPrice = s.UnitPrice,
        DetailsUrl = s.DetailsUrl,
        ImageUrl = s.ImageUrl,
    };

    Task<int> IProductGrain.GetProductAvailabilityAsync() =>
        Task.FromResult(_product.State.Quantity);

    Task<ProductState> IProductGrain.GetProductDetailsAsync() =>
        Task.FromResult(ToProductState(_product.State));

    Task IProductGrain.ReturnProductAsync(int quantity) =>
        UpdateStateAsync(_product.State with { Quantity = _product.State.Quantity + quantity });

    async Task<(bool IsAvailable, ProductState? ProductDetails)> IProductGrain.TryTakeProductAsync(int quantity)
    {
        if (_product.State.Quantity < quantity)
            return (false, null);

        var updatedState = _product.State with { Quantity = _product.State.Quantity - quantity };
        await UpdateStateAsync(updatedState);
        return (true, ToProductState(_product.State));
    }

    Task IProductGrain.CreateOrUpdateProductAsync(ProductState productState) =>
        UpdateStateAsync(FromProductState(productState));

    private async Task UpdateStateAsync(ProductGrainState state)
    {
        _product.State = state;
        await _product.WriteStateAsync();
    }

    public async Task<ProductState> ReturnStateAsync()
    {
        await Task.CompletedTask;
        return ToProductState(_product.State);
    }
}
