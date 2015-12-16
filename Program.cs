using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Listdemo
{
    public class Flag
    {
        public string Ticks{get;set;}
        public string Time{get;set;}
        public string Type{get;set;}

        public Flag(int t, float s, string type)
        {
            Ticks = Convert.ToString(t);
            Time = Convert.ToString(s)+ "s";
            Type = type;
        }
    }
    class Mainthing
    {
        static string Fend(DemoParseResult c)
        {
            var z = new List<string>
            {
                c.GameName.ToString(),
                c.PlayerName.ToString(),
                c.TotalTicks.ToString(),
                c.TotalTime.ToString(),
                c.MapName.ToString(),
                c.TotalJumps.ToString(),
                c.Pframes.ToString(),
                c.Pticks.ToString(),
                c.Protocol.ToString(),
                c.NProtocol.ToString(),
                c.ServerName.ToString(),
                c.X.ToString(),
                c.Y.ToString(),
                c.Z.ToString(),
                String.Format("{0: #,0.000}", c.PTime)
            };
            var qry = z.OrderByDescending(j => j.Length).First().Length + 3;
            var temp = "                                      ";
            Console.WriteLine(qry);
            return temp.Substring(0,qry);
        }
        static void Main(string[] args)
        {
            Console.Title = "λ - Sourceruns listdemo by Traderain - λ";
            var datapath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            try
            {
                datapath = args[0];
               
            }
            catch (Exception e)
            {

                if(e is IndexOutOfRangeException)
                {
                    datapath = Environment.CurrentDirectory;
                }
            }
            if (datapath != null)
            {
                if (Path.GetExtension(datapath.ToString()) == ".dem")
                {
                    var a = DemoParser.ParseDemo(args[0].ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    string longeststring = Fend(a);
                    string bord = new string(longeststring.Select(r => '─').ToArray());
                    //im lazy to do convert.toint and all that shit with vars + formatting but its basically done
                    Console.WriteLine("Analyzed demo. Results.");
                    Console.WriteLine("Demoname:\t\t: {15}\nGameName\t\t: {0}\nPlayerName\t\t: {1}\nTicks\t\t\t: {2}\nTime\t\t\t: {3}s\nMapName\t\t\t: {4}\nJumps\t\t\t: {5}\nPlayback time\t\t: {6}\nPlayback frames\t\t: {7}\nPlayback ticks\t\t: {8}\nProtocol\t\t: {9}\nNetwork protocol\t: {10}\nServer Name\t\t: {11}\nX\t\t\t: {12}\nY\t\t\t: {13}\nZ\t\t\t: {14}", a.GameName, a.PlayerName, a.TotalTicks, a.TotalTime, a.MapName, a.TotalJumps, String.Format("{0: #,0.000}", a.PTime), a.Pframes, a.Pticks, a.Protocol, a.NProtocol, a.ServerName, a.X, a.Y, a.Z,Path.GetFileName(datapath));
                    if (a.Flags != null)
                    {
                        if (a.Flags.Count != 0)
                        {
                            Console.WriteLine("-------------- Detected Flags ------------");
                            foreach (var b in a.Flags)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Detected {0} flag. Time to flag is:  {1}s in {2} ticks.", b.Type, b.Time, b.Ticks);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No save flags detected!");
                        }

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("No flags detected!");
                    }
                    if (a.Cheated)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("----------------- Cheats ----------------");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The speedrun police is after you for: ");
                        Array.ForEach(a.Cheetz.ToArray(),Console.WriteLine);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("No cheats detected: ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("OK");
                    }

                }
                else
                {
                    do
                    {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("(Incorrect demo file/You didn't drag&drop!)");
                    Console.WriteLine(@"
 ┌────────────────────────┐
 │Menu:                   │
 │1 :  Meaning of life.   │
 │2 :  Black mesa logo    │
 │3 :  Sourceruns.org     │
 │4 :  Credits            │
 │                        │
 └────────────────────────┘
");
                        Console.ForegroundColor = ConsoleColor.Red;
                    int k = 0;
                    do
                    {
                        try
                        {
                            k = Convert.ToInt32(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            
                            if(e is FormatException)
                            {
                                Console.Write("Please use the given numbers");
                            }
                            
                        }
                         
                    } while (k<0 || k>4);
                    switch (k)
                    {
                        case 1: 
                        {
                            Console.Clear();
                            Console.WriteLine("42");
                            break;
                        }
                        case 2: 
                        {
                            Console.Clear();
                            #region bms
                            Console.WriteLine(@"
                    .-;+$XHHHHHHX$+;-.
	        ,;X@@X%/;=----=: /%X@@X/
	      =$@@%=.              .=+H@X: 
	    -XMX:                       =XMX=
	   /@@:                           =H@+
	  %@X,                            .$@$
	 +@X.                               $@%
	-@@,                                .@@=
	%@%                                  +@$
	H@:                                   : @H
	H@:          : HHHHHHHHHHHHHHHHHHX,    =@H
	%@%         ;@M@@@@@@@@@@@@@@@@@H-   +@$
	=@@,        : @@@@@@@@@@@@@@@@@@@@@= .@@: 
	 +@X        : @@@@@@@@@@@@@@@M@@@@@@: %@%
	  $@$,      ;@@@@@@@@@@@@@@@@@M@@@@@@$.
	   +@@HHHHHHH@@@@@@@@@@@@@@@@@@@@@@@+
	    =X@@@@@@@@@@@@@@@@@@@@@@@@@@@@X=
	      : $@@@@@@@@@@@@@@@@@@@M@@@@$: 
	        ,;$@@@@@@@@@@@@@@@@@@X/-
	           .-;+$XXHHHHHX$+;-.");
                            break;
                            #endregion
                            Console.ReadKey();
                        }
                        case 3: 
                        {
                            System.Diagnostics.Process.Start("http://www.sourceruns.org");
                            
                            break;
                            Console.Clear();
                        }
                        case 4: 
                        {
                            Console.WriteLine(@"
CBenni for the demoparser.
YaLTeR for the c++ version(idea).
Centaur1um for this awesome community.");
                            break;
                        }
                        default: 
                            break;
                    }
                    } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                    
                }
            }
            
            Console.ReadKey();
        }
    }
    public class DemoParser
    {
        public static DemoParseResult ParseDemo(string file)
        {
            string[] cheats = { "host_timescale", "god","sv_cheats", "buddha", "host_framerate", "sv_accelerate", "sv_airaccelerate", "noclip", "ent_fire", };
            var result = new DemoParseResult();
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                var identifier = Encoding.ASCII.GetString(br.ReadBytes(8)).TrimEnd('\0'); // skip identifier
                if (identifier != "HL2DEMO")
                    throw new Exception("Not a demo");
                result.Protocol = (BitConverter.ToInt32(br.ReadBytes(4), 0)).ToString(CultureInfo.InvariantCulture);
                result.NProtocol = (BitConverter.ToInt32(br.ReadBytes(4), 0)).ToString(CultureInfo.InvariantCulture);
                result.ServerName = Encoding.ASCII.GetString(br.ReadBytes(260)).TrimEnd('\0');
                result.PlayerName = Encoding.ASCII.GetString(br.ReadBytes(260)).TrimEnd('\0');
                result.MapName = Encoding.ASCII.GetString(br.ReadBytes(260)).TrimEnd('\0');
                result.GameName = Encoding.ASCII.GetString(br.ReadBytes(260)).TrimEnd('\0'); // gamedir=gamename

                result.PTime = (Math.Abs(BitConverter.ToInt32(br.ReadBytes(4), 0))).ToString(CultureInfo.InvariantCulture);
                result.Pticks = (Math.Abs(BitConverter.ToInt32(br.ReadBytes(4),0))).ToString(CultureInfo.InvariantCulture);
                result.Pframes = (Math.Abs(BitConverter.ToInt32(br.ReadBytes(4),0))).ToString(CultureInfo.InvariantCulture);
                var signOnLen = br.ReadInt32();
                result.Flags = new List<Flag>();
                result.Cheetz = new List<string>();

                byte command;
                do
                {
                    command = br.ReadByte();

                    if (command == 0x07) // dem_stop
                        break;

                    var tick = br.ReadInt32();
                    if (tick >= 0)
                    {
                        result.TotalTicks = tick;
                    }


                    switch (command)
                    {
                        case 0x01: 
                            br.BaseStream.Seek(signOnLen, SeekOrigin.Current);
                            break;
                        case 0x02: 
                        {
                            br.BaseStream.Seek(4, SeekOrigin.Current); // skip flags

                            var x = br.ReadSingle();
                            var y = br.ReadSingle();
                            var z = br.ReadSingle();
                            result.X = x;
                            result.Y = y;
                            result.Z = z;

                            br.BaseStream.Seek(0x44, SeekOrigin.Current);

                            var packetLen = br.ReadInt32();
                            br.BaseStream.Seek(packetLen, SeekOrigin.Current);
                        }
                            break;
                        case 0x04: 
                        {
                            var concmdLen = br.ReadInt32();
                            var concmd = Encoding.ASCII.GetString(br.ReadBytes(concmdLen - 1));
                            //Handling that damm save flag everyone asked about
                                if (concmd.Contains("#SAVE#"))
                                {
                                    if (tick >= 0)
                                    {
                                        result.Flags.Add(new Flag(tick,tick*0.015f,"#SAVE#"));
                                    } 
                                }
                            foreach (var s in cheats.Where(concmd.Contains))
                            {
                                result.Cheated = true;
                                result.Cheetz.Add(concmd);
                            }
                            if (concmd == "autosave")
                            {
                                //Autosave happened.
                                if (tick >= 0)
                                {
                                    result.Flags.Add(new Flag(tick,tick*0.015f,"autosave"));
                                }
                            }
                            // haaaaaaaaaaack
                            if (result.MapName == "escape_02" && concmd == "startneurotoxins 99999")
                            {
                                result.CrosshairDisappearTick = tick + 1;
                            }
                            if (concmd.StartsWith("+jump")) result.TotalJumps++;
                            //ADD ALL THE COMMANDS YOU WANT
                            br.BaseStream.Seek(1, SeekOrigin.Current); // skip null terminator
                        }
                            break;
                        case 0x05: 
                        {
                            br.BaseStream.Seek(4, SeekOrigin.Current); // skip sequence//int test = br.ReadInt32();
                            var userCmdLen = br.ReadInt32();
                            br.BaseStream.Seek(userCmdLen, SeekOrigin.Current);
                        }
                            break;
                        case 0x08: 
                        {
                            var stringTableLen = br.ReadInt32();
                            br.BaseStream.Seek(stringTableLen, SeekOrigin.Current);
                        }
                            break;
                        
                    }

                } while (command != 0x07); // dem_stop
            }
            result.TotalTime = result.TotalTicks * 0.015f;
            return result;
        }
    }

    [Serializable]
    public class DemoParseResult
    {
        public bool Cheated { get; set; }
        public List<string> Cheetz { get; set; } 
        public List<Flag> Flags{ get; set; }
        public int CrosshairAppearTick { get; set; }
        public int CrosshairDisappearTick { get; set; }
        public int TotalTicks { get; set; }
        public float TotalTime { get; set; }
        public string MapName { get; set; }
        public string PlayerName { get; set; }
        public string GameName { get; set; }
        public int TotalJumps { get; set; }
        public string PTime { get; set; }
        public string Pframes { get; set; }
        public string Pticks { get; set; }
        public string Protocol { get; set; }
        public string NProtocol { get; set; }
        public string ServerName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public DemoParseResult()
        {
            this.CrosshairAppearTick = -1;
            this.CrosshairDisappearTick = -1;
        }
    }
}
