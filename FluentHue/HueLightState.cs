namespace FluentHue
{
    using FluentHue.Contracts;
    using Validation;

    public sealed class HueLightState : IHueLightState
    {
        internal HueLightState(HueLightStateMetadata metadata)
        {
            Requires.NotNull(metadata, nameof(metadata));

            this.IsOn = metadata.On;
        }

        /// <summary>
        /// Gets a value indicating whether the light is on.
        /// </summary>
        public bool IsOn { get; }
    }
}