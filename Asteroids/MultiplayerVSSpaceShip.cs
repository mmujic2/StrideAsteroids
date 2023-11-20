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
        public override void Start()
        {
            base.Start();
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
                if (player != Entity && Utils.CheckIfInRange2D(Entity.Transform.Position, player.Transform.Position, 0.2f + 0.2f, 0.2f + 0.2f))
                {
                    Kill();
                    break;
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
            foreach (var player in MultiplayerVSLogic.playerInfo)
            {
                if (player.Key != Entity && player.Value.Item1 > 0)
                {
                    playerIndex = i;
                    count++;
                }
                i++;
            }

            playerIndex++;

            // If only one player alive
            if (count < 2) 
            {
                Utils.PlaySound(SoundScript.winSound);

                UIScript.GameOverPanel.Visibility = Visibility.Visible;
                UIScript.GameOverTitle.Text = "Game over";
                UIScript.GameOverInfo.Text = "Player " + playerIndex.ToString() + " won";
                UIScript.HideGameOverButton.Visibility = Visibility.Hidden;
                SinglePlayerLogic.isGameOver = true;
            } 
        }

        protected override void SubtractLife()
        {
            var info = MultiplayerVSLogic.playerInfo[Entity];
            MultiplayerVSLogic.playerInfo[Entity] = new(info.Item1 - 1, info.Item2);
        }
    }
}
