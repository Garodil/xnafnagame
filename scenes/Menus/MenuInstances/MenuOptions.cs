using Microsoft.Xna.Framework.Content;
using Myra.Graphics2D.UI;
using model.Interfaces.Service;
using scenes.Manager;
using model.Interfaces.Ui;
using model.Models.Ui.Elements;
using Myra.Graphics2D.Brushes;
using System.Drawing;
using System.Globalization;
using model.Models.Configs;
using model.Models.Structs;
using System.Xml.Linq;
using model.Models.Configs.Enum;

namespace scenes.Menus {
    public class MenuOptions : Menu
    {
        private const int DEFAULT_VOLUME_SLIDER = 70;
        private const float DEFAULT_VOLUME_VALUE = 0.1f;
        private GameOptions options;
        private IConfigManager _configManager;

        public MenuOptions(IMenu parent, IMenuManager menuManager, GameOptions opt, IConfigManager cms) : base(parent, menuManager)
        {
            options = opt;
            _configManager = cms;
        }
        
        protected override void Init(ContentManager contentManager, IServiceProvider serviceProvider)
        {
            _container.Widgets.Add(
                new Label{Text = "le Options"}
            );
            (string, Widget)[] panels = 
            [
                GenerateGamePanel(),
                GenerateAudioPanel(options.AudioOptions),
                GenerateVideoPanel(options.VideoOptions),
                GenerateOtherPanel()
            ];
            var tabControl = CreateTabs(panels);
            _container.Widgets.Add(tabControl);
            Button saveButton = new Button
                                    {
                                        Content = new Label
                                        {
                                            Text = "Save Options"
                                        }
                                    };
            saveButton.Click += (sender, args) => SaveButtonClick(panels);
            _container.Widgets.Add(saveButton);
        }
        private void SaveButtonClick((string, Widget)[] panels)
        {
            var videoStackPanel = panels[2].Item2 as StackPanel;
            var videoComboView = videoStackPanel.Widgets[0] as ComboView;
            var videoItem = videoComboView.SelectedItem as Label;

            var audioStackPanel = panels[1].Item2 as StackPanel;

            var musicSliderStackPanel = audioStackPanel.Widgets[0] as HorizontalStackPanel;
            var musicSlider = musicSliderStackPanel.Widgets[0] as HorizontalSlider;
            var musikValue = musicSlider.Value / DEFAULT_VOLUME_SLIDER * DEFAULT_VOLUME_VALUE;

            var soundSliderStackPanel = audioStackPanel.Widgets[1] as HorizontalStackPanel;
            var soundSlider = soundSliderStackPanel.Widgets[0] as HorizontalSlider;
            var soundValue = soundSlider.Value / DEFAULT_VOLUME_SLIDER * DEFAULT_VOLUME_VALUE;

            var volumes = new  Dictionary<SoundChannelsEnum, float>
            {
                {SoundChannelsEnum.Main, 1}, //TODO create Main slider & connect this
                {SoundChannelsEnum.Music, musikValue},
                {SoundChannelsEnum.Sound, soundValue}
            };
            SaveOptions(options, _configManager, videoItem.Text, volumes);
        }

