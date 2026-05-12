using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// Timeline에서 사용하는 Signal방식에서 파라미터를 전달할수 있도록 해주는 스크립트
namespace PahlUnity
{
    public class SignalReceiverWithBool : MonoBehaviour, INotificationReceiver
    {
        public SignalAssetEventPair[] signalAssetEventPairs;

        [Serializable]
        public class SignalAssetEventPair
        {
            public SignalAsset signalAsset;
            public ParameterizedEvent events;

            [Serializable]
            public class ParameterizedEvent : UnityEvent<bool> { }
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitterWithBool boolEmitter)
            {
                var matches = signalAssetEventPairs.Where(x => ReferenceEquals(x.signalAsset, boolEmitter.asset));
                foreach (var m in matches)
                {
                    m.events.Invoke(boolEmitter.parameter);
                }
            }
        }
    }
    public class SignalReceiverWithInt : MonoBehaviour, INotificationReceiver
    {
        public SignalAssetEventPair[] signalAssetEventPairs;

        [Serializable]
        public class SignalAssetEventPair
        {
            public SignalAsset signalAsset;
            public ParameterizedEvent events;

            [Serializable]
            public class ParameterizedEvent : UnityEvent<int> { }
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitterWithInt intEmitter)
            {
                var matches = signalAssetEventPairs.Where(x => ReferenceEquals(x.signalAsset, intEmitter.asset));
                foreach (var m in matches)
                {
                    m.events.Invoke(intEmitter.parameter);
                }
            }
        }
    }
    public class SignalReceiverWithFloat : MonoBehaviour, INotificationReceiver
    {
        public SignalAssetEventPair[] signalAssetEventPairs;

        [Serializable]
        public class SignalAssetEventPair
        {
            public SignalAsset signalAsset;
            public ParameterizedEvent events;

            [Serializable]
            public class ParameterizedEvent : UnityEvent<float> { }
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitterWithFloat floatEmitter)
            {
                var matches = signalAssetEventPairs.Where(x => ReferenceEquals(x.signalAsset, floatEmitter.asset));
                foreach (var m in matches)
                {
                    m.events.Invoke(floatEmitter.parameter);
                }
            }
        }
    }
    public class SignalReceiverWithString : MonoBehaviour, INotificationReceiver
    {
        public SignalAssetEventPair[] signalAssetEventPairs;

        [Serializable]
        public class SignalAssetEventPair
        {
            public SignalAsset signalAsset;
            public ParameterizedEvent events;

            [Serializable]
            public class ParameterizedEvent : UnityEvent<string> { }
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitterWithString strEmitter)
            {
                var matches = signalAssetEventPairs.Where(x => ReferenceEquals(x.signalAsset, strEmitter.asset));
                foreach (var m in matches)
                {
                    m.events.Invoke(strEmitter.parameter);
                }
            }
        }
    }
}