using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictive
{
    enum SocketCount
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6
    }

    [Flags]
    enum LinkConfiguration
    {
        None = 0,
        OneTwo = 1,
        TwoThree = 2,
        ThreeFour = 4,
        FourFive = 8,
        FiveSix = 16
    }
}
