namespace FluentHue.Tests
{
    using System;

    public abstract class RandomBase
    {
        private Random _rand = new Random();

        protected bool GetRandomBool()
            => this._rand.Next() % 2 == 0;

        protected byte GetRandomByte()
            => (byte)this._rand.Next(1, 254);

        protected float GetRandomFloat()
            => (float)this._rand.NextDouble();
    }
}