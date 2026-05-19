using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OneM.InputSystem
{
    /// <summary>
    /// Data container for Sprite Tag using an <see cref="InputActionAsset"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "ActionSpriteTag", menuName = MENU_NAME + "Action Sprite Tag")]
    public sealed class ActionSpriteTag : AbstractSpriteTag, IDisposable
    {
        [SerializeField, Tooltip("The Input Asset where your action is.")]
        private InputActionAsset inputAsset;
        [SerializeField, Tooltip("The Input Action.")]
        private InputActionPopup actionPopup = new(nameof(inputAsset));

        /// <summary>
        /// Event fired when the action has been started.
        /// </summary>
        public event Action OnActionStarted;

        /// <summary>
        /// Event fired when the action has been fully performed.
        /// </summary>
        public event Action OnActionPerformed;

        /// <summary>
        /// Event fired when the action has been started but then canceled before being fully performed.
        /// </summary>
        public event Action OnActionCanceled;

        /// <summary>
        /// The Input Action associated with this Sprite Tag.
        /// </summary>
        public InputAction Action { get; private set; }

        public override string GetTag(InputDeviceType device)
        {
            var action = inputAsset.FindAction(
                actionPopup.GetPath(),
                throwIfNotFound: true
            );
            var assetName = device.ToString();
            var inputBinding = device.GetInputBinding();
            var bidingIndex = action.GetBindingIndex(inputBinding);

            if (bidingIndex < 0) return string.Empty;

            var binding = action.GetBindingDisplayString(
                bidingIndex,
                out string _,
                out string controlPath
            );
            var spriteName = controlPath ?? binding.ToString();

            return GetTagUsingName(assetName, spriteName);
        }

        /// <summary>
        /// Gets the action from the Input Action.
        /// </summary>
        /// <returns></returns>
        public InputAction GetAction() => inputAsset.FindAction(actionPopup.GetPath(), throwIfNotFound: true);

        /// <summary>
        /// Initializes the Action by enabling it and subscribing to its events.
        /// </summary>
        public void Initialize()
        {
            Action = GetAction();

            Action.started += HandleActionStarted;
            Action.performed += HandleActionPerformed;
            Action.canceled += HandleActionCanceled;

            Action.Enable();
        }

        /// <summary>
        /// Disposes the Action by disabling it and unsubscribing from all events.
        /// </summary>
        public void Dispose()
        {
            if (Action == null) return;

            Action.Disable();

            Action.started -= HandleActionStarted;
            Action.performed -= HandleActionPerformed;
            Action.canceled -= HandleActionCanceled;

            Action = null;
        }

        private void HandleActionStarted(InputAction.CallbackContext _) => OnActionStarted?.Invoke();
        private void HandleActionPerformed(InputAction.CallbackContext _) => OnActionPerformed?.Invoke();
        private void HandleActionCanceled(InputAction.CallbackContext _) => OnActionCanceled?.Invoke();
    }
}