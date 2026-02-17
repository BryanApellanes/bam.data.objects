namespace Bam.Data.Objects;

/// <summary>
/// Provides extension methods for converting objects to property collections and performing safe casts.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Extracts the properties of the specified object as a collection of <see cref="IProperty"/> instances.
    /// </summary>
    /// <param name="data">The object to extract properties from. Must not be null.</param>
    /// <returns>An enumerable of properties from the object.</returns>
    public static IEnumerable<IProperty> ToObjectProperties(this object data)
    {
        Args.ThrowIfNull(data, nameof(data));
        if (data is ObjectData objectData)
        {
            return objectData.Properties;
        }

        if (data is IObjectData iObjectData)
        {
            return iObjectData.Properties;
        }
        return new ObjectData(data).Properties;
    }

    /// <summary>
    /// Attempts to cast the specified object to type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to cast to.</typeparam>
    /// <param name="data">The object to cast.</param>
    /// <param name="obj">The cast result, or default if the cast fails.</param>
    /// <returns>True if the cast succeeded; otherwise, false.</returns>
    public static bool TryCast<T>(this object data, out T obj)
    {
        try
        {
            obj = (T)data;
            return true;
        }
        catch
        {
            obj = default!;
            return false;
        }
    }
}