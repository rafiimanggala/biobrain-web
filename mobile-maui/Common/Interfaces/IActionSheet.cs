using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Common.Interfaces
{
    public interface IActionSheet
    {
        Task<String> UseActionSheet(Page p, String title, String cancel, params String[] buttons);
    }
}