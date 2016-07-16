using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Text.Encoding;
using static System.Math;
using static System.BitConverter;
using static System.Globalization.CultureInfo;

namespace Listdemo
{
    public class Flag
    {
        public int Ticks { get; set; }
        public float Time { get; set; }
        public string Type { get; set; }

        public Flag(int t, float s, string type) //Flags like #SAVE# (in segmented).
        {
            Ticks = t;
            Time = s;
            Type = type;
        }
    }


    public class DemoParser
    {
        #region Pinvoke <3 YaLTeR
        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern demo_header HLDEMO_DemoFileGetDemoHeader(IntPtr demofile);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr HLDEMO_Open([MarshalAs(UnmanagedType.LPStr)] string lpString, int readFrame);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void HLDEMO_Close(IntPtr demofile);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool HLDEMO_IsValidDemo([MarshalAs(UnmanagedType.LPStr)] string lpString);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool HLDEMO_DemoFileDidReadFrames([MarshalAs(UnmanagedType.LPStr)] string lpString);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void HLDEMO_DemoFileReadFrames(IntPtr demofile);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int HLDEMO_GetDirectoryEntryCount(IntPtr demofile);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern demo_directory_entry HLDEMO_GetDirectoryEntry(IntPtr demofile, int index);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int HLDEMO_GetFrameCount(IntPtr frame_data);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern demo_frame HLDEMO_GetFrame(IntPtr frame_data, int index);

