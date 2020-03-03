using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Objects.Enums;

namespace Objects
{
    public class X32EventDto
    {
        MidiEventDto _midiEventDto;

        public X32EventDto(MidiEventDto midiEvent)
        {
            _midiEventDto = midiEvent;
        }

        public int ChannelIndex
        {
            get
            {
                if (_midiEventDto != null)
                {
                    return _midiEventDto.Channel - 1;
                }
                return -1;
            }
        }

        public float TranslatedFloatValue
        {
            get
            {
                if (_midiEventDto?.ControlChangeValue != null)
                {
                    return _midiEventDto.ControlChangeValue / 127;
                }
                return -1;
            }
        }

        public bool ChannelMuted
        {
            get
            {
                return false;
                //if (_midiEventDto != null)
                //{
                //    return _midiEventDto.;
                //}
                //return -1;
            }
        }

        public RangeSelect ChannelRangeSelection
        {
            get
            {
                if (_midiEventDto != null)
                {
                    if (_midiEventDto.Channel == 1 && _midiEventDto.NoteNumber == 2)
                    {
                        return RangeSelect.Ch1_8;
                    }
                    else if (_midiEventDto.Channel == 2 && _midiEventDto.NoteNumber == 3)
                    {
                        return RangeSelect.Ch9_16;
                    }                
                }
                return RangeSelect.Unknown;
            }
        }
    }
}
