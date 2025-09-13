using System.Collections.Generic;
using Zenject;

/// <summary>
/// Generic object pool for List<T> instances to reduce garbage collection pressure.
/// Automatically clears lists when returned to the pool.
/// </summary>
/// <typeparam name="T">The type of elements in the list</typeparam>
public class ListMemoryPool<T> : MemoryPool<List<T>>
{
    protected override void OnSpawned(List<T> item)
    {
        // Ensure the list is ready for use
        item.Clear();
    }

    protected override void OnDespawned(List<T> item)
    {
        // Clear the list when returning to pool to prevent memory leaks
        item.Clear();
    }
}
