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
 *
 * File: TeVirtualMidi.cs
 *
 * This file implements the C#-class-wrapper for the teVirtualMIDI-driver.
 * This class encapsualtes the native C-type interface which is integrated
 * in the teVirtualMIDI32.dll and the teVirtualMIDI64.dll.
 */
using System;
using System.Runtime.InteropServices;

namespace TobiasErichsen.TeVirtualMidi
{

    public sealed class TeVirtualMidi : IDisposable
    {
        /// <summary>Default size of sysex-buffer</summary>
        /// <remarks>Constant TE_VM_DEFAULT_SYSEX_SIZE</remarks>
        private const uint DefaultSysExSize = 65535;

        /// <summary>Constant for loading of teVirtualMIDI-interface-DLL, either 32 or 64 bit</summary>
        private const string DllName = "teVirtualMIDI.dll";

        /// <summary>
        /// Options for internal logging
        /// </summary>
        /// <remarks>
        /// Values from TE_VM_LOGGING_xxx
        /// </remarks>
        [Flags]
        public enum LogOptions : uint
        {
            LogInternalStuff = 1,
            LogDataReceived = 2,
            LogDataSent = 4
        }

        /// <summary>
        /// Port creation options.
        /// </summary>
        /// <remarks>
        /// TE_VM_FLAGS_PARSE_RX - parse incoming data into single, valid MIDI-commands
        /// TE_VM_FLAGS_PARSE_TX - parse outgoing data into single, valid MIDI-commands
        /// TE_VM_FLAGS_INSTANTIATE_RX_ONLY - Only the "midi-out" part of the port is created
        /// TE_VM_FLAGS_INSTANTIATE_TX_ONLY - Only the "midi-in" part of the port is created
        /// TE_VM_FLAGS_INSTANTIATE_BOTH - a bidirectional port is created
        /// </remarks>
        public enum CreationOptions : uint
        {
            ParseIncomingDataIntoValidMidi = 1,
            ParseOutgoingDataIntoValidMidi = 2,
            InstantiateRx = 4,
            InstantiateTx = 8
        }
        
        /// <summary>
        /// Static initializer to retrieve version-info from DLL.
        /// </summary>
        static TeVirtualMidi()
        {
            VersionString = Marshal.PtrToStringAuto(VirtualMIDIGetVersion(ref versionMajor, ref versionMinor, ref versionRelease, ref versionBuild));
            DriverVersionString = Marshal.PtrToStringAuto(VirtualMIDIGetDriverVersion(ref driverVersionMajor, ref driverVersionMinor, ref driverVersionRelease, ref driverVersionBuild));
        }

        public TeVirtualMidi(string portName, uint maxSysexLength = DefaultSysExSize, CreationOptions options = CreationOptions.ParseIncomingDataIntoValidMidi)
        {
            instance = VirtualMIDICreatePortEx2(portName, IntPtr.Zero, IntPtr.Zero, maxSysexLength, (uint)options);
            if (instance == IntPtr.Zero)
            {
                int lastError = Marshal.GetLastWin32Error();
                throw TeVirtualMidiException.NewExceptionForReasonCode(lastError);
            }
            readBuffer = new byte[maxSysexLength];
            readProcessIds = new ulong[17];
            this.maxSysexLength = maxSysexLength;
        }

        public TeVirtualMidi(string portName, uint maxSysexLength, CreationOptions options, ref Guid manufacturer, ref Guid product)
        {
            instance = VirtualMIDICreatePortEx3(portName, IntPtr.Zero, IntPtr.Zero, maxSysexLength, (uint)options, ref manufacturer, ref product);
            if (instance == IntPtr.Zero)
            {
                int lastError = Marshal.GetLastWin32Error();
                throw TeVirtualMidiException.NewExceptionForReasonCode(lastError);
            }
            readBuffer = new byte[maxSysexLength];
            readProcessIds = new ulong[17];
            this.maxSysexLength = maxSysexLength;
        }

        public static int VersionMajor => versionMajor;

        public static int VersionMinor => versionMinor;

        public static int VersionRelease => versionRelease;

        public static int VersionBuild => versionBuild;

        public static string VersionString { get; }

        public static int DriverVersionMajor => driverVersionMajor;

        public static int DriverVersionMinor => driverVersionMinor;

        public static int DriverVersionRelease => driverVersionRelease;

        public static int DriverVersionBuild => driverVersionBuild;

