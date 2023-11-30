using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class MainScript : SyncScript
    {
        public enum Mode
        {
            SinglePlayerCampaign,
            SinglePlayerArcade,
            MultiPlayerCampaign,
            MultiPlayerVersus
        }

        public enum Stage
        {
            stage1,
            stage2,
            stage3,
            stage4
        }

        // This is a single scene game (no scene switching), so keep reference to some entities that never get removed
        public Entity stage1Map;
        public Entity stage2Map;
        public Entity stage3Map;
        public Entity stage4Map;
        public static Dictionary<string, Entity> StageImages;

        public static Mode mode;
        public static Stage stage;

        // Children scenes
        public static Scene projectilesScene;
        public static Scene enemiesScene;
        public static Scene particlesScene;

        public static Entity Camera;

        public override void Start()
        {
            Game.Window.SetSize(new Int2(900, 900));
            EntityPooling.InstantiateEntites(Content);
            SoundScript.LoadSounds(Content);

            // Whole game is made using a single scene, so might as well do this at game startup
            projectilesScene = Content.Load<Scene>("ChildrenScenes/projectilesScene");
            enemiesScene = Content.Load<Scene>("ChildrenScenes/enemiesScene");
            particlesScene = Content.Load<Scene>("ChildrenScenes/particlesScene");

            Entity.Scene.Children.Add(projectilesScene);
            Entity.Scene.Children.Add(enemiesScene);
            Entity.Scene.Children.Add(particlesScene);

            StageImages = new() { { "stage1", stage1Map }, { "stage2", stage2Map }, { "stage3", stage3Map }, { "stage4", stage4Map } };
            Entity.Add(new UIScript());

            Camera = Entity;
        }

        // When returning back from game, refresh everything to be the same as when game was started

        public override void Update()
        {
        }
    }
}
