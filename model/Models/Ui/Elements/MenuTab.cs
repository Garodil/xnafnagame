using model.Interfaces.Ui;
using Myra.Graphics2D.UI;

namespace model.Models.Ui.Elements
{
    public class MenuTab : MenuElement
    {
        public Widget _containedElement;
        private readonly bool _isVertical;

        public MenuTab(
            Widget container, 
            string name, 
            IMenu parentMenu,
            bool isVertical
        ) 
        : base(
            name, 
            parentMenu
        )
        {
            _containedElement = container;
            _isVertical = isVertical;
        }

        protected override Widget InitWidget()
        {
            return _isVertical ? new VerticalStackPanel() : new HorizontalStackPanel();
        }
    }
}