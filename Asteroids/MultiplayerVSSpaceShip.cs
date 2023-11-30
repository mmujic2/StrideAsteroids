using Stride.Audio;
using Stride.Core;
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
    public class MultiplayerVSSpaceShip : Spaceship
    {
        public bool isGameOver;
        public float projectileSpeed;

        public override void Start()
        {
            base.Start();
            isGameOver = false;
        }

        public override void Update()
        {
            base.Update();
        }

        protected override void CheckIfDead()
        {
            base.CheckIfDead();

            foreach(var player in MultiplayerVSLogic.playerInfo.Keys)
            {
                
                if (player != Entity)
                {
                    var spaceShipClass = player.Get<MultiplayerVSSpaceShip>();

                    if(!spaceShipClass.isInvincible && !isInvincible && Utils.CheckIfInRange2D(Entity.Transform.Position, player.Transform.Position, 0.2f + 0.2f, 0.2f + 0.2f))
                    {
                        Kill();
                        spaceShipClass.Kill();
                    }
                }
            }
        }

        protected override bool CheckIfCanShoot()
        {
            return MultiplayerVSLogic.playerInfo[Entity].Item2 < maxProjectilesOnScreen;
        }

        protected override void LoseGame()
        {
            int count = 0;
            int playerIndex = -1;
            int i = 0;

            isGameOver = true;

            foreach (var player in MultiplayerVSLogic.SpaceShipSelection)
            {
                if(player != null)
                {
                    var spaceShipClass = player.Get<MultiplayerVSSpaceShip>();
                    if (player != Entity && !spaceShipClass.isGameOver)
                    {
                        playerIndex = i;
                        count++;
                    }
                }
                
                i++;
            }

            playerIndex++;

            // Ended with collision between two player, both players died so it's a tie
            if(count == 0)
            {
                Utils.PlaySound(SoundScript.winSound);

                UIScript.GameOverPanel.Visibility = Visibility.Visible;
                UIScript.GameOverTitle.Text = "Game over";
                UIScript.GameOverInfo.Text = "Tied game";
                UIScript.HideGameOverButton.Visibility = Visibility.Hidden;
                SinglePlayerLogic.isGameOver = true;

                GameLogic.isGameOver = true;
            }
            // If only one player alive
            else if (count < 2) 
            {
                Utils.PlaySound(SoundScript.winSound);

                UIScript.GameOverPanel.Visibility = Visibility.Visible;
                UIScript.GameOverTitle.Text = "Game over";
                UIScript.GameOverInfo.Text = "Player " + playerIndex.ToString() + " won";
                UIScript.HideGameOverButton.Visibility = Visibility.Hidden;
                SinglePlayerLogic.isGameOver = true;

                GameLogic.isGameOver = true;
            } 
        }

        protected override void ShootBomb()
        {
            // Leave empty as bombs are disabled in versus
        }

        protected override int GetNumberOfLives()
        {
            return MultiplayerVSLogic.playerInfo[Entity].Item1;
        }

        protected override void SubtractLife()
        {
            var info = MultiplayerVSLogic.playerInfo[Entity];
            MultiplayerVSLogic.playerInfo[Entity] = new(info.Item1 - 1, info.Item2);
        }

        protected override float GetProjectileSpeed()
        {
            return projectileSpeed;
        }
    }
}
