using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Objects.Enums;

namespace Objects
{
    public class ToggleGroupDto
    {
        public int Group { get; set; }
        public int FirstButton { get; set; }
        public int LastButton { get; set; }
        public FlashMode FlashMode { get; set; }
    }
}
