using BepInEx;
using HarmonyLib;

namespace itemValueMod
{
    [BepInPlugin("com.KcY.SeeItemValue", "KcY-SeeItemValue", "1.2.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new ItemPatch().Enable();
            new AmmoPatch().Enable();
            new GrenadePatch().Enable();
            new SecureContainerPatch().Enable();
        }
    }
}
