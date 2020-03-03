using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects
{
    public class Enums
    {
        public enum FlashMode
        {
            Enabled,
            Disabled,
            Unknown
        }

        public enum ButtonType
        {
            ChannelSelect,
            ChannelRange,
            ChannelMute
        }

        public enum RangeSelect
        {
            Ch1_8,
            Ch9_16,
            Ch17_24,
            Ch25_32,
            Aux1_8,
            Unknown
        }
    }
}
