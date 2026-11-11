using UnityEngine;
using UnityEngine.Events;

namespace SOAP.Variables
{
    public abstract class ScriptableVariable<T> : RuntimeScriptableObject
    {
        [SerializeField] protected T initialValue;
        [SerializeField] protected T value;

        public event UnityAction<T> OnValueChanged = delegate { };

        public T Value
        {
            get => value;
            set
            {
                if (EqualityComparer(this.value, value)) return;
                this.value = value;
                OnValueChanged.Invoke(value);
            }
        }
        
        protected override void OnReset()
        {
            OnValueChanged.Invoke(value = initialValue);
        }
        
        protected abstract bool EqualityComparer(T a, T b);
        
#if UNITY_EDITOR
        // The last value listeners were told about. Not serialized, so it starts fresh after a domain reload.
        private T lastNotified;
        
        // Editing the value in the inspector changes the serialized field directly and skips the setter,
        // so listeners never hear about it. Notify here, in play mode only, and only when the value really changed
        // (editing initialValue must not re-fire state-entry handlers).
        private void OnValidate()
        {
            if (!Application.isPlaying || EqualityComparer(lastNotified, value)) return;
 
            lastNotified = value;
            OnValueChanged.Invoke(value);
        }
#endif
    }
}