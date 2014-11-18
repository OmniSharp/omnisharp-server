// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Microsoft.Framework.Runtime.Roslyn
{
    public class CompilationSettings
    {
        public int LanguageVersion { get; set; }
        public IEnumerable<string> Defines { get; set; }
        public JObject CompilationOptions { get; set; }
    }
}
