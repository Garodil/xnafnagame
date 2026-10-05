using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using model.Interfaces.Service;
using model.Interfaces.Ui;
using model.Models.Ui;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace scenes.Menus
{
    public abstract class Menu : IMenu
    {
        private readonly WeakReference<IMenuManager> _menuManager;
        private readonly WeakReference<IMenu> _parent;
        private readonly List<IMenu> _children = new List<IMenu>();
        private bool _initPassed = false;
        protected readonly List<MenuElement> _menuElements = [];
        protected readonly Container _container;

        protected Menu(IMenu? parent, IMenuManager menuManager)
        {
            _parent = new WeakReference<IMenu> (parent);
            _menuManager = new WeakReference<IMenuManager>(menuManager);
            _container = InitContainer();
        }

        public bool Enabled
        {
            get 
            {
                return _container.Enabled;
            }
            set 
            {
                _container.Enabled = value;
            }
        }

        protected abstract Container InitContainer();
        protected abstract void Init(ContentManager contentManager, IServiceProvider serviceProvider);
        protected abstract void OnEnter(ContentManager contentManager, IServiceProvider serviceProvider);
        protected abstract void OnExit(ContentManager contentManager, IServiceProvider serviceProvider);

        protected void EnqueueCommand(MenuCommand command)
        {
            if(_menuManager.TryGetTarget(out var menuManager) == false || command == null)
            {
                return;
            }
            menuManager.PushCommand(command);
        }

        public void Enter(ContentManager contentManager, IServiceProvider serviceProvider)
        {
            _container.Visible = true;
            _container.Enabled = true;
            OnEnter(contentManager, serviceProvider);
            
            if(_initPassed)
            {
                return;
            }
            Init(contentManager, serviceProvider);
            _initPassed = true;
        }

        public void Exit(ContentManager contentManager, IServiceProvider serviceProvider)
        {
            _container.Visible = false;
            _container.Enabled = false;
            OnExit(contentManager, serviceProvider);
        }

        public Widget GetWidget()
        {
            return _container;
        }
        public IMenu? GetParent()
        {
            return _parent.TryGetTarget(out var tg) ? tg : null;
        }


        public string GetName()
        {
            return this.GetType().Name;
        }

        public IMenu? GetChild(string name)
        {
            foreach(var menu in _children)
            {
                if(menu.GetName() == name)
                {
                    return menu;
                }
            }
            return null;
        }

        public void AddChild(IMenu menu)
        {
            _children.Add(menu);
        }

        public IEnumerable<MenuElement> GetMenuElements()
        {
            return _menuElements;
        }

        public Widget GetNextWidget()
        {
            if(_menuElements.Count == 0)
            {
                return null;
            }
            var index = _menuElements.FindIndex(0, _menuElements.Count, x => x.IsFocused);
            var nextIndex = Math.Clamp(index + 1, 0, _menuElements.Count - 1);
            return _menuElements[nextIndex].Widget;
        }

        public Widget GetPreviousWidget()
        {
            if(_menuElements.Count == 0)
            {
                return null;
            }
            var index = _menuElements.FindIndex(0, _menuElements.Count, x => x.IsFocused);
            var nextIndex = Math.Clamp(index - 1, 0, _menuElements.Count - 1);
            return _menuElements[nextIndex].Widget;
        }

        public Widget GetCurrentWidget()
        {
            var index = _menuElements.FindIndex(0, _menuElements.Count, x => x.IsFocused);
            var nextIndex = Math.Clamp(index, 0, _menuElements.Count - 1);
            return _menuElements[nextIndex].Widget;
        }
    }
}