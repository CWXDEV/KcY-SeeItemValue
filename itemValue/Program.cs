using HarmonyLib;

namespace itemValueMod
{
    public class Program
    {
        static void Main(string[] args)
        {
            var harmony = new Harmony("com.CWX.ItemValuePatch");
            harmony.PatchAll();
        }
    }
}
