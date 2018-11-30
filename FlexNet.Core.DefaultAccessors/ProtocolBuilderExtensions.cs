using System;
using System.Collections.Generic;
using System.Text;

namespace FlexNet.Core.DefaultAccessors
{
    public static class ProtocolBuilderExtensions
    {
        public static ProtocolBuilder RegisterDefaultAccessors(this ProtocolBuilder builder)
            => builder.RegisterAccessor<Byte, ByteAccessor>()
               .RegisterAccessor<Int32, Int32Accessor>()
            ;
    }
}