        public static string DriverVersionString { get; }

        public static uint ConfigureLog(LogOptions options)
        {
            return VirtualMIDILogging((uint)options);
        }

        public void Shutdown()
        {
            if (!VirtualMIDIShutdown(instance))
            {
                int lastError = Marshal.GetLastWin32Error();
                throw TeVirtualMidiException.NewExceptionForReasonCode(lastError);
            }
        }

        public void SendCommand(byte[] command)
        {
            if ((command == null) || (command.Length == 0))
            {
                return;
            }
            if (!VirtualMIDISendData(instance, command, (uint)command.Length))
            {
                int lastError = Marshal.GetLastWin32Error();
                throw TeVirtualMidiException.NewExceptionForReasonCode(lastError);
            }
        }

        public byte[] GetCommand()
        {
            uint length = maxSysexLength;
            if (!VirtualMIDIGetData(instance, readBuffer, ref length))
            {
                int lastError = Marshal.GetLastWin32Error();
                throw TeVirtualMidiException.NewExceptionForReasonCode(lastError);
            }
            byte[] outBytes = new byte[length];
            Array.Copy(readBuffer, outBytes, length);
            return outBytes;
        }

        public ulong[] GetProcessIds()
        {
            uint length = 17 * sizeof(ulong);
            uint count;
            if (!VirtualMIDIGetProcesses(instance, readProcessIds, ref length))
            {
                int lastError = Marshal.GetLastWin32Error();
                throw TeVirtualMidiException.NewExceptionForReasonCode(lastError);
            }
            count = length / sizeof(ulong);
            ulong[] outIds = new ulong[count];
            Array.Copy(readProcessIds, outIds, count);
            return outIds;
        }

        private readonly byte[] readBuffer;
        private IntPtr instance;
        private readonly uint maxSysexLength;
        private readonly ulong[] readProcessIds;
        private static readonly ushort versionMajor;
        private static readonly ushort versionMinor;
        private static readonly ushort versionRelease;
        private static readonly ushort versionBuild;
        private static readonly ushort driverVersionMajor;
        private static readonly ushort driverVersionMinor;
        private static readonly ushort driverVersionRelease;
        private static readonly ushort driverVersionBuild;

        [DllImport(DllName, EntryPoint = "virtualMIDICreatePortEx3", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr VirtualMIDICreatePortEx3(string portName, IntPtr callback, IntPtr dwCallbackInstance, uint maxSysexLength, uint flags, ref Guid manufacturer, ref Guid product);

        [DllImport(DllName, EntryPoint = "virtualMIDICreatePortEx2", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr VirtualMIDICreatePortEx2(string portName, IntPtr callback, IntPtr dwCallbackInstance, uint maxSysexLength, uint flags);

        [DllImport(DllName, EntryPoint = "virtualMIDIClosePort", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void VirtualMIDIClosePort(IntPtr instance);

        [DllImport(DllName, EntryPoint = "virtualMIDIShutdown", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool VirtualMIDIShutdown(IntPtr instance);

        [DllImport(DllName, EntryPoint = "virtualMIDISendData", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool VirtualMIDISendData(IntPtr midiPort, byte[] midiDataBytes, uint length);

        [DllImport(DllName, EntryPoint = "virtualMIDIGetData", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool VirtualMIDIGetData(IntPtr midiPort, [Out] byte[] midiDataBytes, ref uint length);

        [DllImport(DllName, EntryPoint = "virtualMIDIGetProcesses", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool VirtualMIDIGetProcesses(IntPtr midiPort, [Out] ulong[] processIds, ref uint length);

        [DllImport(DllName, EntryPoint = "virtualMIDIGetVersion", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr VirtualMIDIGetVersion(ref ushort major, ref ushort minor, ref ushort release, ref ushort build);

        [DllImport(DllName, EntryPoint = "virtualMIDIGetDriverVersion", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr VirtualMIDIGetDriverVersion(ref ushort major, ref ushort minor, ref ushort release, ref ushort build);

        [DllImport(DllName, EntryPoint = "virtualMIDILogging", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint VirtualMIDILogging(uint loggingMask);

        private bool disposedValue = false;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // No managed objects to dispose here
                }
                if (instance != IntPtr.Zero)
                {
                    VirtualMIDIClosePort(instance);
                    instance = IntPtr.Zero;
                }
                disposedValue = true;
            }
        }

        ~TeVirtualMidi()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
