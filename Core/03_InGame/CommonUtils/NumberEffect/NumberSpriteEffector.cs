using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace PahlUnity
{
    public class NumberSpriteEffector : MonoBehaviour
    {
        [SerializeField] NumberSprites NumberPrefab;

        ObjectBody3D mBaseBody = null;

        void Awake()
        {
            mBaseBody = this.ExGetCompInBase<ObjectBody3D>();
        }

        public void ShowNumberEffect(IDamageInfo damageInfo, BaseObject attacker)
        {
            float deltaHP = damageInfo.Value;
            ShowNumberEffect(deltaHP);
        }

        public void ShowNumberEffect(float number)
        {
            Vector3 startPos = mBaseBody.Head;
            NumberSprites effect = Instantiate(NumberPrefab, startPos, Quaternion.identity);
            int val = Mathf.RoundToInt(number);
            val.ExSetMinimum(1);
            effect.SetNumber(val);
            effect.transform.DOMoveY(startPos.y + 0.5f, 0.5f).SetEase(Ease.OutQuad)
                .OnComplete(() => effect.FadeOut());
            Destroy(effect.gameObject, 3);
        }
    }
}
