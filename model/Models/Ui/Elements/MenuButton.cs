using System;
using System.Drawing;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;
using model.Interfaces.Ui;
using Myra.Events;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace model.Models.Ui.Elements
{
    public class MenuButton : MenuElement
    {
        private readonly MenuCommand _command;
        public MenuButton(
            string name, 
            IMenu parentMenu,
            MenuCommand command,
            bool isVisibleAndEnabled
        ) 
        : base(name, parentMenu)
        {
            _command = command;
            Widget.Visible = isVisibleAndEnabled;
            Widget.Enabled = isVisibleAndEnabled;
        }

        public MenuCommand GetCommand() => _command;
        public EventHandler Click;

        protected override Widget InitWidget()
        {
            var btn = new Button()
            {
                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment = VerticalAlignment,
                Border = new SolidBrush(Microsoft.Xna.Framework.Color.White),
                FocusedBorder = new SolidBrush(Microsoft.Xna.Framework.Color.Red),
                BorderThickness = new Myra.Graphics2D.Thickness(2, 2, 2, 2),
                Content = new Label
                {
                    Text = GetTitle(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                AcceptsKeyboardFocus = true,          
            };

            btn.Click += (s, a) => Click?.Invoke(s, a);
            btn.KeyDown += OnKeyDown;

            return btn;
        }

        private void OnKeyDown(object? sender, GenericEventArgs<Keys> key)
        {
            if (key.Data == Keys.Enter)
            {
                var btn = (Button)Widget;
                btn.DoClick();
            }
        }
    }
}