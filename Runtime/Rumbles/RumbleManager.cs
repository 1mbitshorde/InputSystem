using OneM.AwaitableSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OneM.InputSystem
{
    /// <summary>
    /// Global manager handling Gamepad rumble effects using the Unity Input System.
    /// </summary>
    public static class RumbleManager
    {
        /// <summary>
        /// Whether rumble effects enabled.
        /// </summary>
        public static bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Checks if rumble effects are disabled.
        /// </summary>
        /// <returns>True if rumble is disabled; otherwise, false.</returns>
        public static bool IsDisabled() => !IsEnabled;

        /// <summary>
        /// Starts a rumble effect.
        /// </summary>
        /// <param name="data">The frequency and duration configuration for the rumble.</param>
        public static async void StartRumble(RumbleData data) => await StartRumbleAsync(data);

        /// <summary>
        /// Starts a rumble effect and awaits its completion.
        /// </summary>
        /// <param name="data">The frequency and duration configuration for the rumble.</param>
        /// <returns>An Awaitable task that completes when the rumble finishes.</returns>
        public static async Awaitable StartRumbleAsync(RumbleData data)
        {
            if (IsDisabled()) return;

            var gamepad = Gamepad.current;
            if (gamepad == null) return;

            gamepad.SetMotorSpeeds(data.lowFrequency, data.highFrequency);
            await AwaitableUtility.WaitForSecondsRealtimeAsync(data.duration);
            gamepad?.SetMotorSpeeds(0f, 0f); // Gamepad can be disconnected
        }

        /// <summary>
        /// Stops the any rumble effect.
        /// </summary>
        public static void StopRumble() => Gamepad.current?.SetMotorSpeeds(0f, 0f);
    }
}