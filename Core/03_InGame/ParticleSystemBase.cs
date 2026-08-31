using UnityEngine;

namespace PahlUnity
{
    public class ParticleSystemBase : MonoBehaviour
    {
        // Root 파티클은 Stop하는 순간 모두 안보이게 처리
        // 자식 파티클은 Stop하는 순간 Emitting만 중단(기존 파티클은 남아있도록 유지)
        // 주로 투사체뒤에 Tail효과가 붙는 방식에 적용
        public void StopParticleSystem()
        {
            ParticleSystem rootPS = GetComponent<ParticleSystem>();
            ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>(true);
            for (int j = 0; j < systems.Length; j++)
            {
                ParticleSystem ps = systems[j];
                if (ps == null)
                    continue;

                if (ps == rootPS)
                    ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                else
                    ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        public void PlayIfFirst(int index)
        {
            if (index == 0)
            {
                ParticleSystem rootPS = GetComponent<ParticleSystem>();
                rootPS.Play();
            }
        }

        public void PlayIfSecond(int index)
        {
            if (index == 1)
            {
                ParticleSystem rootPS = GetComponent<ParticleSystem>();
                rootPS.Play();
            }
        }
    }
}