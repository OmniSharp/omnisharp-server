using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.Solution;
using OmniSharp.AutoComplete;
using unResolvedTypes = System.Collections.Generic.IEnumerable
    <ICSharpCode.NRefactory.TypeSystem.IUnresolvedTypeDefinition>;

namespace OmniSharp.AutoComplete.Overrides {
    public class AutoCompleteOverrideHandler {

        /// <summary>
        ///   TODO
        /// </summary>
        public IEnumerable<AutoCompleteOverrideResponse>
            GetOverridableTargets
            (ISolution solution, OverrideTypeKind kind) {

            var classesAndStructsInAllProjects =
                from project in solution.Projects
                from unresolvedRef in project.References.OfType<IUnresolvedAssembly>()
                from anyType in unresolvedRef.GetAllTypeDefinitions()
                    .Union(project.ProjectContent.GetAllTypeDefinitions())
                where    anyType.Kind == TypeKind.Class
                      || anyType.Kind == TypeKind.Struct
                select anyType;

            return this.GetOverridableMethods(classesAndStructsInAllProjects)
                .Concat(this.GetOverridableProperties(classesAndStructsInAllProjects))
                .Concat(this.GetOverridableEvents(classesAndStructsInAllProjects));

        }

        public IEnumerable<AutoCompleteOverrideResponse> GetOverridableMethods
            (unResolvedTypes classesAndStructsInAllProjects) {

            var overridableMethods = classesAndStructsInAllProjects
                .SelectMany(c => c.Methods)
                .Where(method => method.IsOverridable)
                .Select(m => new AutoCompleteOverrideResponse
                        ( m
                        , descriptionText : "TODO descriptionText"
                        , completionText  : "TODO completionText"));

            return overridableMethods;
        }

        public IEnumerable<AutoCompleteOverrideResponse> GetOverridableProperties
            (unResolvedTypes classesAndStructsInAllProjects) {

            var overridableProperties = classesAndStructsInAllProjects
                .SelectMany(c => c.Properties)
                .Where(property => property.IsOverridable)
                .Select(p => new AutoCompleteOverrideResponse
                        ( p
                        , descriptionText : "TODO descriptionText"
                        , completionText  : "TODO completionText"));

            return overridableProperties;
        }

        public IEnumerable<AutoCompleteOverrideResponse> GetOverridableEvents
            (unResolvedTypes classesAndStructsInAllProjects) {

            var overridableProperties = classesAndStructsInAllProjects
                .SelectMany(c => c.Events)
                .Where(@event => @event.IsOverridable)
                .Select(e => new AutoCompleteOverrideResponse
                        ( e
                        , descriptionText : "TODO descriptionText"
                        , completionText  : "TODO completionText"));

            return overridableProperties;
        }
    }
}
