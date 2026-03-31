using System;
using System.Collections.Generic;
using System.Text;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class LearningMaterialsAssignedNotification : IEmailNotification
    {
        private readonly List<NotificationLink> _learningMaterialLinks;
        private readonly string _userName;
        private readonly string _teacherName;
        private readonly string _className;
        private readonly DateTime _dueDate;
        private readonly string _icon;

        public LearningMaterialsAssignedNotification(string email, List<NotificationLink> learningMaterialLinks, string userName, string teacherName, string className, DateTime dueDate, string icon)
        {
            _learningMaterialLinks = learningMaterialLinks;
            _userName = userName;
            _teacherName = teacherName;
            _dueDate = dueDate;
            _className = className;
            To = email;
            _icon = icon;
        }

        public string To { get; }
        public string Subject => $"BioBrain - {_className} {_icon} Assigned Work"; //: {Unit, AOS, Topic, Level}
        public string HtmlBody
        {
            get
            {
                var builder = new StringBuilder();
                builder.AddParagraph($"Dear {_userName}");
                builder.AddParagraph($"You have been assigned a BioBrain learning material(s) to complete by {_dueDate:dd/MM/yyyy}");
                builder.AddLinksList(_learningMaterialLinks);
                builder.AddParagraph($"Kind regards,");
                builder.AddParagraph($"{_teacherName}");
                return builder.ToString();
            }
        }
    }
}