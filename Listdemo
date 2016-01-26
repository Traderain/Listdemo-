using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using static System.Console;
using static System.Convert;

namespace Listdemo
{
    public class Program
    {
        private static string Fend(DemoParseResult c)
        {
            var z = new List<string>
            {
                c.GameName,
                c.PlayerName,
                c.TotalTicks.ToString(),
                c.TotalTime.ToString(CultureInfo.InvariantCulture).Replace('.',','),
                c.MapName,
                c.TotalJumps.ToString(),
                c.Pframes,
                c.Pticks,
                c.Protocol,
                c.NProtocol,
                c.ServerName,
                c.X.ToString(CultureInfo.InvariantCulture),
                c.Y.ToString(CultureInfo.InvariantCulture),
                c.Z.ToString(CultureInfo.InvariantCulture),
                $"{c.TotalTime: #,0.000}"
            };
            var qry = z.OrderByDescending(j => j.Length).First().Length + 3;
            const string temp = "                                      ";
            return temp.Substring(0, qry);
        }

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
            if (datapath == null) return;
            {
                if (Path.GetExtension(datapath) == ".dem")
                {
                    var a = DemoParser.ParseDemo(args[0]);
                    ForegroundColor = ConsoleColor.White;
                    var longeststring = Fend(a);
                    var bord = new string(longeststring.Select(r => '─').ToArray());
                    #region print
                    WriteLine("Analyzed demo. Results.");
                    WriteLine($@"
Demoname:		: {Path.GetFileName(datapath)}
GameName		: {a.GameName}
PlayerName		: {a.PlayerName}
Ticks			: {a.TotalTicks}
Time			: {a.TotalTime}s
MapName			: {a.MapName}
Jumps			: {a.TotalJumps}
Playback time		: {a.PTime: #,0.000}
Playback frames		: {a.Pframes}
Playback ticks		: {a.Pticks}
Protocol		: {a.Protocol}
Network protocol	: {a.NProtocol}
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
                                WriteLine("Detected {0} flag. Time to flag is:  {1}s in {2} ticks.", b.Type, b.Time, b.Ticks);
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
                    WriteLine();
                    int? convert = null;
                    while (convert == null)
                    {
                        try
                        {
                            convert = ToInt32(ReadLine());
                            File.Move(datapath, convert + "-" + a.MapName.Substring(3,a.MapName.Length-3) + "-" +
                                                $"{a.TotalTime:#,0.000}" + "-" + a.PlayerName + ".dem");
                        }
                        catch(Exception e)
                        {
                            if (e is FormatException)
                            {
                                //throw new Exception(e.Message);
                            }
                            // Log failure
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
}
