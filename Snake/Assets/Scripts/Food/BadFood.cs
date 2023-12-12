using UnityEngine;

namespace Food
{
    public class BadFood : MonoBehaviour
    {
        public void PlayVFX()
        { 
            var vfx = GetComponentInChildren<ParticleSystem>();
           if(vfx != null)
               vfx.Play();
        }
    }
}