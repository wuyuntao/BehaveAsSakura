using BehaveAsSakura.Attributes;
using BehaveAsSakura.Tasks;
using System;
using System.Collections;
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

        public static TaskDescWrapper FindParentTask(IEnumerable<TaskDescWrapper> tasks, uint childTaskId)
        {
            foreach (var task in tasks)
            {
                if (task is DecoratorTaskDescWrapper)
                {
                    if (((DecoratorTaskDescWrapper)task).ChildTaskId == childTaskId)
                        return task;
                }
                else if (task is CompositeTaskDescWrapper)
                {
                    if (((CompositeTaskDescWrapper)task).ChildTaskIds.Contains(childTaskId))
                        return task;
                }
            }

            return null;
        }

        public static IEnumerable<PropertyInfo> FindAllProperties(object obj)
        {
            return from p in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   where p.GetCustomAttributes(typeof(BehaveAsFieldAttribute), true) != null
                   select p;
        }

        public static void AddNewTaskMenuItems(GenericMenu menu, bool enabled, GenericMenu.MenuFunction2 callback)
        {
            var newTaskText = I18n._("Create task");
            if (enabled)
            {
                foreach (var t in FindAllTaskDescs())
                {
                    var titleText = GetTaskTitle(t, true);
                    var menuItemText = string.Format("{0}/{1}", newTaskText, titleText);

                    menu.AddItem(new GUIContent(menuItemText), false, callback, t);
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(newTaskText));
            }
        }

        public static void AddMoveTaskMenuItems(GenericMenu menu, TaskState task, GenericMenu.MenuFunction2 callback)
        {
            if (task.ParentTask == null || task.ParentTask.Desc is LeafTaskDescWrapper || task.ParentTask.Desc is DecoratorTaskDescWrapper)
                return;

            var parentTask = (CompositeTaskDescWrapper)task.ParentTask.Desc;
            if (parentTask.ChildTaskIds.Count <= 1)
                return;

            var taskIndex = parentTask.ChildTaskIds.IndexOf(task.Desc.Id);
            var canMoveLeft = taskIndex > 0;
            var canMoveRight = taskIndex < parentTask.ChildTaskIds.Count - 1;

            if (canMoveLeft || canMoveRight)
                menu.AddSeparator("");

            if (canMoveLeft)
                menu.AddItem(new GUIContent(I18n._("Move left")), false, callback, -1);

            if (canMoveRight)
                menu.AddItem(new GUIContent(I18n._("Move Right")), false, callback, 1);
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

        public static string TextField(string label, string text, Action<Event> onLabelClick = null)
        {
            ClickableLabel(label, () =>
            {
                text = EditorGUILayout.TextField(text, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }, onLabelClick);
            return text;
        }

        public static void ReadOnlyTextField(string label, string text)
        {
            ClickableLabel(label, () =>
            {
                EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
        }

        public static string TextArea(string label, string text, int lineHeight = 4, Action<Event> onLabelClick = null)
        {
            ClickableLabel(label, () =>
            {
                EditorStyles.textArea.wordWrap = true;

                text = EditorGUILayout.TextArea(text,
                    FieldWidth(),
                    GUILayout.ExpandWidth(false),
                    GUILayout.Height(EditorGUIUtility.singleLineHeight * lineHeight));
            }, onLabelClick);
            return text;
        }

        public static void ReadOnlyTextArea(string label, string text, int lineHeight = 4)
        {
            ClickableLabel(label, () =>
            {
                EditorStyles.textArea.wordWrap = true;

                EditorGUILayout.SelectableLabel(text, EditorStyles.textArea,
                    FieldWidth(),
                    GUILayout.ExpandWidth(false),
                    GUILayout.Height(EditorGUIUtility.singleLineHeight * lineHeight));
            });
        }

        public static bool Toggle(string label, bool value, Action<Event> onLabelClick = null)
        {
            ClickableLabel(label, () =>
            {
                value = EditorGUILayout.Toggle(value);
            }, onLabelClick);
            return value;
        }

        public static int IntField(string label, int value, Action<Event> onLabelClick = null)
        {
            ClickableLabel(label, () =>
            {
                value = EditorGUILayout.IntField(value);
            }, onLabelClick);
            return value;
        }

        public static long LongField(string label, long value, Action<Event> onLabelClick = null)
        {
            ClickableLabel(label, () =>
            {
                value = EditorGUILayout.LongField(value);
            }, onLabelClick);
            return value;
        }

        public static float FloatField(string label, float value, Action<Event> onLabelClick = null)
        {
            ClickableLabel(label, () =>
            {
                value = EditorGUILayout.FloatField(value);
            }, onLabelClick);
            return value;
        }

        public static double DoubleField(string label, double value, Action<Event> onLabelClick = null)
        {
            ClickableLabel(label, () =>
            {
                value = EditorGUILayout.DoubleField(value);
            }, onLabelClick);
            return value;
        }

        public static Enum EnumPopup(string label, Enum value, Action<Event> onLabelClick = null)
        {
            ClickableLabel(label, () =>
            {
                value = EditorGUILayout.EnumPopup(value);
            }, onLabelClick);
            return value;
        }

        private static void ClickableLabel(string label, Action action, Action<Event> onLabelClick = null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LabelWidth());

            if (onLabelClick != null
                    && Event.current.type == EventType.MouseUp
                    && IsRightButton(Event.current)
                    && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                onLabelClick(Event.current);

                Event.current.Use();
            }

            action();
            EditorGUILayout.EndHorizontal();
        }

        private static GUILayoutOption LabelWidth()
        {
            return GUILayout.Width(EditorGUIUtility.labelWidth - 4 - 16 * EditorGUI.indentLevel);
        }

        private static GUILayoutOption FieldWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 23);
        }

        public static void Foldout(ref bool foldout, string name, Action action, Action<Event> onlabelClick = null)
        {
            foldout = EditorGUILayout.Foldout(foldout, name);

            if (onlabelClick != null
                    && Event.current.type == EventType.MouseUp
                    && IsRightButton(Event.current)
                    && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                onlabelClick(Event.current);

                Event.current.Use();
            }

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

        public static string GetTaskTitle(Type type, bool includingCategory)
        {
            var title = I18n.__(string.Format("Title of task '{0}'", type.FullName));

            if (string.IsNullOrEmpty(title))
            {
                var attributes = type.GetCustomAttributes(typeof(TaskAttribute), false);
                if (attributes.Length > 0)
                    title = ((TaskAttribute)attributes[0]).Title;
            }

            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"Missing title translation for task '{type.FullName}'");

            if (!includingCategory)
            {
                var slashIndex = title.LastIndexOf('/');
                if (slashIndex > 0)
                    title = title.Substring(slashIndex + 1);
            }

            return title;
        }

        public static string GetTaskDescription(Type type)
        {
            var desc = I18n.__(string.Format("Description of task '{0}'", type.FullName));

            if (string.IsNullOrEmpty(desc))
            {
                var attributes = type.GetCustomAttributes(typeof(TaskAttribute), false);
                if (attributes.Length > 0)
                    desc = ((TaskAttribute)attributes[0]).Description;
            }

            return desc;
        }

        public static Texture2D LoadTaskIcon(Type type)
        {
            string icon = null;
            var attributes = type.GetCustomAttributes(typeof(TaskAttribute), false);
            if (attributes.Length > 0)
                icon = ((TaskAttribute)attributes[0]).Icon;

            if (string.IsNullOrEmpty(icon))
                icon = string.Format("Icons/{0}.png", type.Name.Replace("TaskDesc", ""));

            if (string.IsNullOrEmpty(icon))
                icon = EditorConfiguration.DefaultTaskIconPath;

            return LoadTexture2D(icon);
        }

        public static object CloneObject(Type type, object original)
        {
            if (type.GetCustomAttributes(typeof(BehaveAsTableAttribute), true).Length == 0)
                throw new InvalidOperationException(string.Format("Cannot clone an object without {0}", typeof(BehaveAsTableAttribute).Name));

            if (original == null)
                return null;

            var clone = Activator.CreateInstance(type);
            var properties = from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             where pi.GetCustomAttributes(typeof(BehaveAsFieldAttribute), true).Length > 0
                             select pi;

            foreach (var pi in properties)
            {
                var value = pi.GetValue(original, null);
                if (value != null)
                    pi.SetValue(clone, CloneValue(pi.PropertyType, value), null);
            }

            return clone;
        }

        private static object CloneValue(Type type, object value)
        {
            if (value == null)
                return null;

            if (type == typeof(string)
                    || type == typeof(bool)
                    || type == typeof(byte) || type == typeof(sbyte)
                    || type == typeof(short) || type == typeof(ushort)
                    || type == typeof(int) || type == typeof(uint)
                    || type == typeof(long) || type == typeof(ulong)
                    || type == typeof(float) || type == typeof(double)
                    || type.IsEnum)
            {
                return value;
            }

            if (type.GetCustomAttributes(typeof(BehaveAsTableAttribute), true).Length > 0)
            {
                return CloneObject(type, value);
            }

            if (type.IsArray)
            {
                var array = (Array)value;
                var elementType = type.GetElementType();
                var cloneArray = Array.CreateInstance(elementType, array.Length);

                for (int i = 0; i < array.Length; i++)
                    cloneArray.SetValue(CloneValue(elementType, array.GetValue(i)), i);

                return cloneArray;
            }

            if ((typeof(List<>)).IsAssignableFrom(type))
            {
                var list = (IList)value;

                var elementType = type.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(elementType);
                var cloneList = (IList)Activator.CreateInstance(listType);

                for (int i = 0; i < list.Count; i++)
                    cloneList.Add(CloneValue(elementType, list[i]));

                return cloneList;
            }

            Logger.Error("Unsupported property. Type: {0}, Value: {1}", type, value);
            return null;
        }

        public static Texture2D LoadTexture2D(string path)
        {
            var internalPath = string.Format("BehaveAsSakura.Editor.Resources.{0}", path.Replace('/', '.'));
            var assembly = Assembly.GetExecutingAssembly();

            var stream = assembly.GetManifestResourceStream(internalPath);
            if (stream != null)
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                var texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                return texture;
            }
            else
                return EditorGUIUtility.Load(string.Format("BehaveAsSakura/{0}", path)) as Texture2D;
        }
    }
}
