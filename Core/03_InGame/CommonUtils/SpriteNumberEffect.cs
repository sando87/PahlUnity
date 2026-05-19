using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace PahlUnity
{
    public class SpriteNumberEffect : MonoBehaviour
    {
        [SerializeField] NumberSprites NumberPrefab;

        BaseObject mBaseObject = null;

        void Awake()
        {
            mBaseObject = this.ExGetBase();
        }

        public void ShowNumberEffect(HealthInfo before, HealthInfo after)
        {
            Vector2 startPos = mBaseObject.Body.Head + new Vector2(0, 0.5f);
            NumberSprites effect = Instantiate(NumberPrefab, startPos, Quaternion.identity);
            int deltaHP = after.CurrentHP - before.CurrentHP;
            effect.SetNumber(deltaHP);
            effect.transform.DOMoveY(startPos.y + 0.5f, 0.5f).SetEase(Ease.OutQuad)
                .OnComplete(() => effect.FadeOut());

            Destroy(effect.gameObject, 3);
        }
    }
}
