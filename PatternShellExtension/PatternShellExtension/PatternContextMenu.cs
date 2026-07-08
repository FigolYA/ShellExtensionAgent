using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpShell;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace PatternShellExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class PatternContextMenu : SharpContextMenu
    {
        private static readonly Dictionary<string, List<FileAction>> ActionMap = new Dictionary<string, List<FileAction>>()
        {
            { ".pattern", new List<FileAction>()
                {
                    new FileAction("Импортировать в...", (path) => {
                        MessageBox.Show($"Импорт файла: {path}", "Импорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }),
                    new FileAction("Экспортировать как...", (path) => {
                        MessageBox.Show($"Экспорт файла: {path}", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }),
                    new FileAction("Проверить валидность", (path) => {
                        MessageBox.Show($"Проверка файла: {path}", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    })
                }
            },
            { ".txt", new List<FileAction>()
                {
                    new FileAction("Открыть в Блокноте", (path) => {
                        System.Diagnostics.Process.Start("notepad.exe", path);
                    }),
                    new FileAction("Показать статистику", (path) => {
                        var info = new FileInfo(path);
                        MessageBox.Show($"Размер: {info.Length} байт\nСоздан: {info.CreationTime}", "Статистика");
                    })
                }
            }
        };

        protected override bool CanShowMenu()
        {
            return SelectedItemPaths.Count() == 1;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var mainMenu = new ContextMenuStrip();
            var filePath = SelectedItemPaths.First();
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            // Здесь исправлено: actions.Count (свойство) или actions.Count() (метод)
            if (!ActionMap.TryGetValue(extension, out var actions) || actions.Count == 0)
            {
                var noActionItem = new ToolStripMenuItem("Нет доступных действий");
                noActionItem.Enabled = false;
                mainMenu.Items.Add(noActionItem);
                return mainMenu;
            }

            var mainMenuItem = new ToolStripMenuItem("Дополнительные действия")
            {
                Image = null
            };

            foreach (var action in actions)
            {
                var menuItem = new ToolStripMenuItem(action.DisplayName);
                menuItem.Click += (sender, args) => action.Execute(filePath);
                mainMenuItem.DropDownItems.Add(menuItem);
            }

            mainMenu.Items.Add(mainMenuItem);
            return mainMenu;
        }
    }

    public class FileAction
    {
        public string DisplayName { get; }
        public Action<string> Execute { get; }

        public FileAction(string displayName, Action<string> execute)
        {
            DisplayName = displayName;
            Execute = execute;
        }
    }
}