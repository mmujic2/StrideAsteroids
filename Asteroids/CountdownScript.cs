using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    // Component that handles some wait times
    public class CountdownScript : SyncScript
    {
        public enum CountdownType
        {
            GameStart,
            BossStart,
            BossDead,
            spaceShipDead
        }

        public CountdownType countdownType;
        public double maxTimer;
        private double currentTimer;

        public bool isBossTime;

        public override void Update()
        {
            currentTimer += Game.UpdateTime.Elapsed.TotalSeconds;

            if(countdownType == CountdownType.GameStart)
            {
                UIScript.TimerText.Visibility = Visibility.Visible;
                // + 1.0 to show 3 2 1 instead of 2 1 0
                var timer = (int) (maxTimer - currentTimer + 1.0);

                // Fastest way to implement change that I could think of
                try
                {
                    if (timer != int.Parse(UIScript.TimerText.Text))
                    {
                        Utils.PlaySound(SoundScript.Timer, 0.5f);
                    }
                }
                catch (Exception)
                {
                    Utils.PlaySound(SoundScript.Timer, 0.5f);
                }
                

                UIScript.TimerText.Text = ((int) timer).ToString();
            }
            else if(countdownType == CountdownType.BossStart)
            {
                UIScript.TimerText.TextSize = 40;
                UIScript.TimerText.Visibility = Visibility.Visible;
                UIScript.TimerText.Text = "Warning:\nLarge enemy approaching";

                UIScript.BossPanel.Visibility = Visibility.Visible;
                // Fill up HP bar as countdown goes down. After this, the boss class managaes the width
                UIScript.BossHpDecorator.Width = UIScript.BossHpDecorator.Parent.ActualWidth * (float)(currentTimer / maxTimer);

                UIScript.Overlay.Visibility = Visibility.Visible;
                int number = (int)currentTimer;
                if(number % 2 == 0)
                {
                    UIScript.Overlay.BackgroundColor = new Color(UIScript.Overlay.BackgroundColor.R / 255.0f + (float)Game.UpdateTime.Elapsed.TotalSeconds / 4.0f, 0.0f, 0.0f, 0.0f);
                }
                else
                {
                    UIScript.Overlay.BackgroundColor = new Color(UIScript.Overlay.BackgroundColor.R / 255.0f - (float)Game.UpdateTime.Elapsed.TotalSeconds / 4.0f, 0.0f, 0.0f, 0.0f);
                }
            }
            else if(countdownType == CountdownType.BossDead)
            {
                if(currentTimer < maxTimer && (int) (currentTimer * 16) % 2 == 0)
                {
                    // Put overlay color to white, but don't make it visible yet
                    UIScript.Overlay.BackgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    var explosion = AlienShip.alienShipDestroyParticle.Clone();

                    var rand = new Random();
                    explosion.Transform.Position.X = (2.0f * (float) (rand.NextDouble() - 0.5f) * CampaignModeLogic.Boss.Get<Enemy>().sizeX) + CampaignModeLogic.Boss.Transform.Position.X;
                    explosion.Transform.Position.Z = (2.0f * (float) (rand.NextDouble() - 0.5f) * CampaignModeLogic.Boss.Get<Enemy>().sizeY) + CampaignModeLogic.Boss.Transform.Position.Z;
                    explosion.Transform.Position.Y = 0.5f;

                    MainScript.particlesScene.Entities.Add(explosion);

                    
                    if(rand.NextDouble() < 0.1)
                        Utils.PlaySound(AlienShip.AlienDeathSound, 0.2f);
                }
            }
            else if(countdownType == CountdownType.spaceShipDead)
            {

            }

            if(currentTimer >= maxTimer)
            {
                if (countdownType == CountdownType.GameStart)
                {
                    UIScript.TimerText.Visibility = Visibility.Hidden;
                    UIScript.TimerText.TextSize = 50;

                    if (MainScript.isArcadeMode)
                        Entity.Add(new ArcadeModeLogic());
                    else
                        Entity.Add(new CampaignModeLogic());

                    Utils.PlaySound(SoundScript.Timer, 0.5f, 1.5f);

                    Entity.Remove<CountdownScript>();
                    return;
                }
                else if (countdownType == CountdownType.BossStart)
                {
                    UIScript.TimerText.Visibility = Visibility.Hidden;
                    UIScript.TimerText.Text = "";

                    UIScript.Overlay.Visibility = Visibility.Collapsed;
                    UIScript.Overlay.BackgroundColor = new Color(0f, 0f, 0f, 0f);

                    var teleport = EntityPooling.bossTeleport.Clone();
                    teleport.Transform.Position.Y = 1.0F;
                    MainScript.particlesScene.Entities.Add(teleport);
                    MainScript.enemiesScene.Entities.Add(CampaignModeLogic.Boss);

                    Utils.PlaySound(SoundScript.BossTeleport);
                    CampaignModeLogic.bossMusic.Play();

                    if(CampaignModeLogic.bossStartSound != null)
                        CampaignModeLogic.bossStartSound.Stop();

                    Entity.Remove<CountdownScript>();
                    return;
                }
                else if (countdownType == CountdownType.BossDead)
                {
                    // Remove all enemies
                    if (MainScript.enemiesScene.Entities.Count > 0)
                    {
                        CampaignModeLogic.Boss.Scene.Entities.Remove(CampaignModeLogic.Boss);

                        Utils.PlaySound(SoundScript.BossDeath);

                        foreach (var entity in MainScript.enemiesScene.Entities)
                        {
                            var enemyClass = entity.Get<Enemy>();
                            if (enemyClass != null)
                                enemyClass.Kill();
                        }
                        MainScript.enemiesScene.Entities.Clear();
                    }

                    // Make overlay visible and slowly fade out
                    UIScript.Overlay.Visibility = Visibility.Visible;

                    // Value for all color components is the same because it's white color
                    var value = UIScript.Overlay.BackgroundColor.R / 255.0f - (float) Game.UpdateTime.Elapsed.TotalSeconds / 3.5f;
                    UIScript.Overlay.BackgroundColor = new Color(value, value, value, value);

                    if(UIScript.Overlay.BackgroundColor.A < 0.001f)
                    {
                        // Once faded out enough, show gameover screen
                        UIScript.GameOverPanel.Visibility = Visibility.Visible;
                        UIScript.GameOverTitle.Text = "Victory";
                        UIScript.GameOverInfo.Text = "The enemy has been defeated";
                        UIScript.HideGameOverButton.Visibility = Visibility.Hidden;

                        UIScript.Overlay.Visibility = Visibility.Collapsed;

                        Utils.PlaySound(SoundScript.winSound, 0.25f);

                        Entity.Remove<CountdownScript>();
                        return;
                    }
                }
                else if(countdownType == CountdownType.spaceShipDead)
                {

                    if(GameLogic.spaceShip.Get<Spaceship>().isDead)
                    {
                        GameLogic.spaceShip.Get<Spaceship>().isDead = false;
                    }
                    else if(currentTimer < 3.0 * maxTimer)
                    {
                        int number = (int)(currentTimer * 8.0);
                        // Blink model
                        if(number % 2 == 0)
                            GameLogic.spaceShip.Get<Spaceship>().spaceShipModel.Get<ModelComponent>().Enabled = false;
                        else
                            GameLogic.spaceShip.Get<Spaceship>().spaceShipModel.Get<ModelComponent>().Enabled = true;
                            
                    }
                    else if (currentTimer > 3.0 * maxTimer)
                    {
                        GameLogic.spaceShip.Get<Spaceship>().spaceShipModel.Get<ModelComponent>().Enabled = true;
                        GameLogic.spaceShip.Get<Spaceship>().isInvincible = false;
                        Entity.Remove<CountdownScript>();
                        return;
                    }
                }
            }
        }
    }
}
