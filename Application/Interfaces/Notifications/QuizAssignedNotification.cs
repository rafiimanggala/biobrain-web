using System;
using System.Collections.Generic;
using System.Text;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class QuizzesAssignedNotification : IEmailNotification
    {
        private readonly List<NotificationLink> _quizLinks;
        private readonly string _userName;
        private readonly string _teacherName;
        private readonly string _className;
        private readonly DateTime _dueDate;
        private readonly string _icon;


        public QuizzesAssignedNotification(string email, List<NotificationLink> quizLinks, string userName, string teacherName, string className, DateTime dueDate, string icon)
        {
            _quizLinks = quizLinks;
            _userName = userName;
            _teacherName = teacherName;
            _dueDate = dueDate;
            _className = className;
            To = email;
            _icon = icon;
        }

        public string To { get; }
        public string Subject => $"BioBrain - {_className} {_icon} Assigned Work"; //{Unit,AOS, Topic, Level}

        public string HtmlBody
        {
            get
            {
                var builder = new StringBuilder();
                builder.AddParagraph($"Dear {_userName}");
                builder.AddParagraph($"You have been assigned a BioBrain quiz/quizzes to complete by {_dueDate:dd/MM/yyyy} ");
                builder.AddLinksList(_quizLinks);
                builder.AddParagraph($"Kind regards,");
                builder.AddParagraph($"{_teacherName}");
                return builder.ToString();
            }
        }
    }
}