﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColonyPlusPlus.Classes.Data
{
    class PlayerData
    {
        public NetworkID PID { get; private set; }
        public string chunkID;
        public TradeData tradeData;
        public NetworkID tpRequester = NetworkID.Invalid;
        public bool requestingTP = false;
        public bool tpHere = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">Player NetworkID</param>
        /// <param name="cid">ChunkID</param>
        public PlayerData(NetworkID n, string cid)
        {
            PID = n;
            chunkID = cid;
        }
    }
}
