﻿using Melanchall.DryWetMidi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Melanchall.DryWetMidi.Smf.Interaction
{
    /// <summary>
    /// Represents a musical chord.
    /// </summary>
    public sealed class Chord : ILengthedObject
    {
        #region Events

        /// <summary>
        /// Occurs when notes collection changes.
        /// </summary>
        public event NotesCollectionChangedEventHandler NotesCollectionChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Chord"/>.
        /// </summary>
        public Chord()
            : this(Enumerable.Empty<Note>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chord"/> with the specified
        /// collection of notes.
        /// </summary>
        /// <param name="notes">Notes to combine into a chord.</param>
        /// <exception cref="ArgumentNullException"><paramref name="notes"/> is null.</exception>
        public Chord(IEnumerable<Note> notes)
        {
            ThrowIfArgument.IsNull(nameof(notes), notes);

            Notes = new NotesCollection(notes);
            Notes.CollectionChanged += OnNotesCollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chord"/> with the specified
        /// collection of notes.
        /// </summary>
        /// <param name="notes">Notes to combine into a chord.</param>
        /// <exception cref="ArgumentNullException"><paramref name="notes"/> is null.</exception>
        public Chord(params Note[] notes)
            : this(notes as IEnumerable<Note>)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chord"/> with the specified
        /// collection of notes and chord time.
        /// </summary>
        /// <param name="notes">Notes to combine into a chord.</param>
        /// <param name="time">Time of the chord which is time of the earliest note of the <paramref name="notes"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="notes"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is negative.</exception>
        public Chord(IEnumerable<Note> notes, long time)
            : this(notes)
        {
            ThrowIfTimeArgument.IsNegative(nameof(time), time);

            Time = time;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a <see cref="NotesCollection"/> that represents notes of this chord.
        /// </summary>
        public NotesCollection Notes { get; }

        /// <summary>
        /// Gets or sets absolute time of the chord in units defined by the time division of a MIDI file.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Time is negative.</exception>
        public long Time
        {
            get { return Notes.FirstOrDefault()?.Time ?? 0; }
            set
            {
                ThrowIfTimeArgument.IsNegative(nameof(value), value);

                var currentTime = Time;

                foreach (var note in Notes)
                {
                    var offset = note.Time - currentTime;
                    note.Time = value + offset;
                }
            }
        }

        /// <summary>
        /// Gets or sets length of the chord in units defined by the time division of a MIDI file.
        /// </summary>
        public long Length
        {
            get
            {
                if (!Notes.Any())
                    return 0;

                var startTime = long.MaxValue;
                var endTime = long.MinValue;

                foreach (var note in Notes)
                {
                    var noteStartTime = note.Time;
                    startTime = Math.Min(noteStartTime, startTime);

                    var noteEndTime = noteStartTime + note.Length;
                    endTime = Math.Max(noteEndTime, endTime);
                }

                return endTime - startTime;
            }
            set
            {
                var lengthChange = value - Length;

                foreach (var note in Notes)
                {
                    note.Length += lengthChange;
                }
            }
        }

        public FourBitNumber Channel
        {
            get => GetNotesProperty(n => n.Channel);
            set => SetNotesProperty(n => n.Channel, value);
        }

        public SevenBitNumber Velocity
        {
            get => GetNotesProperty(n => n.Velocity);
            set => SetNotesProperty(n => n.Velocity, value);
        }

        public SevenBitNumber OffVelocity
        {
            get => GetNotesProperty(n => n.OffVelocity);
            set => SetNotesProperty(n => n.OffVelocity, value);
        }

        #endregion

        #region Methods

        private void OnNotesCollectionChanged(NotesCollection collection, NotesCollectionChangedEventArgs args)
        {
            NotesCollectionChanged?.Invoke(collection, args);
        }

        private TValue GetNotesProperty<TValue>(Expression<Func<Note, TValue>> propertySelector)
        {
            if (!Notes.Any())
                throw new InvalidOperationException("Chord doesn't contain notes.");

            var propertyInfo = GetPropertyInfo(propertySelector);

            var values = Notes.Select(n => (TValue)propertyInfo.GetValue(n)).Distinct().ToArray();
            if (values.Length > 1)
                throw new InvalidOperationException($"Chord's notes have different values of the {propertyInfo.Name} property.");

            return values.First();
        }

        private void SetNotesProperty<TValue>(Expression<Func<Note, TValue>> propertySelector, TValue value)
        {
            var propertyInfo = GetPropertyInfo(propertySelector);

            foreach (var note in Notes)
            {
                propertyInfo.SetValue(note, value);
            }
        }

        private static PropertyInfo GetPropertyInfo<TValue>(Expression<Func<Note, TValue>> propertySelector)
        {
            return (propertySelector.Body as MemberExpression)?.Member as PropertyInfo;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var notes = Notes;
            return notes.Any()
                ? string.Join(" ", notes.OrderBy(n => n.NoteNumber))
                : "Empty notes collection";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;

            var chord = obj as Chord;
            if (chord == null)
                return false;

            return Notes.SequenceEqual(chord.Notes);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int result = 0;

            foreach (var note in Notes)
            {
                result = ((result << 5) + result) ^ note.GetHashCode();
            }

            return result;
        }

        #endregion
    }
}
