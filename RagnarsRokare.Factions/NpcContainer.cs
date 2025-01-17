﻿using HarmonyLib;

namespace RagnarsRokare.Factions
{
    internal class NpcContainer : Container, Interactable
    {
        private Humanoid m_npc;
        private Inventory m_humanoidInventory;

        private new void Awake()
        {
            base.Awake();
            m_nview.Unregister("OpenRespons");
            m_nview.Register<bool>("OpenRespons", RPC_OpenRespons);

            m_npc = gameObject.GetComponent<Humanoid>();
        }

        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Hide)), HarmonyPrefix]
        private void InventoryGui_Hide(InventoryGui __instance)
        {
            if (__instance.m_currentContainer?.m_inventory == m_humanoidInventory)
            {
                Save(m_npc);
            }
        }

        private new void RPC_OpenRespons(long uid, bool granted)
        {
            if ((bool)Player.m_localPlayer)
            {
                if (granted)
                {
                    m_inventory = m_humanoidInventory;
                    var npcZdo = gameObject.GetComponent<ZNetView>().GetZDO();
                    m_name = npcZdo.GetString(Constants.Z_GivenName);

                    InventoryGui.instance.Show(this);
                }
                else
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_inuse");
                }
            }
        }

        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.OnRightClickItem)), HarmonyPrefix]
        private bool InventoryGui_OnRightClickItem(InventoryGui __instance, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos)
        {
            if (grid.GetInventory() == this.GetInventory())
            {
                var itemType = item.m_shared.m_itemType;
                if (item.IsEquipable())
                {
                    if (m_npc.IsItemEquiped(item))
                    {
                        m_npc.UnequipItem(item);
                    }
                    else
                    {
                        m_npc.EquipItem(item);
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Load(Humanoid npc)
        {
            this.m_humanoidInventory = npc.GetInventory();
            var nview = npc.GetComponent<ZNetView>();
            if (nview != null && nview.IsValid())
            {
                var savedInventory = nview.GetZDO().GetByteArray(Misc.Constants.Z_SavedInventory);
                if (savedInventory != null)
                {
                    Jotunn.Logger.LogDebug($"{npc.GetHoverName()}:Loading inventory");
                    m_humanoidInventory.Load(new ZPackage(savedInventory));
                }
            }
        }

        public void Save(Humanoid npc)
        {
            var nview = npc.GetComponent<ZNetView>();
            if (nview != null && nview.IsValid())
            {
                Jotunn.Logger.LogDebug($"{npc.GetHoverName()}:Saving inventory");
                var savedInventory = new ZPackage();
                m_humanoidInventory.Save(savedInventory);
                nview.GetZDO().Set(Misc.Constants.Z_SavedInventory, savedInventory.GetArray());
            }
        }

        public bool NpcInteract(Humanoid character)
        {
            return base.Interact(character, false, false);
        }

        public new bool Interact(Humanoid character, bool hold, bool alt)
        {
            var tamable = gameObject.GetComponent<Tameable>();
            return tamable.Interact(character, hold, alt);
        }
    }
}
