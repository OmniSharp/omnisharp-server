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
        public void Should_contain_one_project()
        {
            _solution.Projects.Count.ShouldEqual (1);
            _solution.Projects [0].Title.ShouldEqual ("minimal");
        }

        [Test]
        public void Should_put_unknown_file_into_orphan_project()
        {
            _solution.ProjectContainingFile ("test.cs").Title.ShouldEqual ("Orphan Project");
        }
    }
}

