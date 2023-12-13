using UnityEngine;

namespace DefaultNamespace
{
    public class SnakeGame
    {
        public const float FOOD_MIN_SPAWN_TIME = 1.0f;
        public const float FOOD_MAX_SPAWN_TIME = 5.0f;
        
        public const float PLAYER_RESPAWN_TIME = 2.0f;

        public const int PLAYER_MAX_LIVES = 2;

        public const string PLAYER_LIVES = "PlayerLives";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

        public static Color GetColor(int colorChoice)
        {
            switch (colorChoice)
            {
                case 0: return Color.red;
                case 1: return Color.green;
                case 2: return Color.blue;
                case 3: return Color.yellow;
                case 4: return Color.cyan;
                case 5: return Color.grey;
                case 6: return Color.magenta;
                case 7: return Color.white;
            }

            return Color.black;
        }
    }
}