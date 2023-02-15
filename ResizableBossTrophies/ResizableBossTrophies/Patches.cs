using HarmonyLib;
using System;
using UnityEngine;
using static ResizableBossTrophies.ResizableBossTrophiesPlugin;

namespace ResizableBossTrophies
{
    [HarmonyPatch(typeof(ItemDrop.ItemData))]
    internal static class PatchItemDrop
    {
        [HarmonyPatch(nameof(ItemDrop.ItemData.GetScale), new Type[] { typeof(float) }), HarmonyPostfix]
        public static void GetScale_Postfix(ItemDrop.ItemData __instance, ref Vector3 __result)
        {
            switch (__instance.m_shared.m_name)
            {
                case "$item_trophy_eikthyr":
                    __result *= EikthyTrophySize.Value;
                    break;

                case "$item_trophy_elder":
                    __result *= ElderTrophySize.Value;
                    break;

                case "$item_trophy_bonemass":
                    __result *= BonemassTrophySize.Value;
                    break;

                case "$item_trophy_dragonqueen":
                    __result *= ModerTrophySize.Value;
                    break;

                case "$item_trophy_goblinking":
                    __result *= YagluthTrophySize.Value;
                    break;

                case "$item_trophy_seekerqueen":
                    __result *= QueenTrophySize.Value;
                    break;

                //////////////////////////////////////////

                case "$item_trophy_troll":
                    __result *= TrollTrophySize.Value;
                    break;

                case "$item_trophy_abomination":
                    __result *= AbominationTrophySize.Value;
                    break;

                case "$item_trophy_wraith":
                    __result *= WraithTrophySize.Value;
                    break;

                case "$item_trophy_leech":
                    __result *= LeechTrophySize.Value;
                    break;

                case "$item_trophy_serpent":
                    __result *= SerpentTrophySize.Value;
                    break;

                case "$item_trophy_sgolem":
                    __result *= StoneGolemTrophySize.Value;
                    break;

                case "$item_trophy_goblinbrute":
                    __result *= FulingBerserkerTrophySize.Value;
                    break;

                case "$item_trophy_lox":
                    __result *= LoxTrophySize.Value;
                    break;

                case "$item_trophy_gjall":
                    __result *= GjallTrophySize.Value;
                    break;

                case "$item_trophy_seeker_brute":

                    __result *= SeekerSoldierTrophySize.Value;
                    break;

                case "$item_trophy_seeker":

                    __result *= SeekerTrophySize.Value;
                    break;
            }
        }
    }
}