        [DllImport("HLDemo.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern console_command_frame HLDEMO_TreatAsConsoleCommandFrame(IntPtr frame_pointer);

        [StructLayout(LayoutKind.Sequential)]
        public struct demo_header
        {
            public int netProtocol;
            public int demoProtocol;
            public IntPtr mapName;
            public IntPtr gameDir;
            public int mapCRC;
            public int directoryOffset;
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct demo_directory_entry
        {
            public int type;
            public IntPtr description;
            public int flags;
            public int CDTrack;
            public float trackTime;
            public int frameCount;
            public int offset;
            public int fileLength;
            // Pass this to relevant functions.
            public IntPtr frame_data;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct demo_frame
        {
            public int type;
            public float time;
            public int frame;
            // Pass this to HLDEMO_TreatAs<x>Frame().
            public IntPtr frame_pointer;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct console_command_frame
        {
            public IntPtr command;
        }
        public enum demo_frame_type
        {
            DEMO_START = 2,
            CONSOLE_COMMAND = 3,
            CLIENT_DATA = 4,
            NEXT_SECTION = 5,
            EVENT = 6,
            WEAPON_ANIM = 7,
            SOUND = 8,
            DEMO_BUFFER = 9
        }

        #endregion
        public static DemoParseResult ParseDemo(string file)
        {
            string[] cheats = { "host_timescale", "god", "sv_cheats", "buddha", "host_framerate", "sv_accelerate", "sv_airaccelerate", "noclip", "ent_fire" };
            var result = new DemoParseResult();
            var type = "";
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs)){type = ASCII.GetString(br.ReadBytes(8)).TrimEnd('\0');}   
                switch (type)
                    {
                        case "HL2DEMO": // Bug: not checking for missbehaving demos.  
                            {
                                result.DemoT = DemoParseResult.DemoType.Source;
                                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                                using (var br = new BinaryReader(fs))
                                {
                                    #region Original HL2 Demo Parser
                            br.ReadBytes(8);
                                result.Protocol = (ToInt32(br.ReadBytes(4), 0)).ToString(InvariantCulture);
                                result.NProtocol = (ToInt32(br.ReadBytes(4), 0)).ToString(InvariantCulture);
                                result.ServerName = ASCII.GetString(br.ReadBytes(260)).TrimEnd('\0');
                                result.PlayerName = ASCII.GetString(br.ReadBytes(260)).TrimEnd('\0');
                                result.MapName = ASCII.GetString(br.ReadBytes(260)).TrimEnd('\0');
                                result.GameName = ASCII.GetString(br.ReadBytes(260)).TrimEnd('\0'); // gamedir=gamename

                                result.PTime = (Abs(ToInt32(br.ReadBytes(4), 0))).ToString(InvariantCulture);
                                result.Pticks = (Abs(ToInt32(br.ReadBytes(4), 0))).ToString(InvariantCulture);
                                result.Pframes = (Abs(ToInt32(br.ReadBytes(4), 0))).ToString(InvariantCulture);
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
                                                var concmd = ASCII.GetString(br.ReadBytes(concmdLen - 1));
                                                if (concmd.Contains("#SAVE#"))
                                                {
                                                    if (tick >= 0)
                                                    {
                                                        result.Flags.Add(new Flag(tick, tick * 0.015f, "#SAVE#"));
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
                                                        result.Flags.Add(new Flag(tick, tick * 0.015f, "autosave"));
                                                    }
                                                }
                                                if (concmd.StartsWith("+jump")) result.TotalJumps++;
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
                            #endregion
                                }       
                                break;
                            }
                        case "HLDEMO": //BUG: "Half-Life:Source" Demos crash it.
                            {
                            #region Goldsource demo parser
                            result.Flags = new List<Flag>();
                            result.Cheetz = new List<string>();
                                if (HLDEMO_IsValidDemo(file))
                                {
                                     result.DemoT = DemoParseResult.DemoType.Goldsource;
                                     IntPtr demoFile = HLDEMO_Open(file, 1);
                                     demo_header demoFileDemoHeader = HLDEMO_DemoFileGetDemoHeader(demoFile);
                                     result.MapName = Marshal.PtrToStringAnsi(demoFileDemoHeader.mapName);
                                     result.Protocol = demoFileDemoHeader.demoProtocol.ToString();
                                     result.MapCrc = demoFileDemoHeader.mapCRC.ToString();
                                     result.DOffset = demoFileDemoHeader.directoryOffset.ToString();
                                     result.NProtocol = demoFileDemoHeader.netProtocol.ToString();
                                     result.GameName = "Half-Life";
                                     result.Gamedir = Marshal.PtrToStringAnsi(demoFileDemoHeader.gameDir);
                                    var size = HLDEMO_GetDirectoryEntryCount(demoFile);
                                    for (int i = 0; i < size; i++)
                                    {
                                        demo_directory_entry currentDemoDirectoryEntry =
                                            HLDEMO_GetDirectoryEntry(demoFile, i);
                                        int frameCount = HLDEMO_GetFrameCount(currentDemoDirectoryEntry.frame_data);
                                        for (int j = 0; j < frameCount; j++)
                                        {
                                            demo_frame currFrame = HLDEMO_GetFrame(
                                                currentDemoDirectoryEntry.frame_data, j);
                                            if (currFrame.type == (int)demo_frame_type.CONSOLE_COMMAND)
                                            {
                                                var command = Marshal.PtrToStringAnsi(HLDEMO_TreatAsConsoleCommandFrame(currFrame.frame_pointer).command);
                                                result.TotalTime = currFrame.time;
                                                //Console.WriteLine(command);
                                                result.Pframes = currFrame.frame.ToString();
                                                if (command != null)
                                                    foreach (var s in cheats.Where(command.Contains))
                                                    {
                                                        result.Cheated = true;
                                                        result.Cheetz.Add(command);
                                                    }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    result.DemoT = DemoParseResult.DemoType.Nonsupported;
                                }
                                #endregion
                        break;
                            }
                        default: // Set everything to blank and print that we don't know how to deal with this.
                            {
                                result.DemoT = DemoParseResult.DemoType.Nonsupported;
                                #region Game not supported

                                Console.WriteLine("Game not supported :/");
                                Console.WriteLine();
                                result.Cheated = false;
                                result.Cheetz = new List<string>();
                                result.CrosshairAppearTick = 0;
                                result.CrosshairDisappearTick = 0;
                                result.TotalTicks = 0;
                                result.TotalTime = 0;
                                result.MapName = "-";
                                result.PlayerName = "-";
                                result.PlayerName = "-";
                                result.PTime = "-";
                                result.TotalJumps = 0;
                                result.Pframes = "-";
                                result.Pticks = "-";
                                result.Protocol = "-";
                                result.NProtocol = "-";
                                result.ServerName = "-";
                                result.X = 0;
                                result.Y = 0;
                                result.Z = 0;
                                #endregion
                                break;
                            }
                }
                result.TotalTime = result.TotalTicks * 0.015f; // 1 tick = 0.015s
                return result;
            }
    }

    [Serializable]
    public class DemoParseResult
    {
        public bool Cheated { get; set; } = false;
        public List<string> Cheetz { get; set; } 
        public List<Flag> Flags { get; set; }
        public int CrosshairAppearTick { get; set; }
        public int CrosshairDisappearTick { get; set; }
        public int TotalTicks { get; set; } = 0;
        public float TotalTime { get; set; } = 0;
        public string MapName { get; set; } = "-";
        public string PlayerName { get; set; } = "-";
        public string GameName { get; set; } = "-";
        public int TotalJumps { get; set; } = 0;
        public string PTime { get; set; } = "-";
        public string Pframes { get; set; } = "-";
        public string Pticks { get; set; } = "-";
        public string Protocol { get; set; } = "-";
        public string NProtocol { get; set; } = "-";
        public string ServerName { get; set; } = "-";
        public string MapCrc { get; set; } = "-";
        public string Gamedir { get; set; } = "-";
        public string DOffset { get; set; } = "-";
        public float X { get; set; } = 0f;
        public float Y { get; set; } = 0f;
        public float Z { get; set; } = 0f;
        public enum DemoType
        {
            Source,
            Goldsource,
            Nonsupported
        }
        public DemoType DemoT{get;set;} = DemoType.Nonsupported;
        public DemoParseResult()
        {
            CrosshairAppearTick = -1;
            CrosshairDisappearTick = -1;
        }
    }
}
