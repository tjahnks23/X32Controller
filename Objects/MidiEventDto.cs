using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace Objects
{
    public class MidiEventDto
    {
        private MidiEvent _midiEvent;

        public MidiEventDto(MidiEvent midiEvent)
        {
            _midiEvent = midiEvent;
        }

        public string EventName
        {
            get
            {
                if (_midiEvent != null) return _midiEvent.ToString();
                return null;
            }
        }

        public int Channel
        {
            get
            {
                if (_midiEvent != null) return _midiEvent.Channel;
                return -1;
            }
        }

        public string NoteLetter
        {
            get
            {
                if (_midiEvent != null && !string.IsNullOrWhiteSpace(EventName))
                {
                    if (_midiEvent.CommandCode == MidiCommandCode.NoteOn)
                    {
                        return _midiEvent.ToString().Substring(15, 3).Trim();
                    }
                    else if (_midiEvent.CommandCode == MidiCommandCode.NoteOff)
                    {
                        return _midiEvent.ToString().Substring(16, 3).Trim();
                    }
                }
                return null;
            }
        }

        public int NoteNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NoteLetter))
                {
                    if (NoteLetter.StartsWith("E4") && Constants.NoteMappings.ContainsKey($"E4|{Channel}"))
                    {
                        return Constants.NoteMappings[$"E4|{Channel}"];
                    }

                    if (Constants.NoteMappings.ContainsKey(NoteLetter))
                    {
                        return Constants.NoteMappings[NoteLetter];
                    }
                }
                return -1;
            }
        }

        private int _ccValue
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(EventName))
                {
                    var startIndex = EventName.IndexOf("Value") + 5;
                    var valueStr = EventName.Substring(startIndex, EventName.Length - startIndex);

                    int value;
                    bool success = int.TryParse(valueStr.Trim(), out value);
                    if (success)
                    {
                        return int.Parse(valueStr.Trim()) / 127;
                    }
                }
                return -1;
            }
        }

        public int ControlChangeValue
        {
            get
            {
                if (_ccValue != -1)
                {
                    return _ccValue;
                }
                return -1;
            }
        }
        
    }
}
