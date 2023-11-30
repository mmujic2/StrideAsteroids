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

        public static Grid MultiplayerPanel;

        public static Grid HowToPlayPanel;

        public static StackPanel MapSelectPanel;
        public static StackPanel OptionStack;

        public static StackPanel ShipSelectPanel;

        public static StackPanel ShipSelectMultiPanel;
        public static ToggleButton player3Enable;
        public static ToggleButton player4Enable;

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

        // Multiplayer
        public static UniformGrid TopBarMultiplayerPanel;
        public static UIElementCollection PlayerInfos;

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
            MultiplayerPanel = MenuGrid.FindVisualChildOfType<Grid>("Multiplayer");
            HowToPlayPanel = MenuGrid.FindVisualChildOfType<Grid>("HowToPlay");
            MapSelectPanel = MenuGrid.FindVisualChildOfType<StackPanel>("MapSelect");
            OptionStack = MapSelectPanel.FindVisualChildOfType<StackPanel>("Option");
            ShipSelectPanel = MenuGrid.FindVisualChildOfType<StackPanel>("ShipSelect");
            ShipSelectMultiPanel = MenuGrid.FindVisualChildOfType<StackPanel>("ShipSelectMulti");

            TitleScreenPanel.Visibility = Visibility.Visible;
            HowToPlayPanel.Visibility = Visibility.Collapsed;
            MapSelectPanel.Visibility = Visibility.Collapsed;
            ShipSelectPanel.Visibility = Visibility.Collapsed;

            // Game menu elements
            TopBarPanel = GameGrid.FindVisualChildOfType<StackPanel>("TopBar");
            TopBarMultiplayerPanel = (UniformGrid) GameGrid.FindVisualChildOfType<ContentDecorator>("TopBarMultiplayer").Content;
            TimerText = GameGrid.FindVisualChildOfType<TextBlock>("timer");
            GameOverPanel = GameGrid.FindVisualChildOfType<Grid>("GameOver");
            Overlay = GameGrid.FindVisualChildOfType<ContentDecorator>("overlay");

            GameOverPanel.Visibility = Visibility.Collapsed;

            InitializeTitleScreen();
            InitializeMultiplayer();
            InitializeHowToPlay();
            InitializeMapSelect();
            InitializeShipSelect();
            InitializeShipSelectMulti();

            InitializeTopBar();
            InitializeTopBarMultiplayer();
            InitializeGameOver();

            RefreshUI();
        }

        private void InitializeTitleScreen()
        {
            var buttonsStack = TitleScreenPanel.FindVisualChildOfType<Grid>("Main").FindVisualChildOfType<StackPanel>("Buttons");

            var playButton = buttonsStack.FindVisualChildOfType<Button>("play");
            var multiPlayButton = buttonsStack.FindVisualChildOfType<Button>("multiplay");
            var howToPlayButton = buttonsStack.FindVisualChildOfType<Button>("howtoplay");
            var quitButton = buttonsStack.FindVisualChildOfType<Button>("quit");

            playButton.Click += delegate
            {
                SoundScript.PlayUISound();
                OptionStack.FindVisualChildOfType<TextBlock>("description").Text = "Arcade Mode";

                TitleScreenPanel.Visibility = Visibility.Collapsed;
                MapSelectPanel.Visibility = Visibility.Visible;
            };

            howToPlayButton.Click += delegate
            {
                SoundScript.PlayUISound();
                TitleScreenPanel.Visibility = Visibility.Collapsed;
                HowToPlayPanel.Visibility = Visibility.Visible;
            };

            multiPlayButton.Click += delegate
            {
                SoundScript.PlayUISound();
                TitleScreenPanel.Visibility = Visibility.Collapsed;
                MultiplayerPanel.Visibility = Visibility.Visible;
            };

            quitButton.Click += delegate
            {
                ((Game)Game).Exit();
            };
        }

        private void InitializeMultiplayer()
        {
            var coopButton = MultiplayerPanel.FindVisualChildOfType<StackPanel>("Buttons").FindVisualChildOfType<Button>("coop");
            var versusButton = MultiplayerPanel.FindVisualChildOfType<StackPanel>("Buttons").FindVisualChildOfType<Button>("versus");
            var backButton = MultiplayerPanel.FindVisualChildOfType<StackPanel>("Buttons").FindVisualChildOfType<Button>("back");

            coopButton.Click += delegate
            {
                SoundScript.PlayUISound();
                MainScript.mode = MainScript.Mode.MultiPlayerCampaign;
                OptionStack.FindVisualChildOfType<TextBlock>("description").Text = "Friendly Fire";

                // Hide bombs as they are not used in coop
                BombsCountText.Parent.Visibility = Visibility.Collapsed;

                // Hide 3rd and fourth player
                var shipSelectionAllPanel = ShipSelectMultiPanel.FindVisualChildOfType<UniformGrid>("ShipSelectionAll").Children;
                shipSelectionAllPanel[2].Visibility = Visibility.Collapsed;
                shipSelectionAllPanel[3].Visibility = Visibility.Collapsed;

                ShipSelectMultiPanel.FindVisualChildOfType<UniformGrid>("ShipSelectionAll").Rows = 1;

                MultiplayerPanel.Visibility = Visibility.Collapsed;
                MapSelectPanel.Visibility = Visibility.Visible;
            };

            versusButton.Click += delegate
            {
                SoundScript.PlayUISound();
                MainScript.mode = MainScript.Mode.MultiPlayerVersus;
                OptionStack.FindVisualChildOfType<TextBlock>("description").Text = "Spawn Asteroids";

                // Enable 3rd and 4th player
                var shipSelectionAllPanel = ShipSelectMultiPanel.FindVisualChildOfType<UniformGrid>("ShipSelectionAll").Children;
                shipSelectionAllPanel[2].Visibility = Visibility.Visible;
                shipSelectionAllPanel[3].Visibility = Visibility.Visible;

                ShipSelectMultiPanel.FindVisualChildOfType<UniformGrid>("ShipSelectionAll").Rows = 2;

                MultiplayerPanel.Visibility = Visibility.Collapsed;
                MapSelectPanel.Visibility = Visibility.Visible;
            };

            backButton.Click += delegate
            {
                SoundScript.PlayUISound();
                MultiplayerPanel.Visibility = Visibility.Collapsed;
                TitleScreenPanel.Visibility = Visibility.Visible;

                RefreshUI();
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

            ToggleButton optionButton = OptionStack.FindVisualChildOfType<ToggleButton>("option");

            var backButton = MapSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("back");
            var nextButton = MapSelectPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("next");

            // Hide until a map is selected
            nextButton.Visibility = Visibility.Hidden;

            optionButton.State = ToggleState.UnChecked;
            MainScript.mode = MainScript.Mode.SinglePlayerCampaign;
            // MainScript.isArcadeMode = false; // manually change

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

            optionButton.Click += delegate
            {
                SoundScript.PlayUISound();

                if(MainScript.mode == MainScript.Mode.SinglePlayerCampaign || MainScript.mode == MainScript.Mode.SinglePlayerArcade)
                {
                    if (optionButton.State == ToggleState.Checked)
                        MainScript.mode = MainScript.Mode.SinglePlayerArcade;
                    else
                        MainScript.mode = MainScript.Mode.SinglePlayerCampaign;
                }
                else if(MainScript.mode == MainScript.Mode.MultiPlayerVersus)
                    MultiplayerVSLogic.spawnAsteroids = optionButton.State == ToggleState.Checked;
                else if (MainScript.mode == MainScript.Mode.MultiPlayerCampaign)
                    MultiplayerCoopLogic.friendlyFire = optionButton.State == ToggleState.Checked;
            };

            backButton.Click += delegate
            {
                SoundScript.PlayUISound();
                MapSelectPanel.Visibility = Visibility.Collapsed;

                if (MainScript.mode == MainScript.Mode.SinglePlayerCampaign || MainScript.mode == MainScript.Mode.SinglePlayerArcade)
                {
                    RefreshUI();
                    TitleScreenPanel.Visibility = Visibility.Visible;
                } 
                else
                    MultiplayerPanel.Visibility = Visibility.Visible;

            };

            nextButton.Click += delegate
            {
                SoundScript.PlayUISound();
                MapSelectPanel.Visibility = Visibility.Collapsed;

                if(MainScript.mode == MainScript.Mode.SinglePlayerCampaign || MainScript.mode == MainScript.Mode.SinglePlayerArcade)
                    ShipSelectPanel.Visibility = Visibility.Visible;
                else 
                    ShipSelectMultiPanel.Visibility = Visibility.Visible;
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
                        ArcadeModeLogic.spaceShip = EntityPooling.spaceShips[button.Name];

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
            MultiplayerVSLogic.SpaceShipSelection = new();
            MultiplayerVSLogic.SpaceShipSelection.Add(null);
            MultiplayerVSLogic.SpaceShipSelection.Add(null);
            MultiplayerVSLogic.SpaceShipSelection.Add(null);
            MultiplayerVSLogic.SpaceShipSelection.Add(null);

            // Coop only has 2 players
            MultiplayerCoopLogic.SpaceShipSelection = new();
            MultiplayerCoopLogic.SpaceShipSelection.Add(null);
            MultiplayerCoopLogic.SpaceShipSelection.Add(null);

            var backButton = ShipSelectMultiPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("back");
            var startButton = ShipSelectMultiPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("start");
            var shipSelectionAllPanel = ShipSelectMultiPanel.FindVisualChildOfType<UniformGrid>("ShipSelectionAll");

            int i = 0;
            foreach(StackPanel ShipSelection in shipSelectionAllPanel.Children)
            {
                UIElementCollection buttons;
                int index = i;

                if(i > 1)
                {
                    // Player 3 and 4 are a bit different
                    buttons = ShipSelection.FindVisualChildOfType<StackPanel>("ShipSelection").Children;

                    if(i == 2)
                    {
                        player3Enable = ShipSelection.FindVisualChildOfType<StackPanel>("Option").FindVisualChildOfType<ToggleButton>();

                        player3Enable.Click += delegate
                        {
                            SoundScript.PlayUISound();

                            if (player3Enable.State == ToggleState.Checked)
                            {
                                ShipSelection.FindVisualChildOfType<StackPanel>("ShipSelection").Visibility = Visibility.Visible;
                                PlayerInfos[index].Visibility = Visibility.Visible;
                            } 
                            else
                            {
                                ShipSelection.FindVisualChildOfType<StackPanel>("ShipSelection").Visibility = Visibility.Hidden;
                                PlayerInfos[index].Visibility = Visibility.Hidden;
                            }

                            if (CheckIfAllPlayersReady())
                                startButton.Visibility = Visibility.Visible;
                            else
                                startButton.Visibility = Visibility.Hidden;
                        };
                    }
                    else if(i == 3)
                    {
                        player4Enable = ShipSelection.FindVisualChildOfType<StackPanel>("Option").FindVisualChildOfType<ToggleButton>();

                        player4Enable.Click += delegate
                        {
                            SoundScript.PlayUISound();

                            if (player4Enable.State == ToggleState.Checked)
                            {
                                ShipSelection.FindVisualChildOfType<StackPanel>("ShipSelection").Visibility = Visibility.Visible;
                                PlayerInfos[index].Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ShipSelection.FindVisualChildOfType<StackPanel>("ShipSelection").Visibility = Visibility.Hidden;
                                PlayerInfos[index].Visibility = Visibility.Hidden;
                            }
                                

                            if (CheckIfAllPlayersReady())
                                startButton.Visibility = Visibility.Visible;
                            else
                                startButton.Visibility = Visibility.Hidden;
                        };
                    }  
                }
                else
                {
                    buttons = ShipSelection.Children;
                }

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
                            MultiplayerVSLogic.SpaceShipSelection[index] = EntityPooling.multiplayerVsSpaceShips[button.Name];
                            if(index < 2)
                            {
                                MultiplayerCoopLogic.SpaceShipSelection[index] = EntityPooling.multiplayerCoopSpaceeShips[button.Name];
                                ((SpriteFromSheet)LivesDecorator.BackgroundImage).CurrentFrame = Utils.GetSpriteFrameFromShipName(MultiplayerCoopLogic.SpaceShipSelection[index].Name);
                            }
                            
                            ((SpriteFromSheet)PlayerInfos[index].FindVisualChildOfType<ContentDecorator>().BackgroundImage).CurrentFrame = Utils.GetSpriteFrameFromShipName(MultiplayerVSLogic.SpaceShipSelection[index].Name);

                            if (CheckIfAllPlayersReady())
                                startButton.Visibility = Visibility.Visible;
                        };
                    }

                    // Manually change font size of all text because editor and in game text size do not match
                    var infoStack = (StackPanel)button.Content;

                    infoStack.FindVisualChildOfType<StackPanel>("Damage").FindVisualChildOfType<TextBlock>().TextSize = 11;
                    infoStack.FindVisualChildOfType<StackPanel>("Agility").FindVisualChildOfType<TextBlock>().TextSize = 11;
                    infoStack.FindVisualChildOfType<StackPanel>("Projectiles").FindVisualChildOfType<TextBlock>().TextSize = 11;
                }

                i++;
            }

            startButton.Click += delegate
            {
                SoundScript.PlayUISound();

                RootElement.FindVisualChildOfType<Grid>("Menu").Visibility = Visibility.Collapsed;
                RootElement.FindVisualChildOfType<Grid>("Game").Visibility = Visibility.Visible;

                if(MainScript.mode == MainScript.Mode.MultiPlayerVersus)
                {
                    TopBarPanel.Visibility = Visibility.Collapsed;
                    TopBarMultiplayerPanel.Parent.Visibility = Visibility.Visible;
                }

                // Avoid instantly starting game, do it through a countdown script instead
                var countdown = new CountdownScript();
                countdown.maxTimer = 3.0;

                Entity.Add(countdown);
            };

            backButton.Click += delegate
            {
                SoundScript.PlayUISound();
                ShipSelectMultiPanel.Visibility = Visibility.Collapsed;
                MapSelectPanel.Visibility = Visibility.Visible;
            };
        }

        private bool CheckIfAllPlayersReady()
        {
            var selectionPanels = ShipSelectMultiPanel.FindVisualChildOfType<UniformGrid>().Children;
            int i = 0;
            foreach (StackPanel selectionPanel in selectionPanels)
            {
                UIElementCollection selectionButtons;

                if (i > 1)
                {
                    if (i == 2 && player3Enable.State == ToggleState.Checked)
                        selectionButtons = selectionPanel.FindVisualChildOfType<StackPanel>("ShipSelection").Children;
                    else if (i == 3 && player4Enable.State == ToggleState.Checked)
                        selectionButtons = selectionPanel.FindVisualChildOfType<StackPanel>("ShipSelection").Children;
                    else
                    {
                        i++;
                        continue;
                    }
                }
                else
                    selectionButtons = selectionPanel.Children;
                

                bool currentPlayerReady = false;
                foreach (var element in selectionButtons)
                {
                    var button = element as ToggleButton;
                    if (button != null && button.State == ToggleState.Checked)
                    {
                        currentPlayerReady = true;
                        break;
                    }
                }

                if (!currentPlayerReady)
                    return false;

                i++;
            }

            return true;
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

        private void InitializeTopBarMultiplayer()
        {
            PlayerInfos = new();
            PlayerInfos.AddRange(TopBarMultiplayerPanel.Children);
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

                Entity.Remove<GameLogic>();

                if(MainScript.mode == MainScript.Mode.SinglePlayerCampaign)
                {
                    if (SinglePlayerLogic.spaceShip.Scene != null)
                        SinglePlayerLogic.spaceShip.Scene = null;

                    SinglePlayerLogic.spaceShip.Dispose();
                }

                // Remove all countdown scripts
                while (Entity.Get<CountdownScript>() != null)
                    Entity.Remove<CountdownScript>();

                
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
            ShipSelectMultiPanel.Visibility = Visibility.Collapsed;

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

            OptionStack.FindVisualChildOfType<ToggleButton>().State = ToggleState.UnChecked;
            MainScript.mode = MainScript.Mode.SinglePlayerCampaign;
            // MainScript.isArcadeMode = false;


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

            // Ship multiplayer selection
            var selectionPanels = ShipSelectMultiPanel.FindVisualChildOfType<UniformGrid>().Children;
            int i = 0;
            foreach(StackPanel selectionPanel in selectionPanels)
            {
                UIElementCollection selectionButtons = null;
                if (i > 1)
                {
                    selectionPanel.FindVisualChildOfType<StackPanel>("ShipSelection").Visibility = Visibility.Hidden;
                    selectionButtons = selectionPanel.FindVisualChildOfType<StackPanel>("ShipSelection").Children;
                }
                else
                    selectionButtons = selectionPanel.Children;

                foreach(var element in selectionButtons)
                {
                    var button = element as ToggleButton;
                    if (button != null)
                        button.State = ToggleState.UnChecked;
                }

                i++;
            }

            // Space ship selection panels hidden in loop above
            player3Enable.State = ToggleState.UnChecked;
            PlayerInfos[2].Visibility = Visibility.Hidden;
            player4Enable.State = ToggleState.UnChecked;
            PlayerInfos[3].Visibility = Visibility.Hidden;

            ShipSelectMultiPanel.FindVisualChildOfType<StackPanel>("OptionButtons").FindVisualChildOfType<Button>("start").Visibility = Visibility.Hidden;

            // Game menu ////////////////////////////
            TopBarPanel.Visibility = Visibility.Visible;
            TopBarMultiplayerPanel.Parent.Visibility = Visibility.Collapsed;

            BombsCountText.Parent.Visibility = Visibility.Visible;

            BossPanel.Visibility = Visibility.Hidden;
            GameOverPanel.Visibility = Visibility.Collapsed;
            TimerText.Visibility = Visibility.Hidden;

            Overlay.BackgroundColor = new Color(0f, 0f, 0f, 0f);
            Overlay.Visibility = Visibility.Collapsed;
        }
    }
}
