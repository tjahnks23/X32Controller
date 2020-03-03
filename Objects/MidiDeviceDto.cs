using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace Objects
{
    public class MidiDeviceDto
    {        
        public List<ToggleGroupDto> ToggleGroups { get; set; }
        public Dictionary<int, ToggleGroupDto> ToggleGroupLookup { get; set; }
        public MidiIn MidiIn { get; set; }
        public MidiOut LocalMidiOut { get; set; }
        public List<ApcButton> ApcButtons { get; set; }
    }
}
