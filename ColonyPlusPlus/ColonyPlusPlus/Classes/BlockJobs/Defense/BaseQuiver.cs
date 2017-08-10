﻿using BlockTypes.Builtin;
using System.Collections.Generic;
using NPC;
using Pipliz.APIProvider.Jobs;
using Pipliz;
using Pipliz.JSON;
using UnityEngine;

namespace ColonyPlusPlus.Classes.BlockJobs.Defense
{
	public class BaseQuiver : BlockJobBase, IBlockJobBase, INPCTypeDefiner
    {
        ushort quiverType;
        Zombie target;

        public string jobtype = "archery";

        public override string NPCTypeKey { get { return "pipliz.guardbow"; } }

        public override float TimeBetweenJobs
        {
            get
            {
                Data.NPCData d = Managers.NPCManager.getNPCData(this.usedNPC.ID, this.owner);

                return 2.5f * d.XPData.getCraftingMultiplier(jobtype);
            }
        }

        public override bool ToSleep { get { return false; } }

        public override InventoryItem RecruitementItem { get { return new InventoryItem(BuiltinBlocks.Bow, 1); } }

        public override ITrackableBlock InitializeFromJSON(Players.Player player, JSONNode node)
        {
            quiverType = ItemTypes.IndexLookup.GetIndex(node.GetAs<string>("type"));
            InitializeJob(player, (Vector3Int)node["position"], node.GetAs<int>("npcID"));
            return this;
        }

        public ITrackableBlock InitializeOnAdd(Vector3Int position, ushort type, Players.Player player)
        {
            quiverType = type;
            InitializeJob(player, position, 0);
            return this;
        }

        public override JSONNode GetJSON()
        {
            return base.GetJSON()
                .SetAs("type", ItemTypes.IndexLookup.GetName(quiverType));
        }

        public override void OnNPCDoJob(ref NPCBase.NPCState state)
        {
            if (target != null && target.IsValid)
            {
                Vector3 npcPos = usedNPC.Position + Vector3.up;
                Vector3 targetPos = target.Position + Vector3.up;
                if (General.Physics.Physics.CanSee(npcPos, targetPos))
                {
                    usedNPC.LookAt(targetPos);
                    if (Stockpile.GetStockPile(owner).Remove(BuiltinBlocks.Arrow, 1))
                    {
                        Arrow.New(npcPos, targetPos, target.Direction);
                        AddXP();
                        OverrideCooldown(5.0);
                    }
                    else
                    {
                        state.SetIndicator(NPCIndicatorType.MissingItem, 1.5f, BuiltinBlocks.Arrow);
                        OverrideCooldown(1.5);
                    }
                }
                else
                {
                    target = null;
                }
            }
            if (target == null || !target.IsValid)
            {
                target = ZombieTracker.Find(position.Add(0, 1, 0), 20);
                if (target == null)
                {
                    Vector3 desiredPosition = usedNPC.Position;
                    if (quiverType == BuiltinBlocks.QuiverXN)
                    {
                        desiredPosition += Vector3.left;
                    }
                    else if (quiverType == BuiltinBlocks.QuiverXP)
                    {
                        desiredPosition += Vector3.right;
                    }
                    else if (quiverType == BuiltinBlocks.QuiverZP)
                    {
                        desiredPosition += Vector3.forward;
                    }
                    else if (quiverType == BuiltinBlocks.QuiverZN)
                    {
                        desiredPosition += Vector3.back;
                    }
                    usedNPC.LookAt(desiredPosition);
                }
                else
                {
                    OverrideCooldown(0.3);
                }
            }
        }

        public override void OnRemovedNPC()
        {
            Managers.NPCManager.removeNPCData(this.usedNPC.ID);
            base.OnRemovedNPC();
        }

        private void AddXP()
        {
            Data.NPCData d = Managers.NPCManager.getNPCData(this.usedNPC.ID, this.owner);
            d.XPData.addXP(jobtype, this.owner);
            Managers.NPCManager.updateNPCData(this.usedNPC.ID, d);
        }

        NPCTypeSettings INPCTypeDefiner.GetNPCTypeDefinition()
        {
            NPCTypeSettings def = NPCTypeSettings.Default;
            def.keyName = NPCTypeKey;
            def.printName = "Bow guard";
            def.maskColor1 = new Color32(159, 155, 152, 255);
            def.type = NPCTypeID.GetNextID();
            return def;
        }
    }
}