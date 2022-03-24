﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RagnarsRokare.Factions
{
    internal class Helpers
    {
        public static Tameable GetOrAddTameable(GameObject gameObject)
        {
            var tameable = gameObject.GetComponent<Tameable>();
            if (tameable == null)
            {
                tameable = gameObject.AddComponent<Tameable>();
            }

            return tameable;
        }

        public static string GetOrCreateUniqueId(ZNetView ___m_nview)
        {
            var uniqueId = ___m_nview.GetZDO().GetString(Constants.Z_UniqueId);
            if (string.IsNullOrEmpty(uniqueId))
            {
                uniqueId = ___m_nview.GetZDO().GetString(Constants.Z_CharacterId);
                if (string.IsNullOrEmpty(uniqueId))
                {
                    uniqueId = System.Guid.NewGuid().ToString();
                }
                ___m_nview.GetZDO().Set(Constants.Z_UniqueId, uniqueId);
            }
            return uniqueId;
        }

        public static int GetComfortFromNearbyPieces(Vector3 position)
        {
            List<Piece> list = ((!Terminal.m_testList.ContainsKey("oldcomfort")) ? SE_Rested.GetNearbyComfortPieces(position) : SE_Rested.GetNearbyPieces(position));
            list.OrderByDescending(p => p.GetComfort());
            int num = 1;
            num++;
            for (int i = 0; i < list.Count; i++)
            {
                Piece piece = list[i];
                if (i > 0)
                {
                    Piece piece2 = list[i - 1];
                    if ((piece.m_comfortGroup != 0 && piece.m_comfortGroup == piece2.m_comfortGroup) || piece.m_name == piece2.m_name)
                    {
                        continue;
                    }
                }
                num += piece.GetComfort();
            }
            return num;
        }

    }
}
