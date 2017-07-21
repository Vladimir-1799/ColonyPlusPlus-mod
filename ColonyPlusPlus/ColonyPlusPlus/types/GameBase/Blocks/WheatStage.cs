﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColonyPlusPlus.types.GameBase.Blocks
{
    class WheatStage : classes.Type
    {
        public WheatStage(string name) : base(name)
        {
            this.IsSolid = false;
            this.NeedsBase = true;
            this.IsPlaceable = true;
            this.OnRemoveAudio = "grassDelete";
            this.OnPlaceAudio = "grassDelete";
            this.MaxStackSize = 1200;
            this.Register();
        }
    }
}
