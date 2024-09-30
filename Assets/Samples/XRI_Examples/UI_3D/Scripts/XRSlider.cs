using System;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Content.Interaction
{
    /// <summary>
    /// An interactable that follows the position of the interactor on a single axis
    /// </summary>
    public class XRSlider : XRBaseInteractable
    {
        [Serializable]
        public class ValueChangeEvent : UnityEvent<float> { }

        [SerializeField]
        [Tooltip("In which axis the slider will change")]
        private Axis m_SlideAxis;
        private enum Axis
        {
            X,
            Y, 
            Z
        }

        [SerializeField]
        [Tooltip("The object that is visually grabbed and manipulated")]
        Transform m_Handle = null;

        [SerializeField]
        [Tooltip("The value of the slider")]
        [Range(0.0f, 1.0f)]
        float m_Value = 0.5f;

        [SerializeField]
        [Tooltip("The offset of the slider at value '1'")]
        float m_MaxPosition = 0.5f;

        [SerializeField]
        [Tooltip("The offset of the slider at value '0'")]
        float m_MinPosition = -0.5f;

        [SerializeField]
        [Tooltip("Events to trigger when the slider is moved")]
        ValueChangeEvent m_OnValueChange = new ValueChangeEvent();

        IXRSelectInteractor m_Interactor;

        /// <summary>
        /// The value of the slider
        /// </summary>
        public float value
        {
            get => m_Value;
            set
            {
                SetValue(value);
                SetSliderPosition(value);
            }
        }

        /// <summary>
        /// Events to trigger when the slider is moved
        /// </summary>
        public ValueChangeEvent onValueChange => m_OnValueChange;

        void Start()
        {
            SetValue(m_Value);
            SetSliderPosition(m_Value);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(StartGrab);
            selectExited.AddListener(EndGrab);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(StartGrab);
            selectExited.RemoveListener(EndGrab);
            base.OnDisable();
        }

        void StartGrab(SelectEnterEventArgs args)
        {
            m_Interactor = args.interactorObject;
            UpdateSliderPosition();
        }

        void EndGrab(SelectExitEventArgs args)
        {
            m_Interactor = null;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected)
                {
                    UpdateSliderPosition();
                }
            }
        }

        void UpdateSliderPosition()
        {
            // Put anchor position into slider space
            var localPosition = transform.InverseTransformPoint(m_Interactor.GetAttachTransform(this).position);

            var sliderValue = 0f;
            
            if (m_SlideAxis == Axis.X)
            {
                sliderValue = Mathf.Clamp01((localPosition.x - m_MinPosition) / (m_MaxPosition - m_MinPosition));
            }
            else if (m_SlideAxis == Axis.Y)
            {
                sliderValue = Mathf.Clamp01((localPosition.y - m_MinPosition) / (m_MaxPosition - m_MinPosition));
            }
            else
            {
                sliderValue = Mathf.Clamp01((localPosition.z - m_MinPosition) / (m_MaxPosition - m_MinPosition));
            }
            
            SetValue(sliderValue);
            SetSliderPosition(sliderValue);
        }

        void SetSliderPosition(float value)
        {
            if (m_Handle == null)
                return;

            var handlePos = m_Handle.localPosition;

            if (m_SlideAxis == Axis.X)
            {
                handlePos.x = Mathf.Lerp(m_MinPosition, m_MaxPosition, value);
            }
            else if (m_SlideAxis == Axis.Y)
            {
                handlePos.y = Mathf.Lerp(m_MinPosition, m_MaxPosition, value);
            }
            else
            {
                handlePos.z = Mathf.Lerp(m_MinPosition, m_MaxPosition, value);
            }

            m_Handle.localPosition = handlePos;
        }

        void SetValue(float value)
        {
            m_Value = value;
            m_OnValueChange.Invoke(m_Value);
        }

        void OnDrawGizmosSelected()
        {

            var sliderMinPoint = new Vector3();
            var sliderMaxPoint = new Vector3();

            if (m_SlideAxis == Axis.X)
            {
                sliderMinPoint = transform.TransformPoint(new Vector3(m_MinPosition, 0.0f, 0.0f));
                sliderMaxPoint = transform.TransformPoint(new Vector3(m_MaxPosition, 0.0f, 0.0f));
            }
            else if (m_SlideAxis == Axis.Y)
            {
                sliderMinPoint = transform.TransformPoint(new Vector3(0.0f, m_MinPosition, 0.0f));
                sliderMaxPoint = transform.TransformPoint(new Vector3(0.0f, m_MaxPosition, 0.0f));
            }
            else
            {
                sliderMinPoint = transform.TransformPoint(new Vector3(0.0f, 0.0f, m_MinPosition));
                sliderMinPoint = transform.TransformPoint(new Vector3(0.0f, 0.0f, m_MaxPosition));
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(sliderMinPoint, sliderMaxPoint);
        }

        void OnValidate()
        {
            SetSliderPosition(m_Value);
        }

        public void SetValueWithoutNotify(float value)
        {
            m_Value = value;
            SetSliderPosition(value);
        }
    }
}
