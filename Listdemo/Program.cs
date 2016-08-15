using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using ConsoleExtender;
using static System.Console;

namespace Listdemo
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Title = "λ - Sourceruns listdemo by Traderain - λ";
            ConsoleHelper.SetConsoleIcon(SystemIcons.Information);
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            var datapath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            try
            {
                datapath = args[0];
            }
            catch (Exception e)
            {
                if (e is IndexOutOfRangeException)
                {
                    datapath = Environment.CurrentDirectory;
                }
            }
            if (Path.GetExtension(datapath) == ".dem")
            {
                ForegroundColor = ConsoleColor.White;
                var valid = true;
                var demoType = DemoParser.DetermineDemoType(datapath);
                switch (demoType)
                {
                    case DemoParser.DEMO_TYPE.Goldsource:
                    {
                        var a = DemoParser.ParseGoldSourceDemo(datapath, demoType);

                        #region GoldSource print

                        WriteLine("Analyzed goldsource demo. Results:");
                        WriteLine(
                            $@"
Demo name           :   {Path.GetFileName(datapath)}
Map name            :   {
                                a.MapName}
Map CRC             :   {a.MapCrc}
Demo protocol       :   {a.Protocol
                                }
Network protocol    :   {a.NProtocol}
Directory Offset    :   {a.DOffset
                                }
Playback frames	    :   {a.Pframes}
Time                :   {a.TotalTime}s
");
                        valid = true;
                        if (a.Flags != null)
                        {
                            if (a.Flags.Count != 0)
                            {
                                WriteLine("-------------- Detected Flags ------------");
                                foreach (var b in a.Flags)
                                {
                                    ForegroundColor = ConsoleColor.Yellow;
                                    WriteLine("Detected {0} flag. Time to flag is:  {1}s in {2} ticks.", b.Type, b.Time,
                                        b.Ticks);
                                }
                            }
                            else
                            {
                                WriteLine("No save flags detected!");
                            }
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Green;
                            WriteLine("No flags detected!");
                        }
                        if (a.Cheated)
                        {
                            ForegroundColor = ConsoleColor.White;
                            WriteLine("----------------- Cheats ----------------");
                            ForegroundColor = ConsoleColor.Red;
                            WriteLine("The speedrun police is after you for: ");
                            Array.ForEach(a.Cheetz.ToArray(), WriteLine);
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.White;
                            Write("No cheats detected: ");
                            ForegroundColor = ConsoleColor.Green;
                            Write("OK");
                        }

                        #endregion

                        #region rename

                        WriteLine();
                        try
                        {
                            var convert = ReadLine();
                            var time = (a.Flags.Count(x => x.Type.Contains("SAVE")) == 0)
                                ? a.TotalTime.ToString("#,0.000")
                                : a.Flags.First(x => x.Type.Contains("SAVE")).Time.ToString("#,0.000");
                            File.Move(datapath, convert + "-" + a.MapName.Substring(3, a.MapName.Length - 3) + "-" +
                                                $"{time}" + "-" + Environment.UserName + ".dem");
                        }
                        catch (Exception e)
                        {
                            if (e is FormatException)
                            {
                                //TODO: fancy error
                            }
                            WriteLine(e.Message);
                        }

                        #endregion

                        break;
                    }
                    case DemoParser.DEMO_TYPE.Source:
                    {
                        #region sourceprint

                        var a = DemoParser.ParseDemo(datapath, demoType);
                        WriteLine(@"Analyzed source demo. Results.");
                        WriteLine(
                            $@"
Demoname:		: {Path.GetFileName(datapath)}
GameName		: {a.GameName}
PlayerName		: {
                                a.PlayerName}
Ticks			: {a.TotalTicks}
Time			: {a.TotalTime}s
MapName			: {a.MapName
                                }
Jumps			: {a.TotalJumps}
Playback time		: {a.PTime: #,0.000}
Playback frames		: {
                                a.Pframes
                                }
Playback ticks		: {a.Pticks}
Protocol		: {a.Protocol}
Network protocol	: {a.NProtocol
                                }
Server Name		: {a.ServerName}
X			: {a.CoordsList.Last().X}
Y			: {
                                a.CoordsList.Last().Y}
Z			: {a.CoordsList.Last().Z}");
                        if (a.Flags != null)
                        {
                            if (a.Flags.Count != 0)
                            {
                                WriteLine("-------------- Detected Flags ------------");
                                foreach (var b in a.Flags)
                                {
                                    ForegroundColor = ConsoleColor.Yellow;
                                    WriteLine("Detected {0} flag. Time to flag is:  {1}s in {2} ticks.", b.Type, b.Time,
                                        b.Ticks);
                                }
                            }
                            else
                            {
                                WriteLine("No save flags detected!");
                            }
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Green;
                            WriteLine("No flags detected!");
                        }
                        if (a.Cheated)
                        {
                            ForegroundColor = ConsoleColor.White;
                            WriteLine("----------------- Cheats ----------------");
                            ForegroundColor = ConsoleColor.Red;
                            WriteLine("The speedrun police is after you for: ");
                            Array.ForEach(a.Cheetz.ToArray(), WriteLine);
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.White;
                            Write("No cheats detected: ");
                            ForegroundColor = ConsoleColor.Green;
                            Write("OK");
                        }

                            #endregion

                        #region rename

                            WriteLine();
                        try
                        {
                            var convert = ReadLine();
                            var time = (a.Flags.Count(x => x.Type.Contains("SAVE")) == 0)
                                ? a.TotalTime.ToString("#,0.000")
                                : a.Flags.First(x => x.Type.Contains("SAVE")).Time.ToString("#,0.000");
                            File.Move(datapath, convert + "-" + a.MapName.Substring(3, a.MapName.Length - 3) + "-" +
                                                $"{time}" + "-" + a.PlayerName + ".dem");
                        }
                        catch (Exception e)
                        {
                            if (e is FormatException)
                            {
                                //TODO: fancy error
                            }
                            WriteLine(e.Message);
                        }

                        #endregion

                        break;
                    }
                    default:
                    {
                        valid = false;
                        WriteLine("I am really sorry but we can't analyze your demo. :\\");
                        WriteLine("Be sure to make an issue at \"https://github.com/Traderain/Listdemo-\"");
                        break;
                    }
                }
            }
            else
            {
                #region nofile

                do
                {
                    Clear();
                    ForegroundColor = ConsoleColor.Cyan;
                    WriteLine("(Incorrect demo file/You didn't drag&drop!)");
                    WriteLine(@"
 ┌────────────────────────┐
 │Menu:                   │
 │1 :  Meaning of life.   │
 │2 :  Black mesa logo    │
 │3 :  Sourceruns.org     │
 │4 :  Credits            │
 │Esc: Exit               │
 └────────────────────────┘
");
                    ForegroundColor = ConsoleColor.Red;
                    var k = ConsoleKey.Escape;
                    do
                    {
                        try
                        {
                            k = ReadKey().Key;
                        }
                        catch (Exception e)
                        {
                            if (e is FormatException)
                            {
                                Environment.Exit(0x01);
                            }
                        }
                        if (k == ConsoleKey.Escape)
                        {
                            Environment.Exit(0x01);
                        }
                    } while (k != ConsoleKey.D1 && k != ConsoleKey.D2 && k != ConsoleKey.D3 && k != ConsoleKey.D4);
                    switch (k)
                    {
                        case ConsoleKey.D1:
                        {
                            Clear();
                            WriteLine("42");
                            break;
                        }
                        case ConsoleKey.D2:
                        {
                            Clear();

                            #region bms

                            WriteLine(@"
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
                        }
                        case ConsoleKey.D3:
                        {
                            Clear();
                            Process.Start("https://sourceruns.org");
                            break;
                        }
                        case ConsoleKey.D4:
                        {
                            Clear();
                            WriteLine(@"
CBenni for the source parser.
YaLTeR for the the goldsource and Half-Life:Source parser.
Centaur1um for this awesome community.");
                            break;
                        }
                        default:
                            break;
                    }
                } while (ReadKey(true).Key != ConsoleKey.Escape);

                #endregion
            }
        }
    }
}