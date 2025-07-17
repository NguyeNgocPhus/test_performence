// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Orleans.Runtime;
using test_peformance.Abstractions;
using test_peformance.Configurations;
using test_peformance.Entities;

namespace test_peformance.StartupTasks;

public sealed class SeedProductStoreTask(IGrainFactory grainFactory) : IStartupTask
{
    async Task IStartupTask.Execute(CancellationToken cancellationToken)
    {
        var faker = new ProductDetails().GetBogusFaker();

        foreach (var product in faker.GenerateLazy(50))
        {
            var productGrain = grainFactory.GetGrain<IProductGrain>(product.Id);
            await productGrain.CreateOrUpdateProductAsync(product);
        }
    }
}
