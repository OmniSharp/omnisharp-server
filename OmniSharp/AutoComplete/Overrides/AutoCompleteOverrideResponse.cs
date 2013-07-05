using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.AutoComplete.Overrides;

namespace OmniSharp.AutoComplete.Overrides {

    public class AutoCompleteOverrideResponse {

        public AutoCompleteOverrideResponse() {}

        public AutoCompleteOverrideResponse(IMember m) {
            if (m == null)
                throw new ArgumentNullException("m");

            this._entityType = m.EntityType;

            // TODO this seems like a dirty hack
            this.OverrideTargetName = m.ToString();
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
