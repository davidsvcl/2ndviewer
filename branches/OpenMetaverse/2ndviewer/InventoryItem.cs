using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenMetaverse;

namespace _2ndviewer
{
    class InventoryItem
    {
        public InventoryItem()
        {
        }
        public int indent_;
        public string Name_;
        public UUID uuid_;
        public bool folder_;
    }
}
