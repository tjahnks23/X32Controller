using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Behringer.X32;
using NAudio.Midi;
using Objects;
using static Objects.Enums;

namespace Core
{
    public class MidiService
    {
        #region Private Fields
        private List<MidiDeviceDto> _midiDeviceDtos;
        private MidiOut _apcOut;
        private X32Console _x32;
        private X32ConsoleService _x32Service;
        private List<ApcButton> _apcButtons;
        private ApcButton _currentButton;
        private int _currentKnobVal = 63;
        #endregion

        #region Constructors
        public MidiService() { }

        public MidiService(List<MidiDeviceDto> midiDevices, X32Console x32)
        {
            _midiDeviceDtos = midiDevices;
            _x32 = x32;
            _x32Service = new X32ConsoleService(_x32);
            _apcOut = midiDevices.SingleOrDefault().LocalMidiOut;
            _apcButtons = midiDevices.SingleOrDefault().ApcButtons;
        }
        #endregion

        #region Device Setup
        public Dictionary<string, int> GetMidiInDevices()
        {
            Dictionary<string, int> midiInDevices = new Dictionary<string, int>();
            for (int device = 0; device < MidiIn.NumberOfDevices; device++)
            {
                if (!midiInDevices.ContainsKey(MidiIn.DeviceInfo(device).ProductName))
                {
                    midiInDevices.Add(MidiIn.DeviceInfo(device).ProductName, device);
                }
                else
                {
                    midiInDevices.Add($"{MidiIn.DeviceInfo(device).ProductName} (2)", device);
                }
            }
            return midiInDevices;
        }

        public Dictionary<string, int> GetMidiOutDevices()
        {
            Dictionary<string, int> midiOutDevices = new Dictionary<string, int>();
            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                if (!midiOutDevices.ContainsKey(MidiOut.DeviceInfo(device).ProductName))
                {
                    midiOutDevices.Add(MidiOut.DeviceInfo(device).ProductName, device);
                }
                else
                {
                    midiOutDevices.Add($"{MidiOut.DeviceInfo(device).ProductName} (2)", device);
                }
            }
            return midiOutDevices;
        }

        public MidiIn SetMidiInDevice(Dictionary<string, int> lookup, string key)
        {
            if (lookup.ContainsKey(key))
                return new MidiIn(lookup[key]);
            else
            {
                EventLog.WriteEntry(Constants.EVENT_SOURCE, $"{key} device (IN) not found", EventLogEntryType.Error);
                MessageBox.Show($"{key} device (IN) not found");
                return null;
            }
        }

        public MidiOut SetMidiOutDevice(Dictionary<string, int> lookup, string key)
        {
            if (lookup.ContainsKey(key))
                return new MidiOut(lookup[key]);
            else
            {
                EventLog.WriteEntry(Constants.EVENT_SOURCE, $"{key} device (OUT) not found", EventLogEntryType.Error);
                MessageBox.Show($"{key} device (OUT) not found");
                return null;
            }
        }

        public void ActivateButtons(List<MidiDeviceDto> midiDevices)
        {
            foreach (var device in midiDevices)
            {
                foreach (var btn in device.ApcButtons)
                {
                    if (ButtonIsValid(btn))
                    {
                        var buttonIdMapping = Constants.ButtonIdMappings[btn.Id];
                        int ledValue;
                        if (btn.Id == 41 || btn.Id == 49) // Only one channel range button and channel select button activated at a time
                            ledValue = 1;
                        else if (btn.FlashMode == FlashMode.Enabled) // Maybe could use flash mode for tap tempo
                            ledValue = 5;
                        else
                            ledValue = 3; // Mute buttons ?

                        var noteMessage = new NoteOnEvent(0, buttonIdMapping.Channel, buttonIdMapping.Note, ledValue, 1);

                        device.LocalMidiOut.Send(noteMessage.GetAsShortMessage());
                    }
                }
            }
        }
        #endregion

        #region Event Handlers
        public void HandleMidiInEvent(MidiEvent midiEvent)
        {
            switch (midiEvent.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                    ProcessNoteOnEvent(new MidiEventDto(midiEvent));
                    break;
                case MidiCommandCode.NoteOff:
                    ProcessNoteOffEvent(new MidiEventDto(midiEvent));
                    break;
                case MidiCommandCode.ControlChange:
                    ProcessControlChange(new MidiEventDto(midiEvent));
                    break;
                default:
                    break;
            }
        }

