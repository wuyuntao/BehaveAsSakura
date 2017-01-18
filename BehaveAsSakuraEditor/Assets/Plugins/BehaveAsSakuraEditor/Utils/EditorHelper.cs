using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public static class EditorHelper
    {
        public static IEnumerable<Type> FindAllTaskDescs()
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(Assembly.GetCallingAssembly());
            assemblies.AddRange(Assembly.GetCallingAssembly().GetReferencedAssemblies().Select(a => Assembly.Load(a)));

            return from a in assemblies
                   from t in a.GetTypes()
                   where t != typeof(ITaskDesc) && typeof(ITaskDesc).IsAssignableFrom(t)
                   select t;
        }

        public static void AddNewTaskMenuItems(GenericMenu menu, bool enabled, GenericMenu.MenuFunction2 callback)
        {
            var newTaskText = I18n._("New task");
            if (enabled)
            {
                foreach (var t in FindAllTaskDescs())
                {
                    var categoryText = I18n._(string.Format("Category of task '{0}'", t.FullName));
                    var titleText = I18n._(string.Format("Title of task '{0}'", t.FullName));
                    var menuItemText = string.Format("{0}/{1}/{2}", newTaskText, categoryText, titleText);

                    menu.AddItem(new GUIContent(menuItemText), false, callback, t);
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(newTaskText));
            }
        }

        public static bool IsLeftButton(Event e)
        {
            return e.button == 0;
        }

        public static bool IsRightButton(Event e)
        {
            return e.button == 1;
        }

        public static bool IsMiddleButton(Event e)
        {
            return e.button == 2;
        }
    }
}
