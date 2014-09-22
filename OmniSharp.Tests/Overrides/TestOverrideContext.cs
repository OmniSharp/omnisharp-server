using System.Linq;
using NUnit.Framework;
using OmniSharp.Common;
using OmniSharp.Overrides;
using OmniSharp.Parser;
using OmniSharp.Tests.Rename;

namespace OmniSharp.Tests.Overrides
{
    [TestFixture]
    public class TestOverrideContext
    {
        [Test]
        public void Should_not_offer_already_overridden_method()
        {
            var buffer = @"
public class WeirdString : String
{
    public override string ToString()
    {
        return ""weird"";
    }
$
}";
            var location = TestHelpers.GetLineAndColumnFromDollar(buffer);
            var request = new Request { Buffer = buffer, FileName = "myfile.cs", Line = location.Line, Column = location.Column };
            var solution = new FakeSolutionBuilder().AddFile(buffer, "myfile.cs").Build();
            var parser = new BufferParser(solution);
           
            var context = new OverrideContext(request, parser);
            var overrides = context.OverrideTargets.Select(m => m.OverrideTargetName).ToArray();
            overrides.ShouldNotContain("public virtual string ToString ();");

        }
    }
}