        private void ProcessNoteOnEvent(MidiEventDto midiEventDto)
        {
            if (Constants.NoteMappings.ContainsKey(midiEventDto.NoteLetter) || Constants.NoteMappings.ContainsKey($"{midiEventDto.NoteLetter}|{midiEventDto.Channel}"))
            {
                _currentButton = _apcButtons.SingleOrDefault(x => x.Channel == midiEventDto.Channel && x.Note == midiEventDto.NoteNumber);

                if (ButtonIsValid(_currentButton))
                {
                    switch (_currentButton.ButtonType)
                    {
                        case ButtonType.ChannelRange:
                            HandleChannelRangeChange(new X32EventDto(midiEventDto));
                            break;
                        case ButtonType.ChannelSelect:
                            //_x32Service.HandleChannelSelectEvent(new X32EventDto(midiEventDto)); I don't think I need this - only need as LED feedback
                            break;
                        case ButtonType.ChannelMute:
                             _x32Service.SendChannelMute(new X32EventDto(midiEventDto));
                            break;
                        default:
                            break;
                    }   
                }
            }
        }

        private void ProcessNoteOffEvent(MidiEventDto midiEventDto)
        {
            Dictionary<int, ToggleGroupDto> toggleGroupLookup = new Dictionary<int, ToggleGroupDto>();
            List<ApcButton> buttons = new List<ApcButton>();

            // Step 1: set appropriate scope variables for device
            var device = _midiDeviceDtos.SingleOrDefault();
            toggleGroupLookup = device.ToggleGroupLookup;
            buttons = _apcButtons;
            _apcOut = device.LocalMidiOut;

            // Step 2: determine if current button is enabled and contained in note mappings
            if (ButtonIsValid(_currentButton) && Constants.NoteMappings.ContainsKey(midiEventDto.NoteLetter))
            {
                if (_currentButton.FlashMode != FlashMode.Enabled) // Do not allow LED toggle if button has flash mode enabled or is the reset button
                {
                    // Step 4: determine toggle group and appropriate activation
                    var group = toggleGroupLookup[_currentButton.Id].Group;
                    var groupButtons = buttons.Where(x => x.Group == group);

                    if (groupButtons.Any(x => x.Active))
                    {
                        // There should only be one button active in group
                        var activeButtonInGroup = groupButtons.SingleOrDefault(x => x.Active);

                        if (activeButtonInGroup != null && activeButtonInGroup.Id == _currentButton.Id)
                        {
                            // There is a button in the group that is active and it is equal to the current button pressed, deactivate it 
                            NoteEvent greenLed = new NoteEvent(0, activeButtonInGroup.Channel, MidiCommandCode.NoteOn, activeButtonInGroup.Note, 1);
                            _apcOut.Send(greenLed.GetAsShortMessage());

                            _currentButton.Active = false;
                        }
                        else if (activeButtonInGroup != null && activeButtonInGroup.Id != _currentButton.Id)
                        {
                            // There is a button in the group that is active and it is NOT equal to the current button pressed, deactivate it
                            NoteEvent greenLed = new NoteEvent(0, activeButtonInGroup.Channel, MidiCommandCode.NoteOn, activeButtonInGroup.Note, 1);
                            _apcOut.Send(greenLed.GetAsShortMessage());

                            activeButtonInGroup.Active = false;

                            // Activate current button
                            NoteEvent redLed = new NoteEvent(0, _currentButton.Channel, MidiCommandCode.NoteOn, _currentButton.Note, 3);
                            _apcOut.Send(redLed.GetAsShortMessage());

                            _currentButton.Active = true;
                        }
                    }
                    else
                    {
                        // No active button in group, activate current button
                        NoteEvent redLed = new NoteEvent(0, _currentButton.Channel, MidiCommandCode.NoteOn, _currentButton.Note, 3);
                        _apcOut.Send(redLed.GetAsShortMessage());

                        _currentButton.Active = true;
                    }
                }
            }
            //else { /* button disabled or not in note mapping lookup, swallow MIDI message */ }
        }

