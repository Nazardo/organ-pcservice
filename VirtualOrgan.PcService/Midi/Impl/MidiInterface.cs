using System;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TobiasErichsen.TeVirtualMidi;

namespace VirtualOrgan.PcService.Midi.Impl
{
    sealed class MidiInterface : IMidiInterface
    {
        internal sealed class Options
        {
            public string PortName { get; set; } = nameof(VirtualOrgan.PcService);
        }

        private readonly ILogger<MidiInterface> logger;
        private readonly Thread receiver;
        private readonly IMidiMessageSerializer serializer;
        private TeVirtualMidi port;
        private bool running;

        public IObservable<MidiMessage> Messages { get; }

        private delegate void OnReceivedMidiMessageDelegate(object o, MidiMessage message);

        private event OnReceivedMidiMessageDelegate MessageReceived;

        public MidiInterface(
            IOptions<Options> options,
            ILogger<MidiInterface> logger,
            IMidiMessageSerializer serializer)
        {
            this.logger = logger;
            this.serializer = serializer;
            Messages = Observable.FromEventPattern<OnReceivedMidiMessageDelegate, MidiMessage>(
                h => MessageReceived += h,
                h => MessageReceived -= h)
                .Select(o => o.EventArgs);
            receiver = new Thread(ReceiverThread);
            port = new TeVirtualMidi(options.Value.PortName);
            running = true;
            receiver.Start();
        }

        public void Send(MidiMessage message)
        {
            try
            {
                var bytes = serializer.Serialize(message);
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("OUT > {0}", MessageToString(message));
                }
                port.SendCommand(bytes);
            }
            catch (MidiSerializerException e)
            {
                logger.LogError(e.Message, e);
                throw;
            }
        }

        private void ReceiverThread()
        {
            while (running)
            {
                try
                {
                    var data = port.GetCommand();
                    var message = serializer.Deserialize(data);
                    if (logger.IsEnabled(LogLevel.Debug))
                    {
                        logger.LogDebug("IN < {0}", MessageToString(message));
                    }
                    MessageReceived.Invoke(this, message);
                }
                catch (MidiSerializerException e)
                {
                    logger.LogWarning(e, e.Message);
                }
                catch (TeVirtualMidiException e)
                {
                    if (running && e.ReasonCode != (int)TeVirtualMidiException.Code.InvalidHandle)
                    {
                        logger.LogError(e, e.Message);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                }
            }
        }

        private string MessageToString(MidiMessage message)
        {
            var formatter = new MidiMessageFormatter();
            message.Accept(formatter);
            return formatter.GetString();
        }

        public void Dispose()
        {
            if (running)
            {
                running = false;
                // Shutting down the port is the only way to unlock
                // the reading call in receiver thread.
                port.Shutdown();
                receiver.Join();
            }
            if (port != null)
            {
                port.Dispose();
                port = null;
            }
        }
    }
}
