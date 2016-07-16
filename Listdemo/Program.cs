using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Console;
using static System.Convert;

namespace Listdemo
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Title = "λ - Sourceruns listdemo by Traderain - λ";
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
                    var a = DemoParser.ParseDemo(datapath);
                    ForegroundColor = ConsoleColor.White;
                    bool valid = true;
                    #region print

                    switch (a.DemoT)
                    {
                        case DemoParseResult.DemoType.Goldsource:
                        {
                            #region GoldSource print
                            Console.WriteLine("Analyzed goldsource demo. Results:");
                            Console.WriteLine($@"
Demo name           :   {Path.GetFileName(datapath)}
Game name           :   {a.GameName}
Map name            :   {a.MapName}
Map CRC             :   {a.MapCrc}
Demo protocol       :   {a.Protocol}
Network protocol    :   {a.NProtocol}
Directory Offset    :   {a.DOffset}
Playback frames	    :   {a.Pframes}
Time                :   {a.TotalTime}s
");
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
                            break;
                        }
                        case DemoParseResult.DemoType.Source:
                        {
                            #region sourceprint
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
Playback frames		: {a.Pframes
                                    }
Playback ticks		: {a.Pticks}
Protocol		: {a.Protocol}
Network protocol	: {a.NProtocol
                                    }
Server Name		: {a.ServerName}
X			: {a.X}
Y			: {a.Y}
Z			: {a.Z}");
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
                    WriteLine();
                    #endregion
                    string convert = "";
                    if (valid)
                    {
                        #region rename
                    while (convert == "")
                    {
                        try
                        {
                            convert = ReadLine();
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
                            Console.WriteLine(e.Message);
                        }
                    }
                    #endregion
                    }
                    else
                    {
                        WriteLine("Press any key to exit...");
                        ReadKey();
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
 │                        │
 └────────────────────────┘
");
                        ForegroundColor = ConsoleColor.Red;
                        var k = 0;
                        do
                        {
                            try
                            {
                                k = ToInt32(ReadLine());
                            }
                            catch (Exception e)
                            {

                                if (e is FormatException)
                                {
                                    Write("Please use the given numbers");
                                }

                            }

                        } while (k < 0 || k > 4);
                        switch (k)
                        {
                            case 1:
                            {
                                Clear();
                                WriteLine("42");
                                break;
                            }
                            case 2:
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
                                ReadKey();
                            }
                            case 3:
                            {
                                Process.Start("http://www.sourceruns.org");
                                break;
                                Clear();
                            }
                            case 4:
                            {
                                WriteLine(@"
CBenni for the demoparser.
YaLTeR for the c++ version(idea).
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