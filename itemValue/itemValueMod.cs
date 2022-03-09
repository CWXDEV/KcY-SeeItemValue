/* Original Author: KyC
 * CWX - updated to work for 2.2.3 AKI
 * Client version: 0.12.12.15.16909
 *  - removed the need for KyC's ModLoader
 *  - Commented out the code for stackCount total worth
 *  - Commented out the code for weapon parts total worth
 *  - Commented out the code for container total worth
 *  - Commented out the code for magazine total worth
 */

using EFT.InventoryLogic;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Aki.Common.Utils;
using System.Linq;
using Newtonsoft.Json;

using Ammo = GClass2076;
using Grenade = GClass2079;
using GrenadeTemplate = GClass1975;
using SecureContainer = GClass2038;
using SecureContainerTemplate = GClass1937;
using Container = GClass2001; // CWX - leaving in as commented out code uses this
using Magazine = GClass2026; // CWX - leaving in as commented out code uses this
using ItemAttribute = GClass2090;
using Aki.Common.Http;

namespace itemValueMod
{
    public class ItemValue
    {
        public static void AddItemValue<T>(ref T __instance, string id, ItemTemplate template) where T : Item
        {
            // Remove item if it has no value
            // if (Math.Round(__instance.Value()) == 0) return;

            // Make a copy of the existing attributes list, this is needed for inherited types of Item that use a global attributes list (ammo)
            var atts = new List<ItemAttribute>();
            atts.AddRange(__instance.Attributes);
            __instance.Attributes = atts;
            ItemAttribute attr = new ItemAttribute(EItemAttributeId.MoneySum)
            {
                StringValue = new Func<string>(__instance.ValueStr), // ₽
                Name = ValueExtension.ValueTrName(), //new Func<string>(ValueExtension.ValueTrName).ToString(),
                DisplayType = new Func<EItemAttributeDisplayType>(() => EItemAttributeDisplayType.Compact)
            };
            __instance.Attributes.Add(attr);
        }
    }

    public static class ValueExtension
    {
        public static string name = "test";
        public static double Value(this Item item)
        {
            var template = item.Template;
            string itemId = template._id;
            double _price = 1;
            var json = RequestHandler.GetJson($"/cwx/itemvaluemod/{itemId}"); // CWX - sends ID to server
            Dictionary<string, string> temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var price = temp["result"];
            name = temp["name"];
            _price = Convert.ToDouble(price);

            // CWX - I have commented out total worth of items for guns etc as it was causing huge client stutters, i think it showing for each item only is better anyway
            // Container
            //if (item is Container container)
            //{
            //    foreach (var slot in container.Slots)
            //    {
            //        foreach (var i in slot.Items)
            //        {
            //            _price += i.Value();
            //        }
            //    }
            //    foreach (var c in container.Containers)
            //    {
            //        foreach (var i in c.Items)
            //        {
            //            _price += i.Value();
            //        }
            //    }
            //}

            //if (item is Magazine mag)
            //{
            //    foreach (var i in mag.Cartridges.Items)
            //    {
            //        _price += i.Value();
            //    }

            //}

            //if (item is Weapon wep)
            //{
            //    foreach (Slot s in wep.Chambers)
            //    {
            //        foreach (Item i in s.Items)
            //        {
            //            _price += i.Value();
            //        }
            //    }
            //}

            var medKit = item.GetItemComponent<MedKitComponent>();
            if (medKit != null)
            {
                _price *= medKit.HpResource / medKit.MaxHpResource;
            }

            var repair = item.GetItemComponent<RepairableComponent>();
            if (repair != null)
            {
                _price *= repair.Durability / repair.MaxDurability;
            }

            var dogtag = item.GetItemComponent<DogtagComponent>();
            if (dogtag != null)
            {
                _price *= dogtag.Level;
            }

            //_price *= item.StackObjectsCount;

            return _price;
        }
        public static string ValueStr(this Item item)
        {
            return Math.Round(item.Value()).ToString();
        }

        public static string ValueTrName()
        {
            return name;
        }
    }
    [HarmonyPatch]
    public class itemValueMod
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Item), MethodType.Constructor, new Type[] { typeof(string), typeof(ItemTemplate) })]
        static void PostfixItem(ref Item __instance, string id, ItemTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ammo), MethodType.Constructor, new Type[] { typeof(string), typeof(AmmoTemplate) })]
        static void PostfixAmmo(ref Ammo __instance, string id, AmmoTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Grenade), MethodType.Constructor, new Type[] { typeof(string), typeof(GrenadeTemplate) })]
        static void PostfixGrenade(ref Grenade __instance, string id, GrenadeTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SecureContainer), MethodType.Constructor, new Type[] { typeof(string), typeof(SecureContainerTemplate) })]
        static void PostfixConainer(ref SecureContainer __instance, string id, SecureContainerTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);
    }
}