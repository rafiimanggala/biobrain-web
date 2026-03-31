using System.Collections.Generic;

namespace Biobrain.Application.Values
{
    public static class PageLabelMapper
    {
        private static Dictionary<string, string> _pagePathLabels = new Dictionary<string, string>
        {
            { "/teacher/student-results", "Student results" },
            { "/periodic-table", "Periodic table" },
            { "/teacher/class-results", "Class results (teacher)" },
            { "/bookmarks", "Saved items" },
            { "/teacher/my-classes", "My classes (teacher)" },
            { "/my-courses", "My courses" },
            { "/materials-search", "Materials search" },
            { "/quiz", "Quizzes" },
            { "/assigned-work", "Assigned work" },
            { "/teacher/work-assigned", "Assigned work (teacher)" },
            { "/teacher/class-admin", "Class admin (teacher)" },
            { "/glossary", "Glossary" },
            { "/materials/course", "Learning material" },

        };

        public static string GetLabel(string path)
        {
            var success = _pagePathLabels.TryGetValue(path, out var value);
            return success ? value : path;
        }
    }
}