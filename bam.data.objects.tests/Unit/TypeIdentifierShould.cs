using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Test;

namespace Bam.Application.Unit;

[UnitTestMenu("Type identifier should")]
public class TypeIdentifierShould : UnitTestMenuContainer
{
    public TypeIdentifierShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void GetType()
    {
        Type type = typeof(PlainTestClass);
        string typeName = type.AssemblyQualifiedName;
        TypeDescriptor descriptor = new TypeDescriptor(typeName);
        descriptor.Type.ShouldNotBeNull("Type was null");
        descriptor.Type.ShouldEqual(type);
        descriptor.Type.ShouldBe(type);
    }

    [UnitTest]
    public void HaveTypeName()
    {
        TypeDescriptor descriptor = new TypeDescriptor(typeof(PlainTestClass));
        descriptor.AssemblyQualifiedTypeName.ShouldNotBeNull("AssemblyQualifiedTypeName was null");
    }
}