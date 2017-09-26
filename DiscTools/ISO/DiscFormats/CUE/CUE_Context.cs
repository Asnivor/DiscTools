using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DiscTools.ISO.DiscFormats.CUE
{
    public class CUE_Context
    {
        /// <summary>
        /// The CueFileResolver to be used by this instance
        /// </summary>
        public CueFileResolver Resolver;

        /// <summary>
        /// The DiscMountPolicy to be applied to this context
        /// </summary>
        public DiscMountPolicy DiscMountPolicy;
    }
}
