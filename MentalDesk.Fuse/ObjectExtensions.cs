using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MentalDesk.Fuse;

/// <summary>
/// Exposes extension methods that allow you to add dynamic properties to objects in .NET similar
/// to how this is done in Javascript (as an associative dictionary).
/// </summary>
public static class ObjectExtensions
{
    private static ConditionalWeakTable<object, Dictionary<string, object?>> Map { get; } = new();

    private static Dictionary<string, object?> AssociatedProperties(object source) =>
        Map.GetValue(source, _ => new Dictionary<string, object?>());

    private static bool TryGetTypedValue<T>(this IDictionary<string, object?> source, string key,
        [NotNullWhen(true)] out T value)
    {
        if (source.TryGetValue(key, out var obj) && obj is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default!;
        return false;
    }
    
    /// <summary>
    /// Sets a fused property
    /// </summary>
    /// <param name="source">The object being extended with a fused property</param>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="value">The value of the property</param>
    public static void SetFused(this object source, string propertyName, object? value) =>
        AssociatedProperties(source)[propertyName] = value;

    /// <summary>
    /// Retrieves a fused property
    /// </summary>
    /// <param name="source">The object with the fused property</param>
    /// <param name="propertyName">The name of the fused property</param>
    /// <typeparam name="T">The type of the fused property</typeparam>
    /// <returns>
    /// The instance of <typeparamref name="T"/> that is fused to <paramref name="source"/> or default(T) if none exists.    
    /// </returns>
    public static T? GetFused<T>(this object source, string propertyName) =>
        AssociatedProperties(source).TryGetTypedValue<T>(propertyName, out var value)
            ? value
            : default;

    /// <summary>
    /// Ensures automatic property names won't conflict with user provided names 
    /// </summary>
    private static readonly string FusedPrefix = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Provides an instance of <typeparamref name="T"/> that will remain fused to <paramref name="source"/> until
    /// <paramref name="source"/> is disposed of or garbage collected.
    /// </summary>
    /// <param name="source">The object being extended with a fused property</param>
    /// <typeparam name="T">The type of the fused property</typeparam>
    /// <returns>The instance of <typeparam name="T"></typeparam> that is fused to <paramref name="source"/></returns>
    public static T Fused<T>(this object source) where T : new()
    {
        var propertyName = $"{FusedPrefix}.{typeof(T).Name}";
        if (!AssociatedProperties(source).TryGetTypedValue<T>(propertyName, out var value))
        {
            value = new T();
            AssociatedProperties(source)[propertyName] = value;
        }
        return value;
    }
}