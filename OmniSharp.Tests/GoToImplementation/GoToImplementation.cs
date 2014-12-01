using System.Linq;
using NUnit.Framework;
using OmniSharp.GotoImplementation;
using OmniSharp.Parser;
using OmniSharp.Solution;
using OmniSharp.Tests.Rename;

namespace OmniSharp.Tests.GotoImplementation
{
    [TestFixture]
    public class GotoImplementationTests
    {
        [Test]
        public void Should_find_usages_in_same_file() {
            const string editorText =
@"
public class BaseClass {}
public class DerivedClassA : BaseClass {}
public class DerivedClassB : BaseClass {}
public class DerivedClassC : BaseClass {}
";

            var fileName = "test.cs";
            var solution = new FakeSolution();
            var project = new FakeProject();
            project.AddFile(editorText, fileName);
            solution.Projects.Add(project);

            var handler = new GotoImplementationHandler
                (solution, new BufferParser(solution), new ProjectFinder(solution));
            var request = new GotoImplementationRequest
                { Buffer   = editorText
                , Line     = 2
                , Column   = 14 // At word 'BaseClass'
                , FileName = fileName};
            var gotoImplementationResponse = handler.FindDerivedMembersAsQuickFixes(request);

            var quickFixes = gotoImplementationResponse.QuickFixes.ToArray();
            Assert.AreEqual(3, quickFixes.Length);
            quickFixes[0].Text.Trim().ShouldEqual("public class DerivedClassA : BaseClass {}");
            quickFixes[1].Text.Trim().ShouldEqual("public class DerivedClassB : BaseClass {}");
            quickFixes[2].Text.Trim().ShouldEqual("public class DerivedClassC : BaseClass {}");
        }

        [Test]
        public void Should_find_usages_in_all_files() {
            const string editorText1 =
@"
public class BaseClass {}
public class DerivedClassA : BaseClass {}
";
            var fileName1 = "base.cs";

            const string editorText2 =
@"
public class DerivedClassB : BaseClass {}
";
            var fileName2 = "derived.cs";

            var solution = new FakeSolutionBuilder()
                           .AddFile(editorText1, fileName1)
                           .AddFile(editorText2, fileName2)
                           .Build();

            var handler = new GotoImplementationHandler
                (solution, new BufferParser(solution), new ProjectFinder(solution));
            var request = new GotoImplementationRequest
                { Buffer   = editorText1
                , Line     = 2
                , Column   = 14 // At word 'BaseClass'
                , FileName = fileName1};
            var gotoImplementationResponse = handler.FindDerivedMembersAsQuickFixes(request);

            var quickFixes = gotoImplementationResponse.QuickFixes.ToArray();
            Assert.AreEqual(2, quickFixes.Length);
            quickFixes[0].Text.Trim().ShouldEqual("public class DerivedClassA : BaseClass {}");
            quickFixes[1].Text.Trim().ShouldEqual("public class DerivedClassB : BaseClass {}");
        }
      
        [Test]
        public void Should_find_usages_in_all_projects() {
            const string editorText1 =
@"
public class BaseClass {}
public class DerivedClassA : BaseClass {}
";
            var fileName1 = "base.cs";

            const string editorText2 =
@"
public class DerivedClassB : BaseClass {}
";
            var fileName2 = "derived.cs";

            var solution = new FakeSolutionBuilder()
                           .AddFile(editorText1, fileName1)
                           .AddProject()
                           .AddFile(editorText2, fileName2)
                           .Build();

            var handler = new GotoImplementationHandler
                (solution, new BufferParser(solution), new ProjectFinder(solution));
            var request = new GotoImplementationRequest
                { Buffer   = editorText1
                , Line     = 2
                , Column   = 14 // At word 'BaseClass'
                , FileName = fileName1};
            var gotoImplementationResponse = handler.FindDerivedMembersAsQuickFixes(request);

            var quickFixes = gotoImplementationResponse.QuickFixes.ToArray();
            Assert.AreEqual(2, quickFixes.Length);
            quickFixes[0].Text.Trim().ShouldEqual("public class DerivedClassA : BaseClass {}");
            quickFixes[1].Text.Trim().ShouldEqual("public class DerivedClassB : BaseClass {}");
        }
    }
}
