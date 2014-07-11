using NUnit.Framework;
using OmniSharp.Solution;
using Should;
using System;

namespace OmniSharp.Tests
{
    [TestFixture]
    public class SolutionTest
    {
        readonly CSharpSolution _solution;

        public SolutionTest ()
        {
            _solution = new CSharpSolution (new Logger (Verbosity.Verbose));
            _solution.LoadSolution (Environment.CurrentDirectory + "/Solution/minimal/minimal.sln");
        }

        [Test]
        public void Should_contain_one_orphan_project_and_one_real()
        {
            _solution.Projects.Count.ShouldEqual (2);
            _solution.Projects [1].Title.ShouldEqual ("minimal");
        }

        [Test]
        public void Should_put_unknown_file_into_orphan_project()
        {
            _solution.ProjectContainingFile ("test.cs").Title.ShouldEqual ("Orphan Project");
        }

        [Test]
        public void Should_put_unknown_file_near_to_close_project_file()
        {
            _solution.ProjectContainingFile ((Environment.CurrentDirectory + "/Solution/minimal/minimal/test.cs").LowerCaseDriveLetter())
                .Title.ShouldEqual ("minimal");
        }
    }
}

