using Stride.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class MultiplayerCoopSpaceShip : Spaceship
    {
        public bool isGameOver;

        public override void Start()
        {
            base.Start();
            isGameOver = false;
        }

        protected override void ShootBomb()
        {
            // Leave empty as bombs are disabled in versus
        }

        protected override void CheckIfDead()
        {
            base.CheckIfDead();

            if(MultiplayerCoopLogic.friendlyFire)
            {
                foreach (var player in MultiplayerCoopLogic.playerInfo.Keys)
                {
                    if (player != Entity)
                    {
                        var spaceShipClass = player.Get<MultiplayerCoopSpaceShip>();

                        if (!spaceShipClass.isInvincible && !isInvincible && Utils.CheckIfInRange2D(Entity.Transform.Position, player.Transform.Position, 0.2f + 0.2f, 0.2f + 0.2f))
                        {
                            Kill();
                            spaceShipClass.Kill();
                        }
                    }
                }
            }
        }

        protected override bool CheckIfCanShoot()
        {
            return MultiplayerCoopLogic.playerInfo[Entity] < maxProjectilesOnScreen;
        }

        protected override void LoseGame()
        {
            if (MultiplayerCoopLogic.numberOfLives > 0)
                return;

            isGameOver = true;

            foreach (var player in MultiplayerCoopLogic.SpaceShipSelection)
            {
                if (player != null)
                {
                    var spaceShipClass = player.Get<MultiplayerCoopSpaceShip>();
                    if (!spaceShipClass.isGameOver)
                        return;
                }
            }

            // There are no lives left and none of the players are alive, it's game over
            Utils.PlaySound(SoundScript.loseSound);

            UIScript.GameOverPanel.Visibility = Visibility.Visible;
            UIScript.GameOverTitle.Text = "Game over";
            UIScript.GameOverInfo.Text = "All lives lost";
            UIScript.HideGameOverButton.Visibility = Visibility.Hidden;

            GameLogic.isGameOver = true;
        }

        protected override int GetNumberOfLives()
        {
            return MultiplayerCoopLogic.numberOfLives;
        }

        protected override void SubtractLife()
        {
            MultiplayerCoopLogic.numberOfLives--;
        }
    }
}
