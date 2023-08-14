/*
 * MIT License
 * 
 * Copyright (c) 2023 Mental Desk Limited
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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

    private static Dictionary<string, object?> AssociatedProperties(this object source) =>
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
    /// Sets a fused property
    /// </summary>
    /// <param name="source">The object being extended with a fused property</param>
    /// <param name="value">The value of the property</param>
    public static void SetFused<T>(this object source, T value) => SetFused(source, typeof(T).Name, value);

    /// <summary>
    /// Retrieves a fused property
    /// </summary>
    /// <param name="source">The object with the fused property</param>
    /// <param name="propertyName">The name of the fused property</param>
    /// <typeparam name="T">The type of the fused property</typeparam>
    /// <returns>
    /// The instance of <typeparamref name="T"/> that is fused to <paramref name="source"/> or default(T) if none exists.    
    /// </returns>
    public static T? GetFused<T>(this object source, string? propertyName = null)
    {
        propertyName ??= typeof(T).Name;
        return source.AssociatedProperties().TryGetTypedValue<T>(propertyName, out var value)
            ? value
            : default;
    }

    /// <summary>
    /// Provides an instance of <typeparamref name="T"/> that will remain fused to <paramref name="source"/> until
    /// <paramref name="source"/> is disposed of or garbage collected.
    /// </summary>
    /// <param name="source">The object being extended with a fused property</param>
    /// <typeparam name="T">The type of the fused property</typeparam>
    /// <returns>The instance of <typeparamref name="T"></typeparamref> that is fused to <paramref name="source"/></returns>
    public static T Fused<T>(this object source) where T : new()
    {
        var propertyName = typeof(T).Name;
        if (!source.AssociatedProperties().TryGetTypedValue<T>(propertyName, out var value))
        {
            value = new T();
            source.AssociatedProperties()[propertyName] = value;
        }
        return value;
    }    
}