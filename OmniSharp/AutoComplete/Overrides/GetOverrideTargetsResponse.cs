using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.AutoComplete.Overrides;

namespace OmniSharp.AutoComplete.Overrides {

    public class GetOverrideTargetsResponse {

        public GetOverrideTargetsResponse() {}

        public GetOverrideTargetsResponse
            ( IMember m
            , CSharpTypeResolveContext resolveContext) {
            if (resolveContext == null)
                throw new ArgumentNullException("overrideContext");

            if (m == null)
                throw new ArgumentNullException("m");

            this._entityType = m.EntityType;

            var builder = new TypeSystemAstBuilder
                (new CSharpResolver(resolveContext));

            this.OverrideTargetName =
                builder.ConvertEntity(m).GetText()
                // Builder automatically adds a trailing newline
                .TrimEnd(Environment.NewLine.ToCharArray());
        }

        private EntityType _entityType;
        public string OverrideType {
            get {return _entityType.ToString();}
            set {
                _entityType = (EntityType)
                    Enum.Parse(typeof(EntityType), value);
            }
        }

        /// <summary>
        ///   TODO
        /// </summary>
        public string OverrideTargetName {get; set;}

    }

}
