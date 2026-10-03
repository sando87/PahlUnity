using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class InstantiatorMono : MonoBehaviour
    {
        [SerializeField] GameObject _TargetPrefab;
        [SerializeField] Transform _PositionRotation;
        [SerializeField] Transform _Parent = null;

        public void Instantiate()
        {
            GameObject obj = Instantiate(_TargetPrefab, _PositionRotation.position, _PositionRotation.rotation, _Parent);
            obj.SetActive(true);
        }

        public void InstantiateThis(Transform positionTr)
        {
            GameObject obj = Instantiate(gameObject, positionTr.position, Quaternion.identity);
            obj.SetActive(true);
        }


    }
}