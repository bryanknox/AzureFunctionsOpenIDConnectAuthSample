using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;

namespace TestFixtures.AzureFunctions
{
    /// <summary>
    /// A test fixture useful for testing Azure Functions with output binding to
    /// an Azure Service Bus Queue or Topic.
    /// binding in 
    /// </summary>
    /// <typeparam name="T">
    /// The type of objects written to the queue or topic.
    /// </typeparam>
    public class AsyncCollectorFixture<T> : IAsyncCollector<T>
    {
        public List<T> Items { get; private set; } = new List<T>();

        public Task AddAsync(T item, CancellationToken cancellationToken = default(CancellationToken))
        {
            Items.Add(item);
            return Task.FromResult(true);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }
    }
}
