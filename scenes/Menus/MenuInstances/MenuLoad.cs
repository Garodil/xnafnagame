using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using model.Interfaces.Service;
using model.Interfaces.Ui;
using model.Models.Ui;
using model.Models.Ui.Commands;
using model.Models.Ui.Elements;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace scenes.Menus.MenuInstances
{
    public class MenuLoad : Menu
    {

        private StackPanel _saveFileArea;

        public MenuLoad(IMenu parent, IMenuManager menuManager) : base(parent, menuManager)
        {
        }

        protected override Container InitContainer()
        {
            _saveFileArea = new VerticalStackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            var overall = new Panel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(20)
            };
            overall.Widgets.Add(new ScrollViewer()
            {
                Content = _saveFileArea,
                ShowVerticalScrollBar = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,

            });
            return overall;
        }

        protected override void Init(ContentManager contentManager, IServiceProvider serviceProvider)
        {
        }

        protected override void OnExit(ContentManager contentManager, IServiceProvider serviceProvider)
        {
        }

        protected override void OnEnter(ContentManager contentManager, IServiceProvider serviceProvider)
        {
            PopulateSaveFiles();
        }

        private void PopulateSaveFiles()
        {

            string[] _savefiles = new string[50]; // TODO: add model that represents a save game
            foreach (var savefile in _savefiles)
            {
                var loadPanel = new Panel()
                {
                    Border = new SolidBrush(Color.White),
                    BorderThickness = new Thickness(2),
                    Height = 100,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                var menuButton = new MenuButton("Load", this, new LoadSaveCommand(savefile), true)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                
                loadPanel.Widgets.Add(new Label() { Text = savefile });
                loadPanel.Widgets.Add(menuButton.Widget);
                _menuElements.Add(menuButton);

                _saveFileArea.Widgets.Add(loadPanel);
            }
        }
    }
}