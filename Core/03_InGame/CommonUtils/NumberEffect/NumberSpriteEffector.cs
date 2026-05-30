using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace PahlUnity
{
    public class NumberSpriteEffector : MonoBehaviour
    {
        [SerializeField] NumberSprites NumberPrefab;

        ObjectBody2D mBaseBody = null;

        void Awake()
        {
            mBaseBody = this.ExGetCompInBase<ObjectBody2D>();
        }

        public void ShowNumberEffect(HealthInfo before, HealthInfo after)
        {
            float deltaHP = after.CurrentHP - before.CurrentHP;
            ShowNumberEffect(deltaHP);
        }

        public void ShowNumberEffect(float number)
        {
            Vector2 startPos = mBaseBody.Head + new Vector2(0, 0.5f);
            NumberSprites effect = Instantiate(NumberPrefab, startPos, Quaternion.identity);
            effect.SetNumber(number.ExFloorToInt());
            effect.transform.DOMoveY(startPos.y + 0.5f, 0.5f).SetEase(Ease.OutQuad)
                .OnComplete(() => effect.FadeOut());
            Destroy(effect.gameObject, 3);
        }
    }
}
