using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using model.Models.Ui;
using Myra.Graphics2D.UI;

namespace model.Interfaces.Ui
{
    public interface IMenu 
    {
        public string GetName();
        public Widget GetWidget();
        public IMenu? GetParent();
        public IMenu? GetChild(string name);
        public void Enter(ContentManager contentManager, IServiceProvider serviceProvider);
        public void Exit(ContentManager contentManager, IServiceProvider serviceProvider);
        public IEnumerable<MenuElement> GetMenuElements();
        public Widget GetCurrentWidget();
        public Widget GetNextWidget();
        public Widget GetPreviousWidget();
    }
}