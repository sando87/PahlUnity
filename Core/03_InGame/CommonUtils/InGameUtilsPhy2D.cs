using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    public static class InGameUtilsPhy2D
    {
        private static Collider2D[] _results = new Collider2D[32];
        private static RaycastHit2D[] _hits = new RaycastHit2D[32];

        public static void OverlapBox<T>(Rect area, int layerMask, List<T> rets) where T : MonoBehaviour
        {
            rets.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.layerMask = layerMask;
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = true;
            int ret = Physics2D.OverlapBox(area.center, area.size, 0, contactFilter, _results);
            if (ret > 0)
            {
                for (int i = 0; i < ret; ++i)
                {
                    Collider2D col = _results[i];
                    BaseObject baseObj = col.GetComponentInParent<BaseObject>();
                    if (baseObj != null)
                    {
                        List<T> tmp = TemporaryList<T>.GetTempList();
                        baseObj.transform.GetComponentsInChildren(tmp);
                        rets.AddRange(tmp);
                        tmp.Clear();
                    }
                }
            }
        }

        public static int OverlapCircleAll(Vector2 center, float radius, int layerMask, uint interactMask, List<BaseObject> rets)
        {
            ContactFilter2D filter2D = new ContactFilter2D();
            filter2D.useLayerMask = true;
            filter2D.layerMask = layerMask;
            filter2D.useTriggers = true;

            int count = Physics2D.OverlapCircle(center, radius, filter2D, _results);
            if (count <= 0)
                return 0;

            int retCount = 0;
            for (int i = 0; i < count; i++)
            {
                Collider2D col = _results[i];
                InteractableCollider interCol = col.GetComponent<InteractableCollider>();
                if (interCol == null)
                    continue;

                uint mask = interactMask & interCol.MyProperty;
                if (mask != 0)
                {
                    BaseObject baseObj = col.ExGetBase();
                    if (baseObj != null)
                    {
                        retCount++;
                        rets.Add(baseObj);
                    }
                }
            }

            return retCount;
        }

        public static BaseObject CircleCast(Vector2 center, float radius, Vector2 direction, float distance, int layerMask, uint interactMask)
        {
            ContactFilter2D filter2D = new ContactFilter2D();
            filter2D.useLayerMask = true;
            filter2D.layerMask = layerMask;
            filter2D.useTriggers = true;

            int count = Physics2D.CircleCast(center, radius, direction, filter2D, _hits, distance);
            if (count <= 0)
                return null;

            BaseObject target = null;
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < count; i++)
            {
                Collider2D col = _hits[i].collider;
                InteractableCollider interCol = col.GetComponent<InteractableCollider>();
                if (interCol == null)
                    continue;

                uint mask = interactMask & interCol.MyProperty;
                if (mask != 0)
                {
                    if (minDist > _hits[i].distance)
                    {
                        minDist = _hits[i].distance;
                        target = col.ExGetBase();
                    }
                }
            }

            return target;
        }

    }
}