using System;

namespace BehaveAsSakura.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskAttribute : Attribute
    {
        public TaskAttribute(string title)
        {
            Title = title;
        }

        public string Title { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }
    }
}
