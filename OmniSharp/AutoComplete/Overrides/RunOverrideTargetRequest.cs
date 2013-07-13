using System;
using OmniSharp.Common;

namespace OmniSharp.AutoComplete.Overrides {

    /// <remarks>
    ///   This class uses AutoCompleteRequest.WordToComplete as the
    ///   name of the inherited member to override. This is a hack.
    /// </remarks>
    public class RunOverrideTargetRequest : AutoCompleteRequest {}
}
