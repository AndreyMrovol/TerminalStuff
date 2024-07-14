using OpenLib.Menus;
using System.Collections.Generic;
using static OpenLib.Menus.MenuBuild;
using static OpenLib.ConfigManager.ConfigSetup;
using OpenLib.ConfigManager;

namespace TerminalStuff
{

    internal class MenuBuild
    {
        internal static List<TerminalMenuCategory> myMenuCategories = [];
        internal static List<TerminalMenuItem> myMenuItems = [];
        internal static TerminalMenu myMenu;
        internal static void CategoryList()
        {
            Dictionary<string, string> myCategories = [];
            myCategories.Add("COMFORT", "Improves the terminal user experience.");
            myCategories.Add("EXTRAS", "Adds extra functionality to the ship terminal.");
            myCategories.Add("CONTROLS", "Gives terminal more control of the ship's systems.");
            myCategories.Add("FUN", "Type \"fun\" for a list of these [FUN]ctional commands.");
            myMenuCategories = InitCategories(myCategories);

            //CatName = item.Key,
            //CatDescription = item.Value
            CreateDarmuhsTerminalStuffMenus();
        }

        internal static void CreateDarmuhsTerminalStuffMenus()
        {
            Plugin.Spam("START CreateDarmuhsTerminalStuffMenus");
            myMenuItems = TerminalMenuItems(defaultManagedBools);
            AddMenuItems(ConfigSettings.TerminalStuffBools, myMenuItems);

            myMenu = AssembleMainMenu("darmuhsTerminalStuff", "more", "Welcome to darmuh's Terminal Upgrade!\r\n\tSee below Categories for new stuff :)", myMenuCategories, myMenuItems);

            Plugin.Spam($"myMenu info:\nMenuName: {myMenu.MenuName}\nmyMenu.Categories.Count: {myMenu.Categories.Count}\n");

            CreateCategoryCommands(myMenu, defaultListing);

            Plugin.Spam("END CreateDarmuhsTerminalStuffMenus");

        }

        internal static void AddMenuItems(List<ManagedBool> managedBools, TerminalMenu myMenu)
        {
            if (myMenu.menuItems.Count == 0)
                return;

            foreach(ManagedBool item in managedBools)
            {
                if (item.menuItem == null)
                    continue;

                if(!myMenu.menuItems.Contains(item.menuItem))
                    myMenu.menuItems.Add(item.menuItem);
            }
        }

        internal static void AddMenuItems(List<ManagedBool> managedBools, List<TerminalMenuItem> myMenuItems)
        {
            if (myMenuItems.Count == 0)
                return;

            foreach (ManagedBool item in managedBools)
            {
                if (item.menuItem == null)
                    continue;

                if (!myMenuItems.Contains(item.menuItem))
                    myMenuItems.Add(item.menuItem);
            }
        }

        internal static void RefreshMyMenu()
        {
            myMenu.menuItems.Clear();
            myMenuItems.Clear();
            myMenuItems = TerminalMenuItems(defaultManagedBools);
            myMenu.menuItems = myMenuItems;
            AddMenuItems(ConfigSettings.TerminalStuffBools, myMenu);
            UpdateCategories(myMenu);
        }

        internal static void ClearMyMenustuff()
        {
            myMenu.Delete();
            myMenuItems.Clear();
            myMenuCategories.Clear();
        }
    }
}
