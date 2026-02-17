using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Application.Unit;

[UnitTestMenu("Type identifier should")]
public class TypeIdentifierShould : UnitTestMenuContainer
{
    public TypeIdentifierShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public new void GetType()
    {
        Type type = typeof(PlainTestClass);
        string typeName = type.AssemblyQualifiedName!;

        When.A<TypeDescriptor>("is created from AssemblyQualifiedName",
            () => new TypeDescriptor(typeName),
            (descriptor) => descriptor)
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .As<TypeDescriptor>("has a Type", d => d?.Type != null)
                .As<TypeDescriptor>("Type equals PlainTestClass", d => type.Equals(d?.Type))
                .As<TypeDescriptor>("Type is PlainTestClass", d => d?.Type == type);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void HaveTypeName()
    {
        When.A<TypeDescriptor>("is created from a Type",
            () => new TypeDescriptor(typeof(PlainTestClass)),
            (descriptor) => descriptor)
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .As<TypeDescriptor>("has an AssemblyQualifiedTypeName", d => d?.AssemblyQualifiedTypeName != null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
