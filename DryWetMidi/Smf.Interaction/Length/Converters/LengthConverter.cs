﻿using Melanchall.DryWetMidi.Common;
using System;

namespace Melanchall.DryWetMidi.Smf.Interaction
{
    /// <summary>
    /// Provides a way to convert the length of an object from one representation to another.
    /// </summary>
    public static class LengthConverter
    {
        #region Methods

        /// <summary>
        /// Converts length from <see cref="long"/> to the specified length type.
        /// </summary>
        /// <typeparam name="TLength">Type that will represent the length of an object.</typeparam>
        /// <param name="length">Length to convert.</param>
        /// <param name="time">Start time of an object to convert length of.</param>
        /// <param name="tempoMap">Tempo map used to convert <paramref name="length"/>.</param>
        /// <returns>Length as an instance of <typeparamref name="TLength"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative. -or-
        /// <paramref name="time"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tempoMap"/> is null.</exception>
        /// <exception cref="NotSupportedException"><typeparamref name="TLength"/> is not supported.</exception>
        public static TLength ConvertTo<TLength>(long length, long time, TempoMap tempoMap)
            where TLength : ILength
        {
            ThrowIfLengthArgument.IsNegative(nameof(length), length);
            ThrowIfTimeArgument.IsNegative(nameof(time), time);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            return (TLength)GetConverter<TLength>().ConvertTo(length, time, tempoMap);
        }

        /// <summary>
        /// Converts length from <see cref="long"/> to the specified length type.
        /// </summary>
        /// <typeparam name="TLength">Type that will represent the length of an object.</typeparam>
        /// <param name="length">Length to convert.</param>
        /// <param name="time">Start time of an object to convert length of.</param>
        /// <param name="tempoMap">Tempo map used to convert <paramref name="length"/>.</param>
        /// <returns>Length as an instance of <typeparamref name="TLength"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="time"/> is null. -or-
        /// <paramref name="tempoMap"/> is null.</exception>
        /// <exception cref="NotSupportedException"><typeparamref name="TLength"/> is not supported.</exception>
        public static TLength ConvertTo<TLength>(long length, ITime time, TempoMap tempoMap)
            where TLength : ILength
        {
            ThrowIfLengthArgument.IsNegative(nameof(length), length);
            ThrowIfArgument.IsNull(nameof(time), time);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            return ConvertTo<TLength>(length, TimeConverter.ConvertFrom(time, tempoMap), tempoMap);
        }

        /// <summary>
        /// Converts length from one length type to another one.
        /// </summary>
        /// <typeparam name="TLength">Type that will represent the length of an object.</typeparam>
        /// <param name="length">Length to convert.</param>
        /// <param name="time">Start time of an object to convert length of.</param>
        /// <param name="tempoMap">Tempo map used to convert <paramref name="length"/>.</param>
        /// <returns>Length as an instance of <typeparamref name="TLength"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="length"/> is null. -or-
        /// <paramref name="tempoMap"/> is null.</exception>
        /// <exception cref="NotSupportedException"><typeparamref name="TLength"/> is not supported.</exception>
        public static TLength ConvertTo<TLength>(ILength length, long time, TempoMap tempoMap)
            where TLength : ILength
        {
            ThrowIfArgument.IsNull(nameof(length), length);
            ThrowIfTimeArgument.IsNegative(nameof(time), time);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            return ConvertTo<TLength>(ConvertFrom(length, time, tempoMap), time, tempoMap);
        }

        /// <summary>
        /// Converts length from one length type to another one.
        /// </summary>
        /// <typeparam name="TLength">Type that will represent the length of an object.</typeparam>
        /// <param name="length">Length to convert.</param>
        /// <param name="time">Start time of an object to convert length of.</param>
        /// <param name="tempoMap">Tempo map used to convert <paramref name="length"/>.</param>
        /// <returns>Length as an instance of <typeparamref name="TLength"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="length"/> is null. -or-
        /// <paramref name="time"/> is null. -or- <paramref name="tempoMap"/> is null.</exception>
        /// <exception cref="NotSupportedException"><typeparamref name="TLength"/> is not supported.</exception>
        public static TLength ConvertTo<TLength>(ILength length, ITime time, TempoMap tempoMap)
            where TLength : ILength
        {
            ThrowIfArgument.IsNull(nameof(length), length);
            ThrowIfArgument.IsNull(nameof(time), time);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            return ConvertTo<TLength>(length, TimeConverter.ConvertFrom(time, tempoMap), tempoMap);
        }

        /// <summary>
        /// Converts length from the specified length type to <see cref="long"/>.
        /// </summary>
        /// <param name="length">Length to convert.</param>
        /// <param name="time">Start time of an object to convert length of.</param>
        /// <param name="tempoMap">Tempo map used to convert <paramref name="length"/>.</param>
        /// <returns>Length as <see cref="long"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="length"/> is null. -or-
        /// <paramref name="tempoMap"/> is null.</exception>
        public static long ConvertFrom(ILength length, long time, TempoMap tempoMap)
        {
            ThrowIfArgument.IsNull(nameof(length), length);
            ThrowIfTimeArgument.IsNegative(nameof(time), time);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            return LengthConverterFactory.GetConverter(length.GetType())
                                         .ConvertFrom(length, time, tempoMap);
        }

        /// <summary>
        /// Converts length from the specified length type to <see cref="long"/>.
        /// </summary>
        /// <param name="length">Length to convert.</param>
        /// <param name="time">Start time of an object to convert length of.</param>
        /// <param name="tempoMap">Tempo map used to convert <paramref name="length"/>.</param>
        /// <returns>Length as <see cref="long"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="length"/> is null. -or-
        /// <paramref name="time"/> is null. -or- <paramref name="tempoMap"/> is null.</exception>
        public static long ConvertFrom(ILength length, ITime time, TempoMap tempoMap)
        {
            ThrowIfArgument.IsNull(nameof(length), length);
            ThrowIfArgument.IsNull(nameof(time), time);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            return ConvertFrom(length, TimeConverter.ConvertFrom(time, tempoMap), tempoMap);
        }

        /// <summary>
        /// Gets converter that can be used to convert length of an object from <see cref="long"/>
        /// to the specified length type and vice versa.
        /// </summary>
        /// <typeparam name="TLength">Type that will represent the length of an object.</typeparam>
        /// <returns>Converter to convert length between <see cref="long"/> and <typeparamref name="TLength"/>.</returns>
        /// <exception cref="NotSupportedException"><typeparamref name="TLength"/> is not supported.</exception>
        public static ILengthConverter GetConverter<TLength>()
            where TLength : ILength
        {
            return LengthConverterFactory.GetConverter<TLength>();
        }

        #endregion
    }
}