        private TabControl CreateTabs(IEnumerable<(string, Widget)> tabs)
        {
            TabControl tabControl = new TabControl()
            {
                CloseableTabs = false,
                AcceptsKeyboardFocus = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            foreach(var tab in tabs)
            {
                var tabItem = new TabItem(tab.Item1, null, null, tab.Item2);
                tabControl.Items.Add(tabItem);
            };

            return tabControl;
        }


        private (string, Widget) GenerateGamePanel()
        {
            VerticalStackPanel panel = new VerticalStackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            panel.Widgets.Add(new CheckBox()
            {
                Text = "Debug mode enabled"
            });
            return ("Game", panel);
        }

        private (string, Widget) GenerateVideoPanel(VideoOptions VideoOptions)
        {
            
            var panel = new VerticalStackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var resolutionView = new ComboView();
            foreach(var resolutionOption in VideoOptions.ResolutionDictionary.OrderBy(x => x.Key))
            {
                resolutionView.Widgets.Add(
                    new Label(){ Text =  $"{resolutionOption.Value.Width}x{resolutionOption.Value.Height}"}
                );
            }
            resolutionView.SelectedIndex = (int)VideoOptions.CurrentResolutionIndex ;
            panel.Widgets.Add(resolutionView);


            var sliderStackPanel = new HorizontalStackPanel();
            var pixelizeLabel = new Label();
            var pixelizeSlider = new HorizontalSlider()
            {
                Minimum = 0,
                Maximum = 2,
                Value = 1,
                Width = 400
            };
            pixelizeSlider.ValueChangedByUser += (s, ea) =>
            {
                if(s is not HorizontalSlider hs)
                {
                    return;
                }

                hs.Value = (int)(hs.Value);
                pixelizeLabel.Text = $"{hs.Value}";
            };
            pixelizeLabel.Text = $"{pixelizeSlider.Value}";
            sliderStackPanel.Widgets.Add(pixelizeSlider);
            sliderStackPanel.Widgets.Add(pixelizeLabel);
            panel.Widgets.Add(sliderStackPanel);

            return ("Video", panel);
        }

        private (string, Widget) GenerateAudioPanel(AudioOptions audioOptions)
        {
            var panel = new VerticalStackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var musicSliderStackPanel = new HorizontalStackPanel();
            var musicLabel = new Label();
            var musicValue = audioOptions.ChanelsVolume[SoundChannelsEnum.Music] / DEFAULT_VOLUME_VALUE * DEFAULT_VOLUME_SLIDER; //Делим значение на дефолтное, чтобы понять, насколько оно больше и домножаем на соответсвующее значение слайдера, чтобы найти нужное
            var musicSlider = new HorizontalSlider()    //0.1 = 70 0.05/0.1 = 0.5 в 2 раза меньше, поэтому слайдер будет 35
            {
                Minimum = 0,
                Maximum = 100,
                Value = musicValue,
                Width = 400
            };
            musicSlider.ValueChangedByUser += (s, ea) =>
            {
                if(s is not HorizontalSlider hs)
                {
                    return;
                }

                hs.Value = (int)(hs.Value);
                musicLabel.Text = $"{hs.Value}";
            };
            musicLabel.Text = $"{musicSlider.Value}";
            musicSliderStackPanel.Widgets.Add(musicSlider);
            musicSliderStackPanel.Widgets.Add(musicLabel);
            panel.Widgets.Add(musicSliderStackPanel);

            var soundSliderStackPanel = new HorizontalStackPanel();
            var soundLabel = new Label();
            var soundValue = audioOptions.ChanelsVolume[SoundChannelsEnum.Sound] / DEFAULT_VOLUME_VALUE * DEFAULT_VOLUME_SLIDER;
            var soundSlider = new HorizontalSlider()
            {
                Minimum = 0,
                Maximum = 100,
                Value = soundValue,
                Width = 400
            };
            soundSlider.ValueChangedByUser += (s, ea) =>
            {
                if(s is not HorizontalSlider hs)
                {
                    return;
                }

                hs.Value = (int)(hs.Value);
                soundLabel.Text = $"{hs.Value}";
            };
            soundLabel.Text = $"{soundSlider.Value}";
            soundSliderStackPanel.Widgets.Add(soundSlider);
            soundSliderStackPanel.Widgets.Add(soundLabel);
            panel.Widgets.Add(soundSliderStackPanel);

            return ("Audio", panel);
        }

        private (string, Widget) GenerateOtherPanel()
        {
            var panel = new VerticalStackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Widgets.Add(new Label() { Text = ":)" });

            return ("Other", panel);
        }

        protected override void OnEnter(ContentManager contentManager, IServiceProvider serviceProvider)
        {
        }

        protected override void OnExit(ContentManager contentManager, IServiceProvider serviceProvider)
        {
        }

        protected override Container InitContainer()
        {
            return new VerticalStackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }
        private void SaveOptions( GameOptions oldOptions, IConfigManager configManager, string resolution, Dictionary<SoundChannelsEnum, float> channelsVolume)
        {
            Console.WriteLine("Start function SaveOptions");
            var options = new TemporalOptions();
            options.Resolution = options.ResolutionArray[resolution];
            options.ChanelsVolume = channelsVolume;
            GameOptions opt = configManager.ConvertToGameOptions(options);
            configManager.ChangeConfig(oldOptions, opt);
            Allert(oldOptions, configManager);

        }
        public void Allert(GameOptions oldOptions, IConfigManager configManager)
        {
            bool ans = true;
            if(!ans)
            {
                //cms.ChangeConfig(oldOptions);
                return;
            }
        }
    }
}