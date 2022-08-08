using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;    //TODO check
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EWS_Config_Tool
{
    public static class Languages
    {

        public static void create_language_file(string path)
        {
            /*
            Id Ref_Name
            aar Afar    
            abk Abkhazian
            */
            BindingList<Language_Record> llr = new BindingList<Language_Record>();

            // Language_Record lr = new Language_Record { Id = "aar", Ref_Name = "Afar" };
            // Add all from file Ivan sent
            llr.Add(new Language_Record("UND", "Undetermined"));
            llr.Add(new Language_Record("afr", "Afrikaans"));
            llr.Add(new Language_Record("amh", "Amharic"));
            llr.Add(new Language_Record("ara", "Arabic"));
            llr.Add(new Language_Record("aze", "Azerbaijani"));
            llr.Add(new Language_Record("bel", "Belarusian"));
            llr.Add(new Language_Record("ben", "Bengali"));
            llr.Add(new Language_Record("bos", "Bosnian"));
            llr.Add(new Language_Record("bul", "Bulgarian"));
            llr.Add(new Language_Record("cat", "Catalan"));
            llr.Add(new Language_Record("ceb", "Cebuano"));
            llr.Add(new Language_Record("ces", "Czech"));
            llr.Add(new Language_Record("cos", "Corsican"));
            llr.Add(new Language_Record("dan", "Danish"));
            llr.Add(new Language_Record("deu", "German"));
            llr.Add(new Language_Record("ell", "Greek"));
            llr.Add(new Language_Record("eng", "English"));
            llr.Add(new Language_Record("epo", "Esperanto"));
            llr.Add(new Language_Record("est", "Estonian"));
            llr.Add(new Language_Record("eus", "Basque"));
            llr.Add(new Language_Record("fas", "Persian"));
            llr.Add(new Language_Record("fil", "Filipino"));
            llr.Add(new Language_Record("fin", "Finnish"));
            llr.Add(new Language_Record("fra", "French"));
            llr.Add(new Language_Record("gle", "Irish"));
            llr.Add(new Language_Record("glg", "Galician"));
            llr.Add(new Language_Record("guj", "Gujarati"));
            llr.Add(new Language_Record("hat", "Haitian"));
            llr.Add(new Language_Record("hau", "Hausa"));
            llr.Add(new Language_Record("haw", "Hawaiian"));
            llr.Add(new Language_Record("heb", "Hebrew"));
            llr.Add(new Language_Record("hrv", "Croatian"));
            llr.Add(new Language_Record("hun", "Hungarian"));
            llr.Add(new Language_Record("hye", "Armenian"));
            llr.Add(new Language_Record("ibo", "Igbo"));
            llr.Add(new Language_Record("ind", "Indonesian"));
            llr.Add(new Language_Record("isl", "Icelandic"));
            llr.Add(new Language_Record("ita", "Italian"));
            llr.Add(new Language_Record("jav", "Javanese"));
            llr.Add(new Language_Record("jpn", "Japanese"));
            llr.Add(new Language_Record("kan", "Kannada"));
            llr.Add(new Language_Record("kat", "Georgian"));
            llr.Add(new Language_Record("kaz", "Kazakh"));
            llr.Add(new Language_Record("khm", "Central Khmer"));
            llr.Add(new Language_Record("kor", "Korean"));
            llr.Add(new Language_Record("kur", "Kurdish"));
            llr.Add(new Language_Record("lao", "Lao"));
            llr.Add(new Language_Record("lat", "Latin"));
            llr.Add(new Language_Record("lav", "Latvian"));
            llr.Add(new Language_Record("lit", "Lithuanian"));
            llr.Add(new Language_Record("mal", "Malayalam"));
            llr.Add(new Language_Record("mar", "Marathi"));
            llr.Add(new Language_Record("mlg", "Malagasy"));
            llr.Add(new Language_Record("mlt", "Maltese"));
            llr.Add(new Language_Record("mon", "Mongolian"));
            llr.Add(new Language_Record("mri", "Maori"));
            llr.Add(new Language_Record("msa", "Malay(macrolanguage)"));
            llr.Add(new Language_Record("mya", "Burmese"));
            llr.Add(new Language_Record("nep", "Nepali(macrolanguage)"));
            llr.Add(new Language_Record("nld", "Dutch"));
            llr.Add(new Language_Record("nor", "Norwegian"));
            llr.Add(new Language_Record("pol", "Polish"));
            llr.Add(new Language_Record("por", "Portuguese"));
            llr.Add(new Language_Record("pst", "Pashto"));
            llr.Add(new Language_Record("ron", "Romanian"));
            llr.Add(new Language_Record("rus", "Russian"));
            llr.Add(new Language_Record("sad", "Sandawe"));
            llr.Add(new Language_Record("sag", "Sango"));
            llr.Add(new Language_Record("sah", "Yakut"));
            llr.Add(new Language_Record("sam", "Samaritan Aramaic"));
            llr.Add(new Language_Record("san", "Sanskrit"));
            llr.Add(new Language_Record("sas", "Sasak"));
            llr.Add(new Language_Record("sat", "Santali"));
            llr.Add(new Language_Record("scn", "Sicilian"));
            llr.Add(new Language_Record("sco", "Scots"));
            llr.Add(new Language_Record("sel", "Selkup"));
            llr.Add(new Language_Record("sga", "Old Irish(to 900)"));
            llr.Add(new Language_Record("shn", "Shan"));
            llr.Add(new Language_Record("sid", "Sidamo"));
            llr.Add(new Language_Record("sin", "Sinhala"));
            llr.Add(new Language_Record("slk", "Slovak"));
            llr.Add(new Language_Record("slv", "Slovenian"));
            llr.Add(new Language_Record("sma", "Southern Sami"));
            llr.Add(new Language_Record("sme", "Northern Sami"));
            llr.Add(new Language_Record("smj", "Lule Sami"));
            llr.Add(new Language_Record("smn", "Inari Sami"));
            llr.Add(new Language_Record("smo", "Samoan"));
            llr.Add(new Language_Record("sms", "Skolt Sami"));
            llr.Add(new Language_Record("sna", "Shona"));
            llr.Add(new Language_Record("snd", "Sindhi"));
            llr.Add(new Language_Record("snk", "Soninke"));
            llr.Add(new Language_Record("sog", "Sogdian"));
            llr.Add(new Language_Record("som", "Somali"));
            llr.Add(new Language_Record("sot", "Southern Sotho"));
            llr.Add(new Language_Record("spa", "Spanish"));
            llr.Add(new Language_Record("sqi", "Albanian"));
            llr.Add(new Language_Record("srd", "Sardinian"));
            llr.Add(new Language_Record("srn", "Sranan Tongo"));
            llr.Add(new Language_Record("srp", "Serbian"));
            llr.Add(new Language_Record("srr", "Serer"));
            llr.Add(new Language_Record("ssw", "Swati"));
            llr.Add(new Language_Record("suk", "Sukuma"));
            llr.Add(new Language_Record("sun", "Sundanese"));
            llr.Add(new Language_Record("sus", "Susu"));
            llr.Add(new Language_Record("sux", "Sumerian"));
            llr.Add(new Language_Record("swa", "Swahili(macrolanguage)"));
            llr.Add(new Language_Record("swe", "Swedish"));
            llr.Add(new Language_Record("syc", "Classical Syriac"));
            llr.Add(new Language_Record("syr", "Syriac"));
            llr.Add(new Language_Record("tah", "Tahitian"));
            llr.Add(new Language_Record("tam", "Tamil"));
            llr.Add(new Language_Record("tat", "Tatar"));
            llr.Add(new Language_Record("tel", "Telugu"));
            llr.Add(new Language_Record("tem", "Timne"));
            llr.Add(new Language_Record("ter", "Tereno"));
            llr.Add(new Language_Record("tet", "Tetum"));
            llr.Add(new Language_Record("tgk", "Tajik"));
            llr.Add(new Language_Record("tgl", "Tagalog"));
            llr.Add(new Language_Record("tha", "Thai"));
            llr.Add(new Language_Record("tig", "Tigre"));
            llr.Add(new Language_Record("tir", "Tigrinya"));
            llr.Add(new Language_Record("tiv", "Tiv"));
            llr.Add(new Language_Record("tkl", "Tokelau"));
            llr.Add(new Language_Record("tlh", "Klingon"));
            llr.Add(new Language_Record("tli", "Tlingit"));
            llr.Add(new Language_Record("tmh", "Tamashek"));
            llr.Add(new Language_Record("tog", "Tonga(Nyasa)"));
            llr.Add(new Language_Record("ton", "Tonga(Tonga Islands)"));
            llr.Add(new Language_Record("tpi", "Tok Pisin"));
            llr.Add(new Language_Record("tsi", "Tsimshian"));
            llr.Add(new Language_Record("tsn", "Tswana"));
            llr.Add(new Language_Record("tso", "Tsonga"));
            llr.Add(new Language_Record("tuk", "Turkmen"));
            llr.Add(new Language_Record("tum", "Tumbuka"));
            llr.Add(new Language_Record("tur", "Turkis"));
            llr.Add(new Language_Record("tvl", "Tuvalu"));
            llr.Add(new Language_Record("twi", "Twi"));
            llr.Add(new Language_Record("tyv", "Tuvinian"));
            llr.Add(new Language_Record("udm", "Udmurt"));
            llr.Add(new Language_Record("uga", "Ugaritic"));
            llr.Add(new Language_Record("uig", "Uighur"));
            llr.Add(new Language_Record("ukr", "Ukrainian"));
            llr.Add(new Language_Record("umb", "Umbundu"));
            llr.Add(new Language_Record("und", "Undetermined"));
            llr.Add(new Language_Record("urd", "Urdu"));
            llr.Add(new Language_Record("uzb", "Uzbek"));
            llr.Add(new Language_Record("vai", "Vai"));
            llr.Add(new Language_Record("ven", "Venda"));
            llr.Add(new Language_Record("vie", "Vietnamese"));
            llr.Add(new Language_Record("vol", "Volapük"));
            llr.Add(new Language_Record("vot", "Votic"));
            llr.Add(new Language_Record("wal", "Wolaytta"));
            llr.Add(new Language_Record("war", "Waray(Philippines)"));
            llr.Add(new Language_Record("was", "Washo"));
            llr.Add(new Language_Record("wln", "Walloon"));
            llr.Add(new Language_Record("wol", "Wolof"));
            llr.Add(new Language_Record("xal", "Kalmyk"));
            llr.Add(new Language_Record("xho", "Xhosa"));
            llr.Add(new Language_Record("yao", "Yao"));
            llr.Add(new Language_Record("yap", "Yapese"));
            llr.Add(new Language_Record("yid", "Yiddish"));
            llr.Add(new Language_Record("yor", "Yoruba"));
            llr.Add(new Language_Record("zap", "Zapotec"));
            llr.Add(new Language_Record("zbl", "Blissymbols"));
            llr.Add(new Language_Record("zen", "Zenaga"));
            llr.Add(new Language_Record("zgh", "Standard Moroccan Tamazight"));
            llr.Add(new Language_Record("zha", "Zhuang"));
            llr.Add(new Language_Record("zho", "Chinese"));
            llr.Add(new Language_Record("zul", "Zulu"));
            llr.Add(new Language_Record("zun", "Zuni"));
            llr.Add(new Language_Record("zxx", "No linguistic content"));
            llr.Add(new Language_Record("zza", "Zaza"));

            // If C:\Config does not exist, create it
            System.IO.Directory.CreateDirectory(@path);
            // serialize JSON to a string and then write string to a file, formatted
            File.WriteAllText(@path + @"Language_Codes.json", JsonConvert.SerializeObject(llr, Formatting.Indented));



        }

        public static Language_JSON_Record Read_Languages_Json(string path)
        {
            try
            {
                Language_JSON_Record ljr = JsonConvert.DeserializeObject<Language_JSON_Record>(File.ReadAllText(@path + @"Language_Codes.json"));
                return ljr;
            }

            catch
            {
                Language_JSON_Record ljr = null;
                return ljr;
            }

            
        }

    }

    public class Language_Record
    {
        /// <summary>
        ///  short accr
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Full name
        /// </summary>
        public string Ref_Name { get; set; }

        public Language_Record(string id, string ref_name)
        {
            Id = id;
            Ref_Name = ref_name;
        }

        public override string ToString()
        {
            return "ID: " + Id + " Ref_Name: " + Ref_Name;
        }
    }

    public class Location_Record
    {
        public string Loc { get; set; }
        public int Channel_Spacing { get; set; }
        public string Default_Language { get; set; }
        public int Start_Freq { get; set; }
        public int End_Frequency { get; set; }

        public Location_Record(string l, int cs, string dl, int sf, int ef)
        {
            Loc = l;
            Channel_Spacing = cs;
            Default_Language = dl;
            Start_Freq = sf;
            End_Frequency = ef;
        }

        public override string ToString()
        {
            return "Location: " + Loc + " Channel_Spacing: " + Channel_Spacing.ToString() + "Default_Language" + Default_Language + "Start_Freq" + Start_Freq.ToString() + "End_Frequency" + End_Frequency.ToString();
        }
    }

    public class Language_JSON_Record
    {
        public BindingList<Language_Record> Language = new BindingList<Language_Record>();
        public BindingList<Location_Record> Location = new BindingList<Location_Record>();

        public Language_JSON_Record()
        { }
    }
}

    


