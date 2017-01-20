using BehaveAsSakura.Attributes;
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
                   where t != typeof(ITaskDesc)
                        && t != typeof(ILeafTaskDesc)
                        && t != typeof(IDecoratorTaskDesc)
                        && t != typeof(ICompositeTaskDesc)
                        && typeof(ITaskDesc).IsAssignableFrom(t)
                   select t;
        }

        public static IEnumerable<PropertyInfo> FindAllProperties(object obj)
        {
            return from p in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   where p.GetCustomAttributes(typeof(BehaveAsFieldAttribute), true) != null
                   select p;
        }

        public static void AddNewTaskMenuItems(GenericMenu menu, bool enabled, GenericMenu.MenuFunction2 callback)
        {
            var newTaskText = I18n._("New task");
            if (enabled)
            {
                foreach (var t in FindAllTaskDescs())
                {
                    var categoryText = GetTaskCategory(t);
                    var titleText = GetTaskTitle(t);
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

        public static void DisplayDialog(string title, string message)
        {
            title = I18n._(title);
            message = I18n._(message);
            var ok = I18n._("Ok");

            EditorUtility.DisplayDialog(title, message, ok);
        }

        public static void ReadOnlyTextField(string label, string text)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LabelWidth());
            EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndHorizontal();
        }

        public static string TextArea(string label, string text, int lineHeight = 4)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LabelWidth());
            text = EditorGUILayout.TextArea(text, GUILayout.Height(EditorGUIUtility.singleLineHeight * lineHeight));
            EditorGUILayout.EndHorizontal();
            return text;
        }

        public static void ReadOnlyTextArea(string label, string text, int lineHeight = 4)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LabelWidth());
            EditorGUILayout.SelectableLabel(text, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * lineHeight));
            EditorGUILayout.EndHorizontal();
        }

        private static GUILayoutOption LabelWidth()
        {
            return GUILayout.Width(EditorGUIUtility.labelWidth - 4 - 16 * EditorGUI.indentLevel);
        }

        public static void Foldout(ref bool foldout, string name, Action action)
        {
            foldout = EditorGUILayout.Foldout(foldout, name);
            if (foldout)
            {
                EditorGUI.indentLevel++;
                action();
                EditorGUI.indentLevel--;
            }
        }

        public static void HeaderIconAndTitle(ScriptableObject target, Texture2D icon, string title)
        {
            typeof(EditorGUIUtility).InvokeMember("SetIconForObject"
                    , BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic
                    , null, null, new object[] { target, icon });

            target.name = title;
        }

        public static string GetPropertyName(PropertyInfo pi)
        {
            var t = I18n.__("Name of '{0}.{1}'", pi.PropertyType.FullName, pi.Name);

            return t ?? pi.Name;
        }

        public static string GetTaskCategory(Type type)
        {
            return I18n._(string.Format("Category of task '{0}'", type.FullName));
        }

        public static string GetTaskTitle(Type type)
        {
            var t = I18n.__(string.Format("Title of task '{0}'", type.FullName));

            return t ?? type.Name.Replace("TaskDesc", "");
        }

        public static string GetTaskDescription(Type type)
        {
            return I18n.__(string.Format("Description of task '{0}'", type.FullName));
        }

        public static string GetTaskIcon(Type type)
        {
            return string.Format("BehaveAsSakuraEditor/Textures/{0}", type.Name.Replace("TaskDesc", ""));
        }
    }
}
