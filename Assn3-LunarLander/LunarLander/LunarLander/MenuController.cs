using System.Collections.Generic;

namespace LunarLander
{
    public class MenuController
    {
        // gets the selected menu item
        protected static int GetSelectedIndex(IReadOnlyList<MenuItem> menuItems)
        {
            // iterate until finding a selected item
            for (var i = 0; i < menuItems.Count; i++)
            {
                if (menuItems[i].Selected)
                {
                    return i;
                }
            }

            // if nothing is selected, just select the first menu item and return
            menuItems[0].Selected = true;
            return 0;
        }

        protected static void SelectPreviousItem(IReadOnlyList<MenuItem> menuItems)
        {
            // find selected index, decrement it (with wraparound), deselect old and select new
            var selectedIndex = GetSelectedIndex(menuItems);
            int newIndex;
            if (selectedIndex == 0)
                newIndex = menuItems.Count - 1;
            else
                newIndex = selectedIndex - 1;

            menuItems[selectedIndex].Selected = false;
            menuItems[newIndex].Selected = true;
        }

        protected static void SelectNextItem(IReadOnlyList<MenuItem> menuItems)
        {
            // find selected index, increment it (with wraparound), deselect old and select new
            var selectedIndex = GetSelectedIndex(menuItems);
            int newIndex;
            if (selectedIndex == menuItems.Count - 1)
                newIndex = 0;
            else
                newIndex = selectedIndex + 1;

            menuItems[selectedIndex].Selected = false;
            menuItems[newIndex].Selected = true;
        }
    }
}