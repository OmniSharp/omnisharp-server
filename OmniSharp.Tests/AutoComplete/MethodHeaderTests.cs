using System.Linq;
using NUnit.Framework;
using Should;

namespace OmniSharp.Tests.AutoComplete
{
    [TestFixture]
    public class MethodHeaderTests : CompletionTestBase
    {
        [Test]
        public void Should_return_method_header()
        {
            {
                MethodHeaderFor(
                    @"public class A {
    public A() 
    {
        int n;
        n.T$;
    }
}").First().ShouldStartWith("ToString(");
            }
        }
    }
}