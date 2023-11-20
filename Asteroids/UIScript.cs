using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class UIScript : SyncScript
    {
        private static UIElement RootElement;

        // Main menu elements
        public static StackPanel TitleScreenPanel;

        public static Grid HowToPlayPanel;

        public static StackPanel MapSelectPanel;

        public static StackPanel ShipSelectPanel;

        public static StackPanel ShipSelectMulti;

        // Game menu elements
        public static StackPanel TopBarPanel;
        public static ContentDecorator LivesDecorator;
        public static TextBlock LivesCountText;
        public static TextBlock ScoreText;
        public static TextBlock BombsCountText;

        public static StackPanel BossPanel;
        public static ContentDecorator BossHpDecorator;

        public static TextBlock TimerText;

        public static Grid GameOverPanel;
        public static TextBlock GameOverTitle;
        public static TextBlock GameOverInfo;
        public static Button BackToMenuButton;
        public static Button HideGameOverButton;

        public static ContentDecorator Overlay;

        public override void Start()
        {
            RootElement = Entity.Get<UIComponent>().Page.RootElement;
            InitializeUI();
        }

        public override void Update()
        {

        }

        public void InitializeUI()
        {
            var MenuGrid = RootElement.FindVisualChildOfType<Grid>("Menu");
            var GameGrid = RootElement.FindVisualChildOfType<Grid>("Game");

            MenuGrid.Visibility = Visibility.Visible;
            GameGrid.Visibility = Visibility.Collapsed;

            // Main menu elements
            TitleScreenPanel = MenuGrid.FindVisualChildOfType<StackPanel>("TitleScreen");
            HowToPlayPanel = MenuGrid.FindVisualChildOfType<Grid>("HowToPlay");
            MapSelectPanel = MenuGrid.FindVisualChildOfType<StackPanel>("MapSelect");
            ShipSelectPanel = MenuGrid.FindVisualChildOfType<StackPanel>("ShipSelect");
            ShipSelectMulti = MenuGrid.FindVisualChildOfType<StackPanel>("ShipSelectMulti");

            TitleScreenPanel.Visibility = Visibility.Visible;
            HowToPlayPanel.Visibility = Visibility.Collapsed;
            MapSelectPanel.Visibility = Visibility.Collapsed;
            ShipSelectPanel.Visibility = Visibility.Collapsed;

            // Game menu elements
            TopBarPanel = GameGrid.FindVisualChildOfType<StackPanel>("TopBar");
            TimerText = GameGrid.FindVisualChildOfType<TextBlock>("timer");
            GameOverPanel = GameGrid.FindVisualChildOfType<Grid>("GameOver");
            Overlay = GameGrid.FindVisualChildOfType<ContentDecorator>("overlay");

            GameOverPanel.Visibility = Visibility.Collapsed;

            InitializeTitleScreen();
            InitializeHowToPlay();
            InitializeMapSelect();
            InitializeShipSelect();

            InitializeTopBar();
            InitializeGameOver();

            RefreshUI();
        }

        private void InitializeTitleScreen()
        {
            var buttonsStack = TitleScreenPanel.FindVisualChildOfType<Grid>("Main").FindVisualChildOfType<StackPanel>("Buttons");

            var playButton = buttonsStack.FindVisualChildOfType<Button>("play");
            var howToPlayButton = buttonsStack.FindVisualChildOfType<Button>("howtoplay");
            var quitButton = buttonsStack.FindVisualChildOfType<Button>("quit");

            playButton.Click += delegate
            {
                SoundScript.PlayUISound();
                TitleScreenPanel.Visibility = Visibility.Collapsed;
                MapSelectPanel.Visibility = Visibility.Visible;
            };

            howToPlayButton.Click += delegate
            {
                SoundScript.PlayUISound();
                TitleScreenPanel.Visibility = Visibility.Collapsed;
                HowToPlayPanel.Visibility = Visibility.Visible;
            };

            quitButton.Click += delegate
            {
                ((Game)Game).Exit();
            };
        }

        private void InitializeHowToPlay()
        {
            var backButton = HowToPlayPanel.FindVisualChildOfType<StackPanel>("BasicControls").FindVisualChildOfType<Button>("back");

            backButton.Click += delegate
            {
                SoundScript.PlayUISound();
                TitleScreenPanel.Visibility = Visibility.Visible;
                HowToPlayPanel.Visibility = Visibility.Collapsed;
            };
        }

        private void InitializeMapSelect()
        {
            var buttons = MapSelectPanel.FindVisualChildOfType<UniformGrid>("StageButtons").Children;
            var arcadeModeButton = MapSelectPanel.FindVisualChildOfType<StackPanel>("ArcadeMode").FindVisualChildOfType<ToggleButton>("arcade");
            var backButton = MapSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("back");
            var nextButton = MapSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("next");

            // Hide until a map is selected
            nextButton.Visibility = Visibility.Hidden;

            arcadeModeButton.State = ToggleState.UnChecked;
            MainScript.isArcadeMode = false; // manually change

            foreach (var element in buttons)
            {
                var button = element as ToggleButton;
                if(button != null)
                {
                    button.State = ToggleState.UnChecked;

                    button.Click += delegate
                    {
                        SoundScript.PlayUISound();

                        // Put scaling of all unsued images to 0, except the one that is selected
                        foreach (var stageImage in MainScript.StageImages.Values) {
                            if (stageImage.Name.Equals(button.Name))
                                stageImage.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                            else
                                stageImage.Transform.Scale = new Vector3(0.0f, 0.0f, 0.0f);
                        }

                        foreach(var element2 in buttons)
                        {
                            var button2 = element2 as ToggleButton;
                            if(button2 != null)
                            {
                                var dimDecorator = ((Grid)button2.Content).FindVisualChildOfType<ContentDecorator>("dim");

                                // If clicked button, then don't dim the image. This is done to indicate which map is selected. Check page structure.
                                if (button2 == button)
                                    dimDecorator.BackgroundColor = new Color(0f, 0f, 0f, 0f);
                                // Otherwise dim the button to indicate that it's not selected
                                else
                                    dimDecorator.BackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.75f);
                            }
                        }

                        MainScript.stage = Enum.Parse<MainScript.Stage>(button.Name);
                        CampaignModeLogic.Boss = EntityPooling.bosses[button.Name];

                        nextButton.Visibility = Visibility.Visible;
                    };
                }
            }

            arcadeModeButton.Click += delegate
            {
                SoundScript.PlayUISound();

                if (arcadeModeButton.State == ToggleState.Checked)
                    MainScript.isArcadeMode = true;
                else
                    MainScript.isArcadeMode = false;
            };

            backButton.Click += delegate
            {
                SoundScript.PlayUISound();
                MapSelectPanel.Visibility = Visibility.Collapsed;
                TitleScreenPanel.Visibility = Visibility.Visible;
            };

            nextButton.Click += delegate
            {
                SoundScript.PlayUISound();
                MapSelectPanel.Visibility = Visibility.Collapsed;
                ShipSelectPanel.Visibility = Visibility.Visible;
            };

        }

        private void InitializeShipSelect()
        {
            var buttons = ShipSelectPanel.FindVisualChildOfType<StackPanel>("ShipSelection").Children;
            var backButton = ShipSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("back");
            var startButton = ShipSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("start");

            // Hide until a spaceship is selected
            startButton.Visibility = Visibility.Hidden;

            foreach (var element in buttons)
            {
                var button = element as ToggleButton;
                if(button != null)
                {
                    button.State = ToggleState.UnChecked;

                    button.Click += delegate
                    {
                        SoundScript.PlayUISound();

                        // Uncheck all buttons
                        foreach (var element2 in buttons)
                        {
                            var button2 = element2 as ToggleButton;
                            if (button2 != null)
                                button2.State = ToggleState.UnChecked;
                        }

                        button.State = ToggleState.Checked;

                        // Set corresponding ship
                        SinglePlayerLogic.spaceShip = EntityPooling.spaceShips[button.Name];

                        startButton.Visibility = Visibility.Visible;
                    };
                }
            }

            backButton.Click += delegate
            {
                SoundScript.PlayUISound();
                ShipSelectPanel.Visibility = Visibility.Collapsed;
                MapSelectPanel.Visibility = Visibility.Visible;
            };

            startButton.Click += delegate
            {
                SoundScript.PlayUISound();

                RootElement.FindVisualChildOfType<Grid>("Menu").Visibility = Visibility.Collapsed;
                RootElement.FindVisualChildOfType<Grid>("Game").Visibility = Visibility.Visible;

                // Small detail (change ship lives icon to currently selected ship)
                ((SpriteFromSheet)LivesDecorator.BackgroundImage).CurrentFrame = Utils.GetSpriteFrameFromShipName();

                // Avoid instantly starting game, do it through a countdown script instead
                var countdown = new CountdownScript();
                countdown.maxTimer = 3.0;

                Entity.Add(countdown);
            };
        }

        private void InitializeShipSelectMulti()
        {
            var backButton = ShipSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("back");
            var startButton = ShipSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("start");
            var shipSelectionAllPanel = ShipSelectMulti.FindVisualChildOfType<UniformGrid>("ShipSelectionAll");

            int i = 0;
            foreach(var ShipSelection in shipSelectionAllPanel.Children)
            {
                var buttons = ShipSelectPanel.FindVisualChildOfType<StackPanel>("ShipSelection").Children;

                foreach (var element in buttons)
                {
                    var button = element as ToggleButton;
                    if (button != null)
                    {
                        button.State = ToggleState.UnChecked;

                        button.Click += delegate
                        {
                            SoundScript.PlayUISound();

                            // Uncheck all buttons
                            foreach (var element2 in buttons)
                            {
                                var button2 = element2 as ToggleButton;
                                if (button2 != null)
                                    button2.State = ToggleState.UnChecked;
                            }

                            button.State = ToggleState.Checked;

                            // Set corresponding ship
                            // SinglePlayerLogic.spaceShip = EntityPooling.spaceShips[button.Name];
                            MultiplayerVSLogic.SpaceShipSelection[i] = EntityPooling.multiplayerVsSpaceShips[button.Name];

                            startButton.Visibility = Visibility.Visible;
                        };
                    }
                }

                i++;
            }

            startButton.Click += delegate
            {
                SoundScript.PlayUISound();

                RootElement.FindVisualChildOfType<Grid>("Menu").Visibility = Visibility.Collapsed;
                RootElement.FindVisualChildOfType<Grid>("Game").Visibility = Visibility.Visible;

                // Small detail (change ship lives icon to currently selected ship)
                // ((SpriteFromSheet)LivesDecorator.BackgroundImage).CurrentFrame = Utils.GetSpriteFrameFromShipName();

                // Avoid instantly starting game, do it through a countdown script instead
                var countdown = new CountdownScript();
                countdown.maxTimer = 3.0;

                Entity.Add(countdown);
            };
        }

        private void InitializeTopBar()
        {
            LivesCountText = TopBarPanel.FindVisualChildOfType<UniformGrid>("Stats").FindVisualChildOfType<StackPanel>("Lives").FindVisualChildOfType<TextBlock>("count");
            ScoreText = TopBarPanel.FindVisualChildOfType<UniformGrid>("Stats").FindVisualChildOfType<StackPanel>("Score").FindVisualChildOfType<TextBlock>("count");
            BombsCountText = TopBarPanel.FindVisualChildOfType<UniformGrid>("Stats").FindVisualChildOfType<StackPanel>("Bombs").FindVisualChildOfType<TextBlock>("count");
            LivesDecorator = TopBarPanel.FindVisualChildOfType<UniformGrid>("Stats").FindVisualChildOfType<StackPanel>("Lives").FindVisualChildOfType<ContentDecorator>("imageContainer");

            BossPanel = TopBarPanel.FindVisualChildOfType<StackPanel>("Boss");
            BossPanel.Visibility = Visibility.Hidden;

            BossHpDecorator = (ContentDecorator) ((ContentDecorator)BossPanel.FindVisualChildOfType<ContentDecorator>("HpBackground").Content).Content;
        }

        private void InitializeGameOver()
        {
            GameOverTitle = GameOverPanel.FindVisualChildOfType<TextBlock>("title");
            GameOverInfo = GameOverPanel.FindVisualChildOfType<TextBlock>("info");
            BackToMenuButton = GameOverPanel.FindVisualChildOfType<Button>("back");
            HideGameOverButton = GameOverPanel.FindVisualChildOfType<Button>("hide");

            BackToMenuButton.Click += delegate
            {
                SoundScript.PlayUISound();

                Entity.Remove<SinglePlayerLogic>();

                if (SinglePlayerLogic.spaceShip.Scene != null)
                    SinglePlayerLogic.spaceShip.Scene.Entities.Remove(SinglePlayerLogic.spaceShip);

                // Remove all countdown scripts
                while (Entity.Get<CountdownScript>() != null)
                    Entity.Remove<CountdownScript>();

                SinglePlayerLogic.spaceShip.Dispose();
                MainScript.enemiesScene.Entities.Clear();
                MainScript.projectilesScene.Entities.Clear();
                MainScript.particlesScene.Entities.Clear();

                RefreshUI();
            };

            HideGameOverButton.Click += delegate
            {
                SoundScript.PlayUISound();

                GameOverPanel.Visibility = Visibility.Collapsed;
            };
        }

        public void RefreshUI()
        {
            // Main panels ////////////////////////////////////////
            RootElement.FindVisualChildOfType<Grid>("Menu").Visibility = Visibility.Visible;
            RootElement.FindVisualChildOfType<Grid>("Game").Visibility = Visibility.Collapsed;

            TitleScreenPanel.Visibility = Visibility.Visible;
            HowToPlayPanel.Visibility = Visibility.Collapsed;
            MapSelectPanel.Visibility = Visibility.Collapsed;
            ShipSelectPanel.Visibility = Visibility.Collapsed;
            ShipSelectMulti.Visibility = Visibility.Collapsed;

            GameOverPanel.Visibility = Visibility.Collapsed;


            // Map selection //////////////////////////////////////////
            // Hide until a map is selected
            MapSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("next").Visibility = Visibility.Hidden;

            var buttons = MapSelectPanel.FindVisualChildOfType<UniformGrid>("StageButtons").Children;
            foreach (var element in buttons)
            {
                var button = element as ToggleButton;
                if (button != null)
                {
                    var dimDecorator = ((Grid)button.Content).FindVisualChildOfType<ContentDecorator>("dim");

                    // Reapply black dim on map buttons
                    dimDecorator.BackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.75f);
                }
            }

            MapSelectPanel.FindVisualChildOfType<StackPanel>("ArcadeMode").FindVisualChildOfType<ToggleButton>("arcade").State = ToggleState.UnChecked;
            MainScript.isArcadeMode = false;


            // Ship selection //////////////////////////////////////////
            // Hide until a spaceship is selected
            ShipSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("start").Visibility = Visibility.Hidden;

            buttons = ShipSelectPanel.FindVisualChildOfType<StackPanel>("ShipSelection").Children;
            foreach (var element in buttons)
            {
                var button = element as ToggleButton;
                if (button != null)
                    button.State = ToggleState.UnChecked;
            }

            // Game menu ////////////////////////////
            BossPanel.Visibility = Visibility.Hidden;
            GameOverPanel.Visibility = Visibility.Collapsed;
            TimerText.Visibility = Visibility.Hidden;

            Overlay.BackgroundColor = new Color(0f, 0f, 0f, 0f);
            Overlay.Visibility = Visibility.Collapsed;
        }
    }
}
