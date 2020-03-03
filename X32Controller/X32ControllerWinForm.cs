using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Behringer.X32;
using Core;
using NAudio.Midi;
using Objects;
using static Objects.Enums;

namespace X32Controller
{
    public partial class X32ControllerWinForm : Form
    {
        MidiService _midiService;

        public X32ControllerWinForm()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            var ipAddr = "192.168.1.100";
            X32Console x32 = new X32Console();
            x32.Connect(ipAddr);

            var midiDevice = new MidiDeviceDto();
            midiDevice.ToggleGroups = CreateStaticToggleGroups();
            var toggleGroupLookup = GetToggleGroupLookup(midiDevice.ToggleGroups);
            
            var initMidiService = new MidiService();
            Dictionary<string, int> midiInDeviceLookup = initMidiService.GetMidiInDevices();
            Dictionary<string, int> midiOutDeviceLookup = initMidiService.GetMidiOutDevices();

            if (!midiInDeviceLookup.Any(x => x.Key.Contains(Constants.DeviceNames.AKAI_APC20)) ||
                !midiOutDeviceLookup.Any(x => x.Key.Contains(Constants.DeviceNames.AKAI_APC20)))
            {
                MessageBox.Show("No APC20 found. Ensure the USB cable is connected and the device is powered on.");
                //EnableDisableControls(true);
                return;
            }

            DeviceInitializer deviceInitializer = new DeviceInitializer();
            List<MidiDeviceDto> updatedMidiDevices = new List<MidiDeviceDto>();

            midiDevice.ToggleGroupLookup = toggleGroupLookup;
            midiDevice.ApcButtons = deviceInitializer.Init(midiOutDeviceLookup[Constants.DeviceNames.AKAI_APC20], toggleGroupLookup, x32);
            midiDevice.MidiIn = initMidiService.SetMidiInDevice(midiInDeviceLookup, Constants.DeviceNames.AKAI_APC20);
            midiDevice.LocalMidiOut = initMidiService.SetMidiOutDevice(midiOutDeviceLookup, Constants.DeviceNames.AKAI_APC20);
            updatedMidiDevices.Add(midiDevice);
            
            _midiService = new MidiService(updatedMidiDevices, x32);
            _midiService.ActivateButtons(updatedMidiDevices);

            midiDevice.MidiIn.MessageReceived -= MidiIn_MessageReceived;
            midiDevice.MidiIn.MessageReceived += MidiIn_MessageReceived;
            midiDevice.MidiIn.ErrorReceived += MidiIn_ErrorReceived;
            midiDevice.MidiIn.Start();
        }

        private List<ToggleGroupDto> CreateStaticToggleGroups()
        {
            return new List<ToggleGroupDto>
            {
                new ToggleGroupDto { Group = 1, FirstButton = 47, LastButton = 55, FlashMode = FlashMode.Disabled },
                new ToggleGroupDto { Group = 2, FirstButton = 56, LastButton = 64, FlashMode = FlashMode.Disabled }
            };
        }

        private Dictionary<int, ToggleGroupDto> GetToggleGroupLookup(List<ToggleGroupDto> toggleGroups)
        {
            var lookup = new Dictionary<int, ToggleGroupDto>();

            foreach (var tg in toggleGroups)
            {
                for (int buttonId = 1; buttonId <= 40; buttonId++)
                {
                    if (buttonId >= tg.FirstButton && buttonId <= tg.LastButton)
                    {
                        lookup.Add(buttonId, new ToggleGroupDto
                        {
                            FirstButton = tg.FirstButton,
                            LastButton = tg.LastButton,
                            Group = tg.Group,
                            FlashMode = tg.FlashMode
                        });
                    }
                }
            }

            return lookup;
        }

        #region MIDI Event Handlers
        public void MidiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
        {
            EventLog.WriteEntry(Constants.EVENT_SOURCE, $"MidiIn event error for {e.MidiEvent?.GetType()?.Name}", EventLogEntryType.Error);
        }

        public void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            var midiEventDto = new MidiEventDto(e.MidiEvent);
            _midiService.HandleMidiInEvent(e.MidiEvent);
        }
        #endregion
    }
}
