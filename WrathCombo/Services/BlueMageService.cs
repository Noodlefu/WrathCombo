using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Linq;
namespace WrathCombo.Services;

internal unsafe static class BlueMageService
{
    public static void PopulateBLUSpells()
    {
        // 25 native game calls per frame is expensive for every player on every job —
        // bail immediately when not on BLU.
        if (Player.Job != Job.BLU)
            return;

        // Slots can't change faster than a player can reassign them via the UI,
        // so polling every 2 seconds is more than responsive enough.
        if (!EzThrottler.Throttle("PopulateBLUSpells", 2000))
            return;

        var prevList = Service.Configuration.ActiveBLUSpells.ToList();
        Service.Configuration.ActiveBLUSpells.Clear();

        for (int i = 0; i <= 24; i++)
        {
            var id = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
            if (id != 0)
                Service.Configuration.ActiveBLUSpells.Add(id);
        }

        if (Service.Configuration.ActiveBLUSpells.Except(prevList).Any())
            Service.Configuration.Save();
    }
}