        private void ProcessControlChange(MidiEventDto midiEventDto)
        {
            // Example1: 0 ControlChange Ch: 1 Controller 14 Value 10
            // Example2: 0 ControlChange Ch: 8 Controller MainVolume Value 2

            // Note: MidiCommand enum in NAudio.Midi does not contain controller number 14, 
            //       Using only MidiController.BreathController OR MidiController.Modulation as 'controller type' per device

            if (midiEventDto.EventName.Contains(Constants.ControlChanges.CC47))
            {
                ProcessKnobControlChange(midiEventDto);
            }
            else if (midiEventDto.EventName.Contains(Constants.ControlChanges.MV)) // Channel fade
            {
                _x32Service.SendChannelFade(new X32EventDto(midiEventDto));
            }
            else if (midiEventDto.EventName.Contains(Constants.ControlChanges.CC14)) // Mains fade
            {
                _x32Service.SendMainFade(new X32EventDto(midiEventDto));
            }
        }

        private void ProcessKnobControlChange(MidiEventDto midiEventDto)
        {
            // Using MidiController.MainVolume and MidiController.Expression to differentiate values being sent out per devices
            // Knob turned to the right: value will be < 63 but never increments on its own
            // Knob turned to the left: value will be > 63 but never increments on its own

            if (midiEventDto.ControlChangeValue <= 63)
                _currentKnobVal += 1;
            if (midiEventDto.ControlChangeValue > 63)
                _currentKnobVal -= 1;

            if (_currentKnobVal > 127)
                _currentKnobVal = 1;
            if (_currentKnobVal < 1)
                _currentKnobVal = 127;

            _x32Service.SendChannelPreampGain(new X32EventDto(midiEventDto));
        }

        private void HandleChannelRangeChange(X32EventDto x32Event)
        {
            // TODO: refresh mute buttons per channel 
            if (x32Event.ChannelRangeSelection == RangeSelect.Ch1_8)
            {
                // loop through btn Ids channel + 56, set appropriate mute values
                foreach (var btn in _apcButtons.Where(x => x.ButtonType == ButtonType.ChannelMute && x.Id >= 57 && x.Id <= 64))
                {
                    var led = btn.Muted.HasValue && btn.Muted.Value.Equals(true) ? 3 : 0;
                    var noteEvent = new NoteEvent(0, _currentButton.Channel, MidiCommandCode.NoteOn, _currentButton.Note, led);
                    _apcOut.Send(noteEvent.GetAsShortMessage());
                }
            }
            else if (x32Event.ChannelRangeSelection == RangeSelect.Ch9_16)
            {
                // loop through btn Ids channel + 191, set appropriate mute values
                foreach (var btn in _apcButtons.Where(x => x.ButtonType == ButtonType.ChannelMute && x.Id >= 200 && x.Id <= 207))
                {
                    var led = btn.Muted.HasValue && btn.Muted.Value.Equals(true) ? 3 : 0;
                    var noteEvent = new NoteEvent(0, _currentButton.Channel, MidiCommandCode.NoteOn, _currentButton.Note, led);
                    _apcOut.Send(noteEvent.GetAsShortMessage());
                }
            }
        }

        private bool ButtonIsValid(ApcButton button)
        {
            return button != null && button.Enabled.HasValue && button.Enabled.Value.Equals(true);
        }
        #endregion

        #region Device Clean Up
        public void Dispose()
        {
            try
            {
                foreach (var device in _midiDeviceDtos)
                {
                    if (device.MidiIn != null)
                        device.MidiIn.Dispose();
                    if (device.LocalMidiOut != null)
                        device.LocalMidiOut.Dispose();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(Constants.EVENT_SOURCE, ex.Message, EventLogEntryType.Error);
                MessageBox.Show("An error ovvurred on device disposal. See event log for details.");
            }
        }

        public void ResetAll()
        {
            foreach (var device in _midiDeviceDtos)
            {
                foreach (var apcBtn in device.ApcButtons)
                {
                    if (apcBtn.Enabled.HasValue && apcBtn.Enabled.Value.Equals(true))
                    {
                        apcBtn.Active = false;
                    }
                }
            }
            ActivateButtons(_midiDeviceDtos);
        }
        #endregion
    }
}
