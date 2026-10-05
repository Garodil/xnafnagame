using System;
using System.Linq;
using model.Interfaces.Ui;
using Myra.Graphics2D.UI;

namespace model.Models.Ui
{
    public abstract class MenuElement
    {
        private readonly string _name;
        private readonly Widget _widget;
        public Widget Widget => _widget;
        protected readonly WeakReference<IMenu> _parentMenu;

        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

        public bool IsFocused => _widget.IsKeyboardFocused;

        public MenuElement(string name, IMenu parentMenu)
        {
            _name = name;
            _parentMenu = new WeakReference<IMenu>(parentMenu);
            _widget = InitWidget();
            _widget.MouseEntered += (s, args) => MouseEntered?.Invoke(this, args);
            _widget.MouseLeft += (s, args) => MouseLeft?.Invoke(this, args);
        }

        public event EventHandler MouseEntered;
        public event EventHandler MouseLeft;

        public IMenu? GetParentMenu() => _parentMenu.TryGetTarget(out var menu) ? menu : null;
        public string GetTitle() => _name;

        protected abstract Widget InitWidget();
    }
}