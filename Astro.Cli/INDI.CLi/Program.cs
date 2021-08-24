using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CommandLine;
using Qkmaxware.Astro.Control;

namespace Qkmaxware.Astro.Cli {

    class Options {
        [Option('h', "host", Required = true, HelpText = "INDI server host ip")]
        public string Host {get; set;}
        [Option('p', "port", Required = false, HelpText = "INDI server host port", Default=7624)]
        public int Port {get; set;}
    }

    /*
        Example Usage:
        > indi -h 10.0.0.145 -p 7624
        SEND: <getProperties version="1.7" />
    */
    class IndiCli {
        static void Main() {
            var args = System.Environment.GetCommandLineArgs();
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o => {
                IndiServer server = new IndiServer(o.Host, o.Port);

                IndiConnection conn;
                if (server.TryConnect(out conn)) {
                    // Create separate writer window 
                    var now = DateTime.Now.ToString("dd-MM-yyy HH.mm.ss");
                    using (var fs = new StreamWriter($"{now}.indi.log")) {
                        conn.OutputStream = fs;
                        
                        string command;
                        Console.Write("SEND: ");
                        while ((command = Console.ReadLine()) != "exit") {
                            conn.SendXml(command);
                            Console.Write("SEND: ");
                        }
                    }
                } else {
                    Console.WriteLine($"Failed to connect to INDI server at {o.Host}:{o.Port}");
                }
            });
        }
    }
}
