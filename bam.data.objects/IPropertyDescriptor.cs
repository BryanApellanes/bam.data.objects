using System.Reflection;
using Bam.Data.Objects;

namespace bam.data.objects;

public interface IPropertyDescriptor
{
    ITypeDescriptor TypeDescriptor { get; set; }
    PropertyInfo PropertyInfo { get; set; }
    string PropertyName { get; set; }
}