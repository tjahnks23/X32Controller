using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Behringer.X32;
using Objects;

namespace Core
{

    public class X32ConsoleService
    {
        X32Console _x32;
        
        public X32ConsoleService(X32Console x32)
        {
            _x32 = x32;
        }

        public void SendChannelFade(X32EventDto x32Event)
        {
            _x32.Channel[x32Event.ChannelIndex].Strip.Fader.Value = x32Event.TranslatedFloatValue;
            _x32.SendParameter(_x32.Channel[x32Event.ChannelIndex].Strip.Fader);
        }

        public void SendChannelMute(X32EventDto x32Event)
        {
            _x32.Channel[x32Event.ChannelIndex].Strip.Mute.Value = x32Event.ChannelMuted ? X32OnOff.UnMute : X32OnOff.Mute;
            _x32.SendParameter(_x32.Channel[x32Event.ChannelIndex].Strip.Mute);
        }

        public void SendChannelSelect(X32EventDto x32Event)
        {
            // do nothing, already handled in MidiService
        }

        public void SendMainFade(X32EventDto x32Event)
        {
            _x32.Main.Strip.Fader.Value = x32Event.TranslatedFloatValue;
            _x32.SendParameter(_x32.Main.Strip.Fader);
        }

        public void SendChannelPreampGain(X32EventDto x32Event)
        {
            // TODO: head amp gain?
            //_x32.Channel[x32Event.ChannelIndex]
        }

    }
}
