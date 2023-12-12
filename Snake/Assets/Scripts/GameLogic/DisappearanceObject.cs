using System.Collections;
using GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace DisplayScripts.Services
{
    public class DisappearanceObject : SpawnObject
    {
        private Transform _transform;
        private IEnumerator _resizeCoroutine;
        private IEnumerator _fadeOutCoroutine;
        
        public void Resize(float time, float size)
        {
            _transform = transform;
            Resize(time, new Vector3(size, size, size));
        }

        public void FadeOut(Image image, float time)
        {
            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
            }

            _fadeOutCoroutine = FadeImage(image, true, time);
            StartCoroutine(_fadeOutCoroutine);
        }
        
        private void Resize(float time, Vector3 size)
        {
            if (_resizeCoroutine != null)
                StopCoroutine(_resizeCoroutine);
            _resizeCoroutine = ResizeCoroutine(time, size);
            StartCoroutine(_resizeCoroutine);
        }

        private IEnumerator ResizeCoroutine(float time, Vector3 targetSize)
        {
            float Timer = 0;
            Vector3 baseSize= _transform.localScale;
            while (Timer < time)
            {
                _transform.localScale = Vector3.Lerp(baseSize, targetSize, Timer / time);
                yield return null; // задержка цикла до следующего кадра
                Timer += Time.deltaTime;
            }

            _transform.localScale = targetSize;
            _resizeCoroutine = null;
        }

        private IEnumerator FadeOutMaterial(float fadeSpeed)
        {
            if (TryGetComponent(out Renderer rend))
            {
                Color matColor = rend.material.color;
                float alphaValue = rend.material.color.a;

                while (rend.material.color.a > 0f)
                {
                    alphaValue -= Time.deltaTime / fadeSpeed;
                    rend.material.color = new Color(matColor.r, matColor.g, matColor.b, alphaValue);
                    yield return null;
                }

                rend.material.color = new Color(matColor.r, matColor.g, matColor.b, 0f);
            }
        }

        IEnumerator FadeImage(Image img, bool fadeAway, float time)
        {
            // fade from opaque to transparent
            if (fadeAway)
            {
                // loop over 1 second backwards
                for (float i = time; i >= 0; i -= Time.deltaTime)
                {
                    // set color with i as alpha
                    img.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
            // fade from transparent to opaque
            else
            {
                // loop over 1 second
                for (float i = 0; i <= time; i += Time.deltaTime)
                {
                    // set color with i as alpha
                    img.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
        }
    }
}