using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    public static class SavedData
    {
        private static List<int> PlayersValues = new();

        public static void CreatePlayers(int playersCount)
        {
            // for (int i = 0; i < playersCount; i++)
            // {
            //     PlayersValues.Add(0);
            // }
        }
        public static void GrowPlayerValue(int playerId)
        {
           //PlayersValues[playerId] += 1;
        }

        public static int GetMaxCount()
        {
            //return PlayersValues.Max();
            return 5;
        }

        public static int GetPlayerCount(int playerId)
        {
            //return PlayersValues[playerId];
            return 2;
        }
    }
}