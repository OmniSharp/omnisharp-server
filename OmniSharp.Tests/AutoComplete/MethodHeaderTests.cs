using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;

namespace OmniSharp.Tests.AutoComplete
{
    public class MyClass<TDefault>
    {
        public MyClass()
        {
            
        }
    }

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

        [Test]
        public void Should_add_generic_type_argument()
        {
            MethodHeaderFor(
                @"using System.Collections.Generic;
            public class Class1 {
                public Class1()
                {
                    var l = new Lis$
                }
            }")
                .ShouldContain("List<T>()");
        }

        [Test]
        public void Should_add_angle_bracket_to_generic_completion()
        {
            MethodHeaderFor(
                @"using System.Collections.Generic;
                public class Class1 {
                    public Class1()
                    {
                        var l = new Diction$
                    }
                }")
                .ShouldContain("Dictionary<TKey, TValue>()");
        }
    }
}