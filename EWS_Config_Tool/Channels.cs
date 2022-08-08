using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace EWS_Config_Tool
{
    public static class Channel_File_Utils
    {
        /// <summary>
        /// TODO using test code for now, this will come from dvg and current config
        /// </summary>
        public static void Create_Channel_file()
        {
            Channels ch = new Channels();

            Frequency_Record fi = new Frequency_Record { FREQUENCY = 100000, LANGUAGE = "ENG", LEVEL = 100, AUTO_LEVEL="NO" };
            ch.INCLUDE.Add(fi);
            Frequency_Record fi2 = new Frequency_Record { FREQUENCY = 102000, LANGUAGE = "FRE", LEVEL = 100, AUTO_LEVEL = "NO" };
            ch.INCLUDE.Add(fi2);

            
            ch.EXCLUDE.Add(1000);

            ch.AUTOMATIC.ACTIVE = "YES";
            ch.AUTOMATIC.LEVEL = 100;

            // serialize JSON to a string and then write string to a file, formatted
            File.WriteAllText(@"c:\Config\channels.json", JsonConvert.SerializeObject(ch, Formatting.Indented));
        }

        /// <summary>
        /// used to read the channels.json file and then populate the dgv of instructions
        /// </summary>
        /// <returns></returns>
        public static Channels Read_Channel_file()
        {
            // read file into a string and deserialize JSON to a type
            Channels ch = JsonConvert.DeserializeObject<Channels>(File.ReadAllText(@"c:\Config\channels.json"));

            return ch;
        }
    }

    static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }

    /// <summary>
    /// Testing
    /// </summary>
    public class Channel : ICloneable
    {
        public Channels CHANNELS = new Channels();

        [JsonProperty]
        public string CFG_VERSION = "";

        public Channel()
        { }

        public Object Clone ()
        {
            return new Channel { CHANNELS = new Channels() };
        }
    }

    /// <summary>
    /// Turns into channels.json
    /// </summary>
    public class Channels : ICloneable
    {
        public BindingList<Frequency_Record> INCLUDE = new BindingList<Frequency_Record>();
        public BindingList<int> EXCLUDE = new BindingList<int>();
        public Automatic_Record AUTOMATIC = new Automatic_Record();
        public Enhanced_Record ENHANCED = new Enhanced_Record();

        public Channels() { }

        public Object Clone()
        {
            return new Channels { INCLUDE = new BindingList<Frequency_Record>(this.INCLUDE.ToList()), AUTOMATIC = (Automatic_Record)this.AUTOMATIC.Clone(), ENHANCED = (Enhanced_Record)this.ENHANCED.Clone(), EXCLUDE = new BindingList<int>(this.EXCLUDE.ToList()) };
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Frequency_Record : ICloneable
    {
        /// <summary>
        /// Generate all int ie 100000, 101000, 101500
        /// </summary>
        [JsonProperty]
        public int FREQUENCY { get; set; }

        public string FREQUENCY_MHz
        {
            get
            {
                double freq = this.FREQUENCY / 1000.0;
                string s = string.Format("{0:N2}", freq);
                return s;
            }
        }

        [JsonProperty]
        public string LANGUAGE { get; set; }

        [JsonProperty]
        public int LEVEL { get; set; }

        [JsonProperty]
        public string AUTO_LEVEL { get; set; }

        [JsonProperty]
        public string POPULAR { get; set; }

        [JsonProperty]
        public int TA { get; set; }

        public override string ToString()
        {
            return "Freq: " + FREQUENCY.ToString() + " Language: " + LANGUAGE + " LEVEL: " + LEVEL + " POPULAR: " + POPULAR + "TA: " + TA;
        }

        public Object Clone()
        {
            return new Frequency_Record { FREQUENCY = this.FREQUENCY, LANGUAGE = this.LANGUAGE, LEVEL = this.LEVEL, AUTO_LEVEL = this.AUTO_LEVEL, POPULAR = this.POPULAR, TA = this.TA};
        }

    }

    /// <summary>
    /// No list, single entry
    /// </summary>
    public class Automatic_Record : ICloneable
    {
        public string ACTIVE { get; set; }
        public int LEVEL { get; set; }
        public string AUTO_LEVEL { get; set; }

        public Automatic_Record()
        {
            ACTIVE = "YES";
            LEVEL = 100;
            AUTO_LEVEL = "NO";
        }

        public Object Clone()
        {
            return new Automatic_Record { ACTIVE = this.ACTIVE, LEVEL = this.LEVEL, AUTO_LEVEL = this.AUTO_LEVEL };
        }
    }

    /// <summary>
    /// No list, single entry
    /// </summary>
    public class Enhanced_Record : ICloneable
    {
        public string ACTIVE { get; set; }
        public int REPEAT { get; set; }

        public Enhanced_Record()
        {
            ACTIVE = "NO";
            REPEAT = 1;
        }

        public Object Clone()
        {
            return new Enhanced_Record { ACTIVE = this.ACTIVE, REPEAT = this.REPEAT };
        }
    }
}
