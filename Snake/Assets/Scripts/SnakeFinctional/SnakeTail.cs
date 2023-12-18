using Photon.Realtime;
using UnityEngine;

namespace DefaultNamespace.SnakeFinctional
{
    public class SnakeTail: MonoBehaviour
    {
        public Player Owner { get; private set; }

        public void InitializeTail(Player owner, Transform parent)
        {
            Owner = owner;
            transform.SetParent(parent);
        }
    }
}