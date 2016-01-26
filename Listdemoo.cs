using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Listdemo
{
    public class Flag
    {
        public string Ticks { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }

        public Flag(int t, float s, string type)
        {
            Ticks = Convert.ToString(t);
            Time = Convert.ToString(s, CultureInfo.InvariantCulture) + "s";
            Type = type;
        }
    }

    public class DemoParser
    {
        public static DemoParseResult ParseDemo(string file)
        {
            string[] cheats = { "host_timescale", "god", "sv_cheats", "buddha", "host_framerate", "sv_accelerate", "sv_airaccelerate", "noclip", "ent_fire" };
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
                result.Pticks = (Math.Abs(BitConverter.ToInt32(br.ReadBytes(4), 0))).ToString(CultureInfo.InvariantCulture);
                result.Pframes = (Math.Abs(BitConverter.ToInt32(br.ReadBytes(4), 0))).ToString(CultureInfo.InvariantCulture);
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
                                    result.Flags.Add(new Flag(tick, tick*0.015f, "#SAVE#"));
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
                                    result.Flags.Add(new Flag(tick, tick*0.015f, "autosave"));
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
        public List<Flag> Flags { get; set; }
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
            CrosshairAppearTick = -1;
            CrosshairDisappearTick = -1;
        }
    }
}
