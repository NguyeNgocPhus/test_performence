// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Orleans.Runtime;
using test_peformance.Abstractions;
using test_peformance.Entities;

namespace test_peformance.Grains;

internal class ProductGrain(
    [PersistentState(
            stateName: "Product",
            storageName: "shopping-cart")]
        IPersistentState<ProductDetails> product) : Grain, IProductGrain
{
    private readonly IPersistentState<ProductDetails> _product = product;

    Task<int> IProductGrain.GetProductAvailabilityAsync() =>
        Task.FromResult(_product.State.Quantity);

    Task<ProductDetails> IProductGrain.GetProductDetailsAsync() =>
        Task.FromResult(_product.State);

    Task IProductGrain.ReturnProductAsync(int quantity) =>
        UpdateStateAsync(_product.State with
        {
            Quantity = _product.State.Quantity + quantity
        });

    async Task<(bool IsAvailable, ProductDetails? ProductDetails)> IProductGrain.TryTakeProductAsync(int quantity)
    {
        if (_product.State.Quantity < quantity)
        {
            return (false, null);
        }

        var updatedState = _product.State with
        {
            Quantity = _product.State.Quantity - quantity
        };

        await UpdateStateAsync(updatedState);

        return (true, _product.State);
    }

    Task IProductGrain.CreateOrUpdateProductAsync(ProductDetails productDetails) =>
        UpdateStateAsync(productDetails);

    private async Task UpdateStateAsync(ProductDetails product)
    {
        _product.State = product;
        await _product.WriteStateAsync();
    }
    public async Task<ProductDetails> ReturnStateAsync()
    {
        await Task.CompletedTask;
        return _product.State;
    }
}
