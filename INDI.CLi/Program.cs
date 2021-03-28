using System;
using System.Threading;
using CommandLine;
using Qkmaxware.Astro.Control;

namespace Qkmaxware.Astro.Cli {

    class Options {
        [Option('h', "host", Required = true, HelpText = "INDI server host ip")]
        public string Host {get; set;}
        [Option('p', "port", Required = false, HelpText = "INDI server host port", Default=7624)]
        public int Port {get; set;}
        [Option("devices", Required = false, HelpText = "Scan for devices on the INDI server")]
        public bool QueryDevices { get; set; }
    }

    /*
        Example Usage:
        > indi -h 10.0.0.145 -p 7624 --devices
        Celestron NexStar
        SVBONY 205
        Searching...
    */
    class IndiCli {
        static void Main() {
            var args = System.Environment.GetCommandLineArgs();
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o => {
                IndiServer server = new IndiServer(o.Host, o.Port);

                IndiConnection conn;
                if (server.TryConnect(out conn)) {
                    if (o.QueryDevices) {
                        ScanForDevices(conn);
                    }
                } else {
                    Console.WriteLine($"Failed to connect to INDI server at {o.Host}:{o.Port}");
                }
            });
        }

        private static void ScanForDevices(IndiConnection conn) {
            var subscriber = new IndiDeviceListener();
            conn.Subscribe(subscriber);
            subscriber.PrintScanning();
            conn.QueryProperties();
            // Spin while searching refresh search every x time
            while(true) {
                Thread.Sleep(TimeSpan.FromSeconds(4));
                conn.QueryProperties();
            }
        }
    }

    class IndiDeviceListener : BaseIndiListener {
        private int row;
        private int col;

        public IndiDeviceListener() {}

        public void Print(string text, string scanningText = "Searching...") {
            Console.WriteLine(text);

            PrintScanning(scanningText);
        }
        public void PrintScanning(string scanningText = "Searching...") {
            col = Console.CursorLeft;
            row = Console.CursorTop;
            Console.Write(scanningText);

            Console.SetCursorPosition(col, row);
        }

        public override void OnAddDevice(IndiDevice device) {
            Print(device.Name);
        }
    }
}
