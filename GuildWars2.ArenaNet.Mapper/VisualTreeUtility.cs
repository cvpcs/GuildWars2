using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace GuildWars2.ArenaNet.Mapper
{
    public static class VisualTreeUtility
    {
        public static DependencyObject FindParent(DependencyObject child, Type parentType)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parentType.IsAssignableFrom(parent.GetType()))
                    return parent;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        public static IList<DependencyObject> FindChildren(DependencyObject parent, Type childType)
        {
            IList<DependencyObject> children = new List<DependencyObject>();

            if (childType.IsAssignableFrom(parent.GetType()))
                children.Add(parent);

            for (int i = 0, n = VisualTreeHelper.GetChildrenCount(parent); i < n; i++)
            {
                foreach (DependencyObject child in FindChildren(VisualTreeHelper.GetChild(parent, i), childType))
                {
                    children.Add(child);
                }
            }

            return children;
        }
    }
}
