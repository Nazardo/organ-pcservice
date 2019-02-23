/* teVirtualMIDI C# interface - v1.2.11.41
 *
 * Copyright 2009-2016, Tobias Erichsen
 * All rights reserved, unauthorized usage & distribution is prohibited.
 *
 * For technical or commercial requests contact: info <at> tobias-erichsen <dot> de
 *
 * teVirtualMIDI.sys is a kernel-mode device-driver which can be used to dynamically create & destroy
 * midiports on Windows (XP to Windows 10, 32bit & 64bit).  The "back-end" of teVirtualMIDI can be used
 * to create & destroy such ports and receive and transmit data from/to those created ports.
 */
using System;
using System.Runtime.Serialization;

namespace TobiasErichsen.TeVirtualMidi
{
    [Serializable]
    public sealed class TeVirtualMidiException : Exception
    {
        /// <summary>
        /// Error code
        /// </summary>
        /// <remarks>
        /// Specific WIN32-error-codes that the native teVirtualMIDI-driver
        /// is using to communicate specific problems to the application.
        /// </remarks>
        public enum Code
        {
            PathNotFound = 3,
            InvalidHandle = 6,
            TooManyCommands = 56,
            TooManySessions = 69,
            InvalidName = 123,
            ModuleNotFound = 126,
            BadArguments = 160,
            AlreadyExists = 183,
            OldWindowsVersion = 1150,
            RevisionMismatch = 1306,
            AliasExists = 1379
        }

        public TeVirtualMidiException() { }

        public TeVirtualMidiException(string message)
            : base(message) { }

        public TeVirtualMidiException(string message, Exception inner)
            : base(message, inner) { }

        private TeVirtualMidiException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public int ReasonCode { get; set; }

        private static string ReasonCodeToString(Code reasonCode)
        {
            switch (reasonCode)
            {
                case Code.OldWindowsVersion:
                    return "Your Windows-version is too old for dynamic MIDI-port creation.";
                case Code.InvalidName:
                    return "You need to specify at least 1 character as MIDI-portname!";
                case Code.AlreadyExists:
                    return "The name for the MIDI-port you specified is already in use!";
                case Code.AliasExists:
                    return "The name for the MIDI-port you specified is already in use!";
                case Code.PathNotFound:
                    return "Possibly the teVirtualMIDI-driver has not been installed!";
                case Code.ModuleNotFound:
                    return "The teVirtualMIDIxx.dll could not be loaded!";
                case Code.RevisionMismatch:
                    return "The teVirtualMIDIxx.dll and teVirtualMIDI.sys driver differ in version!";
                case Code.TooManySessions:
                    return "Maximum number of ports reached";
                case Code.InvalidHandle:
                    return "Port not enabled";
                case Code.TooManyCommands:
                    return "MIDI-command too large";
                case Code.BadArguments:
                    return "Invalid flags specified";
                default:
                    return "Unspecified virtualMIDI-error: " + reasonCode;
            }
        }

        internal static Exception NewExceptionForReasonCode(int reasonCode)
        {
            var exception = new TeVirtualMidiException(ReasonCodeToString((Code)reasonCode));
            exception.ReasonCode = reasonCode;
            return exception;
        }

    }
}
