using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NumSharp.Backends
{
    public class SimdEngine : DefaultEngine
    {
        public override NDArray Add(NDArray x, NDArray y)
        {
            return base.Add(x, y);
        }

        public override NDArray Dot(NDArray x, NDArray y)
        {
            return base.Dot(x, y);
        }
    }
}
