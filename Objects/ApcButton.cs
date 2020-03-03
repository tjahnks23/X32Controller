using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Objects.Enums;

namespace Objects
{
    public class ApcButton
    {
        public int Id { get; set; }
        public int Group { get; set; }
        public int Channel { get; set; }
        public int Note { get; set; }
        public bool Active { get; set; }
        public bool? Enabled { get; set; }
        public bool? Muted { get; set; }
        public FlashMode FlashMode { get; set; }        
        public ButtonType ButtonType { get; set; }
    }
}
