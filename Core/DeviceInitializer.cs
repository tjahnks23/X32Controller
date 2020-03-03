using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Behringer.X32;
using Objects;
using OSC;
using Sanford.Multimedia.Midi;
using static Objects.Enums;

namespace Core
{
    public class DeviceInitializer
    {
        private X32Console _x32;
        private List<ApcButton> _buttons = new List<ApcButton>();

        public List<ApcButton> Init(int deviceIndex, Dictionary<int, ToggleGroupDto> toggleGroupLookup, X32Console x32)
        {
            _x32 = x32;

            try
            {
                // APC20 SysEx message for 'note mode'
                byte[] sysExMsg = { 0xf0, 0x47, 0x7f, 0x7b, 0x60, 0x00, 0x04, 0x41, 0x08, 0x02, 0x01, 0xf7 };
                SysExMessage message = new SysExMessage(sysExMsg);

                OutputDevice localOut = new OutputDevice(deviceIndex);
                localOut.Send(message);
                localOut.Dispose();

                int buttonId = 41;
                for (int channel = 1; channel <= 8; channel++)
                {
                    _buttons.Add(new ApcButton
                    {
                        Id = buttonId,
                        Group = toggleGroupLookup.ContainsKey(buttonId) ? toggleGroupLookup[buttonId].Group : -1,
                        Channel = Constants.ButtonIdMappings[buttonId].Channel,
                        Note = Constants.ButtonIdMappings[buttonId].Note,
                        Active = false,
                        Enabled = toggleGroupLookup.ContainsKey(buttonId),
                        FlashMode = toggleGroupLookup.ContainsKey(buttonId) ? toggleGroupLookup[buttonId].FlashMode : FlashMode.Unknown,
                        ButtonType = buttonId >= 41 && buttonId <= 48 ? ButtonType.ChannelRange : buttonId >= 49 && buttonId <= 56 ? ButtonType.ChannelSelect : ButtonType.ChannelMute
                    });
                    buttonId++;
                }

                SetX32Parameters();

                // Setting X32 params uses OSC protocol and event listeners, need to wait to receive all messages via network before continuing
                Thread.Sleep(4000);

                return _buttons;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(Constants.EVENT_SOURCE, ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

        private void SetX32Parameters()
        {
            // Get the current mute state per channel to set red LED buttons appropriately on APC
            for (int channel = 0; channel <= 15; channel++)
            {
                _x32.ControlRequest(_x32.Channel[channel].Strip.Mute);
                _x32.OnChannelMute += OnChannelMuteReceived;
            }
            _x32.OnChannelMute -= OnChannelMuteReceived;
        }

        private void OnChannelMuteReceived(object sender, OSCPacket e)
        {
            int buttonId;
            int channel = int.Parse(e.Nodes[1].ToString());

            if (channel <= 8)
                buttonId = channel + 56;
            else
                buttonId = channel + 191;

            bool muted = e.Arguments[0].ToString() == "0" ? true : false;

            var muteButton = _buttons.SingleOrDefault(x => x.Id == buttonId);
            muteButton.Muted = muted;
        }
    }
}
