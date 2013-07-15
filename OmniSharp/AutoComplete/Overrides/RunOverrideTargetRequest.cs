using System;
using OmniSharp.Common;

namespace OmniSharp.AutoComplete.Overrides {

    public class RunOverrideTargetRequest : Request {
        /// <example>
        ///   public override bool Equals(object obj);
        /// </example>
        public string OverrideTargetName {get; set;}

    }
}
