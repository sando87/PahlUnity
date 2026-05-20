using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace PahlUnity
{
    public class NumberSpriteEffector : MonoBehaviour
    {
        [SerializeField] NumberSprites NumberPrefab;

        BaseObject mBaseObject = null;

        void Awake()
        {
            mBaseObject = this.ExGetBase();
        }

        public void ShowNumberEffect(HealthInfo before, HealthInfo after)
        {
            int deltaHP = after.CurrentHP - before.CurrentHP;
            ShowNumberEffect(deltaHP);
        }

        public void ShowNumberEffect(int number)
        {
            Vector2 startPos = mBaseObject.Body.Head + new Vector2(0, 0.5f);
            NumberSprites effect = Instantiate(NumberPrefab, startPos, Quaternion.identity);
            effect.SetNumber(number);
            effect.transform.DOMoveY(startPos.y + 0.5f, 0.5f).SetEase(Ease.OutQuad)
                .OnComplete(() => effect.FadeOut());
            Destroy(effect.gameObject, 3);
        }
    }
}
