using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace EWS_Config_Tool
{
    /// <summary>
    /// This is the holder of the output, channels, language etc
    /// There is only a single instance, we add and remove settings to the various classes
    /// via this.
    /// 
    /// I am not going to worry about the Singleton design, as we will only create one instance per GUI
    /// 
    /// </summary>
    class Main_Config
    {
        /// <summary>
        /// Output has
        /// List<Instructions>
        /// List<Controls>
        /// </summary>
        public Output EWS_Config_output;
        //public Channels EWS_Channels;
        public Channel EWS_Channels;

        public Language_Record Default_language;

        public BindingList<Language_Record> LanguagesLoaded;

        public BindingList<Location_Record> LocationsLoaded;

        /// <summary>
        /// only populate with instructions that are remaining after loading from control
        /// </summary>
        public BindingList<Instruction> INSTRUCTIONS_POOL = new BindingList<Instruction>();
        BindingList<Instruction> INSTRUCTIONS_POOL_TO_REMOVE = new BindingList<Instruction>();

        public BindingList<Instruction> Button1_Instructions = new BindingList<Instruction>();
        public BindingList<Instruction> Button2_Instructions = new BindingList<Instruction>();
        public BindingList<Instruction> Button3_Instructions = new BindingList<Instruction>();
        public BindingList<Instruction> Button4_Instructions = new BindingList<Instruction>();
        public BindingList<Instruction> Button5_Instructions = new BindingList<Instruction>();
        public BindingList<Instruction> Button6_Instructions = new BindingList<Instruction>();

        // used as temp variables for now // TODO clean up
        public int Character_count = 0;
        public string Wav_File_Name = "";
        // This get updated via arrows
        public BindingList<Frequency_Record> Frequencies_Pool = new BindingList<Frequency_Record>();
        //public SortableBindingList<Frequency_Record> Frequencies_Pool = new SortableBindingList<Frequency_Record>();
        public BindingList<Frequency_Record> Frequencies_Excluded = new BindingList<Frequency_Record>();
        public BindingList<Frequency_Record> Frequencies_Pool_to_Remove = new BindingList<Frequency_Record>();
        public BindingList<Frequency_Record> Frequencies_Included = new BindingList<Frequency_Record>();


        // list of all wav files when creating instructions
        public List<string> Audio_files = new List<string>();

        /// <summary>
        /// App startup + TempConfig
        /// </summary>
        public string TempPath;
        //       public string TempPathForCreatingZip;

        /// <summary>
        /// From select on UsbFolderBrowser
        /// </summary>
        public string UsbPath;
        internal string zippath;

        public string Wav_File_Name_path { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Main_Config(string startupPath)
        {
            EWS_Config_output = new Output();
            EWS_Channels = new Channel();

            EWS_Channels.CHANNELS.AUTOMATIC = new Automatic_Record { ACTIVE = "YES", AUTO_LEVEL = "YES", LEVEL = 100 };
            EWS_Channels.CHANNELS.ENHANCED = new Enhanced_Record { ACTIVE = "NO", REPEAT = 1 };

            // create the frequency list
            for (int f = 87500; f <= 108000; f = f + 100)
            {
                Frequencies_Pool.Add(new Frequency_Record { FREQUENCY = f, LANGUAGE = "", LEVEL = 100, POPULAR = "NO", AUTO_LEVEL = "YES", TA = 1 });
            }




            TempPath = startupPath;

            Console.WriteLine(Directory.Exists(TempPath));

        }

        /// <summary>
        /// TODO
        /// </summary>
        public void UpdateDefaultLanguage()
        {
            // get short name for default lang and set in file
            EWS_Config_output.LANGUAGE = Default_language.Id;
        }

        public Language_Record Get_From_ID(string id)
        {
            Language_Record lr = LanguagesLoaded.First(x => x.Id == id);
            if (lr == null)
            {
                MessageBox.Show("Language Record Not Found using Undetermined !", "Unknown Language Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lr = new Language_Record("UND", "Undetermined");
            }
            return lr;
        }

        public Language_Record Get_From_RefName(string ref_Name)
        {
            Language_Record lr = LanguagesLoaded.First(x => x.Ref_Name == ref_Name);
            if (lr == null)
            {
                MessageBox.Show("Language Record Not Found using Undetermined !", "Unknown Language Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lr = new Language_Record("UND", "Undetermined");
            }
            return lr;
        }

        /// <summary>
        /// Save all Added Instructions
        /// Save controls
        /// </summary>
        public void Save_output_config()
        {

            // convert all languages to short names
            Convert_to_Langauge_to_short_id_output_and_save_to_Json(EWS_Config_output);

        }

        private void Convert_to_Langauge_to_short_id_output_and_save_to_Json(Output EWS_Config_output_cpy)
        {

            Output EWS_Config_output_cpy_manual = new Output();
            //copy instructions
            foreach (Instruction ii in EWS_Config_output_cpy.INSTRUCTIONS)
            {
                Instruction iinew = (Instruction)ii.Clone();
                EWS_Config_output_cpy_manual.INSTRUCTIONS.Add(iinew);
            }
            //copy control
            foreach (Control cc in EWS_Config_output_cpy.CONTROL)
            {
                Control ccnew = (Control)cc.Clone();
                EWS_Config_output_cpy_manual.CONTROL.Add(ccnew);
            }
            EWS_Config_output_cpy_manual.LANGUAGE = EWS_Config_output_cpy.LANGUAGE;

            //iterate each language field and replace
            foreach (Instruction ii in EWS_Config_output_cpy_manual.INSTRUCTIONS)
            {
                Console.WriteLine("Lanuage: " + ii.LANGUAGE);
                //Console.WriteLine("Lanuage full: " + ii.);
                ii.LANGUAGE = Get_From_RefName(ii.LANGUAGE).Id;
            }

            EWS_Config_output_cpy_manual.CFG_VERSION = EWS_Config_output_cpy.CFG_VERSION;


            // check if dir avaiable, if not create it
            if (!Directory.Exists(@TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }
            File.WriteAllText(@TempPath + @"\output.json", JsonConvert.SerializeObject(EWS_Config_output_cpy_manual, Formatting.Indented));
        }

        private void Convert_to_Langauge_to_short_id_channels_and_save_to_json(Channel EWS_Channels_cpy)
        {
            Channel EWS_Channels_cpy_manual = new Channel();
            // manually copy everything into this object
            //EWS_Channels_cpy_manual
            //copy include
            foreach (Frequency_Record fi in EWS_Channels_cpy.CHANNELS.INCLUDE)
            {
                Frequency_Record finew = (Frequency_Record)fi.Clone();
                EWS_Channels_cpy_manual.CHANNELS.INCLUDE.Add(finew);
            }
            //copy exclude
            foreach (int xfi in EWS_Channels_cpy.CHANNELS.EXCLUDE)
            {
                int xnew = xfi;
                EWS_Channels_cpy_manual.CHANNELS.EXCLUDE.Add(xfi);
            }
            //copy automatic
            //            Automatic_Record ar = (Automatic_Record)EWS_Channels_cpy_manual.CHANNELS.AUTOMATIC.Clone();
            EWS_Channels_cpy_manual.CHANNELS.AUTOMATIC.ACTIVE = EWS_Channels_cpy.CHANNELS.AUTOMATIC.ACTIVE;
            EWS_Channels_cpy_manual.CHANNELS.AUTOMATIC.AUTO_LEVEL = EWS_Channels_cpy.CHANNELS.AUTOMATIC.AUTO_LEVEL;
            EWS_Channels_cpy_manual.CHANNELS.AUTOMATIC.LEVEL = EWS_Channels_cpy.CHANNELS.AUTOMATIC.LEVEL;

            //Copy Enhanced Stations
            EWS_Channels_cpy_manual.CHANNELS.ENHANCED.ACTIVE = EWS_Channels_cpy.CHANNELS.ENHANCED.ACTIVE;
            EWS_Channels_cpy_manual.CHANNELS.ENHANCED.REPEAT = EWS_Channels_cpy.CHANNELS.ENHANCED.REPEAT;

            //iterate each language field and replace
            foreach (Frequency_Record fi in EWS_Channels_cpy_manual.CHANNELS.INCLUDE)
            {
                fi.LANGUAGE = Get_From_RefName(fi.LANGUAGE).Id;
                if (fi.AUTO_LEVEL == "AUTO")
                {
                    fi.AUTO_LEVEL = "YES";
                }

                else
                {
                    fi.AUTO_LEVEL = "NO";
                }
            }

            EWS_Channels_cpy_manual.CFG_VERSION = EWS_Channels_cpy.CFG_VERSION;

            File.WriteAllText(@TempPath + @"\channels.json", JsonConvert.SerializeObject(EWS_Channels_cpy_manual, Formatting.Indented));
        }

        /// <summary>
        /// Loads all Instructions
        /// TODO test load contrl
        /// </summary>
        public void Load_output_config()
        {
            INSTRUCTIONS_POOL.Clear();
            INSTRUCTIONS_POOL_TO_REMOVE.Clear();
            Button1_Instructions.Clear();
            Button2_Instructions.Clear();
            Button3_Instructions.Clear();
            Button4_Instructions.Clear();
            Button5_Instructions.Clear();
            Button6_Instructions.Clear();

            string oppath = @TempPath + @"\output.json";
            if (File.Exists(oppath))
            {

                Output op = JsonConvert.DeserializeObject<Output>(File.ReadAllText(oppath));

                //get default langauage
                Console.WriteLine(op.LANGUAGE);
                //set default language
                Default_language = new Language_Record(op.LANGUAGE, Get_From_ID(op.LANGUAGE).Ref_Name);

                // convert all Languages to Long name from ID
                foreach (var a in op.INSTRUCTIONS)
                {
                    a.LANGUAGE = Get_From_ID(a.LANGUAGE).Ref_Name;
                }

                EWS_Config_output = op;

                foreach (Instruction i in EWS_Config_output.INSTRUCTIONS)
                {
                    Instruction ii = i;
                    INSTRUCTIONS_POOL.Add(ii);
                }

                /*
                foreach (Instruction x in INSTRUCTIONS_POOL)
                {
                    Console.WriteLine("Clone" + x.ToString());
                }*/

                // now get Triggers From Control then get Instructions attached to them
                foreach (Control c in EWS_Config_output.CONTROL)
                {
                    if (c.TRIGGER == "Button1")
                    {
                        //get the Control record for Button1
                        foreach (string s in c.INSTRUCTION_LIST)
                        {
                            // get the Instruction from the Main Instructions list and add it to the Button1 instructions so that 
                            // it can be bound to the datagrid view
                            foreach (Instruction ii in op.INSTRUCTIONS)
                            {
                                if (s == ii.NAME)
                                {
                                    Instruction bi = ii;
                                    Button1_Instructions.Add(bi);
                                    // also remove from the instruction pool to avoid duplicates
                                    INSTRUCTIONS_POOL_TO_REMOVE.Add(bi);
                                }
                            }

                        }
                    }
                    else if (c.TRIGGER == "Button2")
                    {
                        //get the Control record for Button2
                        foreach (string s in c.INSTRUCTION_LIST)
                        {
                            // get the Instruction from the Main Instructions list and add it to the Button1 instructions so that 
                            // it can be bound to the datagrid view
                            foreach (Instruction ii in op.INSTRUCTIONS)
                            {
                                if (s == ii.NAME)
                                {
                                    Instruction bi = ii;
                                    Button2_Instructions.Add(bi);
                                    // also remove from the instruction pool to avoid duplicates
                                    INSTRUCTIONS_POOL_TO_REMOVE.Add(bi);
                                }
                            }

                        }
                    }
                    else if (c.TRIGGER == "Button3")
                    {
                        //get the Control record for Button3
                        foreach (string s in c.INSTRUCTION_LIST)
                        {
                            // get the Instruction from the Main Instructions list and add it to the Button1 instructions so that 
                            // it can be bound to the datagrid view
                            foreach (Instruction ii in op.INSTRUCTIONS)
                            {
                                if (s == ii.NAME)
                                {
                                    Instruction bi = ii;
                                    Button3_Instructions.Add(bi);
                                    // also remove from the instruction pool to avoid duplicates
                                    INSTRUCTIONS_POOL_TO_REMOVE.Add(bi);
                                }
                            }

                        }
                    }
                    else if (c.TRIGGER == "Button4")
                    {
                        //get the Control record for Button4
                        foreach (string s in c.INSTRUCTION_LIST)
                        {
                            // get the Instruction from the Main Instructions list and add it to the Button1 instructions so that 
                            // it can be bound to the datagrid view
                            foreach (Instruction ii in op.INSTRUCTIONS)
                            {
                                if (s == ii.NAME)
                                {
                                    Instruction bi = ii;
                                    Button4_Instructions.Add(bi);
                                    // also remove from the instruction pool to avoid duplicates
                                    INSTRUCTIONS_POOL_TO_REMOVE.Add(bi);
                                }
                            }

                        }
                    }
                    else if (c.TRIGGER == "Button5")
                    {
                        //get the Control record for Button5
                        foreach (string s in c.INSTRUCTION_LIST)
                        {
                            // get the Instruction from the Main Instructions list and add it to the Button1 instructions so that 
                            // it can be bound to the datagrid view
                            foreach (Instruction ii in op.INSTRUCTIONS)
                            {
                                if (s == ii.NAME)
                                {
                                    Instruction bi = ii;
                                    Button5_Instructions.Add(bi);

                                    // also remove from the instruction pool to avoid duplicates
                                    INSTRUCTIONS_POOL_TO_REMOVE.Add(bi);
                                }
                            }

                        }
                    }
                    else if (c.TRIGGER == "Button6")
                    {
                        //get the Control record for Button6
                        foreach (string s in c.INSTRUCTION_LIST)
                        {
                            // get the Instruction from the Main Instructions list and add it to the Button1 instructions so that 
                            // it can be bound to the datagrid view
                            foreach (Instruction ii in op.INSTRUCTIONS)
                            {
                                if (s == ii.NAME)
                                {
                                    Instruction bi = ii;
                                    Button6_Instructions.Add(bi);
                                    // also remove from the instruction pool to avoid duplicates
                                    INSTRUCTIONS_POOL_TO_REMOVE.Add(bi);
                                }
                            }

                        }
                    }
                }

                // now remove any instructions that are already assigned
                foreach (Instruction instrremove in INSTRUCTIONS_POOL_TO_REMOVE)
                {
                    try
                    {
                        INSTRUCTIONS_POOL.Remove(instrremove);
                    }
                    catch (Exception)
                    { }
                }

            }
            else
            {
                // TODO send to status bar
                Console.WriteLine("No config found !");
            }

        }

        /// <summary>
        /// Save Include, Exclude, Automatic
        /// </summary>
        public void Save_channel_config()
        {
            Convert_to_Langauge_to_short_id_channels_and_save_to_json(EWS_Channels);
        }

        public void Load_channels_config()
        {
            INSTRUCTIONS_POOL_TO_REMOVE.Clear();

            if (File.Exists(@TempPath + @"\channels.json"))
            {

                Channel ch = JsonConvert.DeserializeObject<Channel>(File.ReadAllText(@TempPath + @"\channels.json"));

                // convert to full language name
                foreach (Frequency_Record fi in ch.CHANNELS.INCLUDE)
                {
                    fi.LANGUAGE = Get_From_ID(fi.LANGUAGE).Ref_Name;
                }

                EWS_Channels = ch;

                foreach (int ftoex in EWS_Channels.CHANNELS.EXCLUDE)
                {
                    Frequencies_Excluded.Add(new Frequency_Record { FREQUENCY = ftoex, LANGUAGE = "", LEVEL = 100 });
                    // find the record in the main pool with same freq in the ex and remove
                    foreach (Frequency_Record fi in Frequencies_Pool.ToList())
                    {
                        // update main frequency pool
                        if (fi.FREQUENCY == ftoex)
                        {
                            Console.WriteLine("Remove from main pool");
                            Frequencies_Pool.Remove(fi);
                        }
                    }
                }

                foreach (Frequency_Record ftoin in EWS_Channels.CHANNELS.INCLUDE)
                {
                    // add to include list for the dgv
                    if (ftoin.AUTO_LEVEL == "YES")
                    {
                        ftoin.AUTO_LEVEL = "AUTO";
                    }

                    else
                    {
                        ftoin.AUTO_LEVEL = "LEVEL";
                    }
                    Frequencies_Included.Add(ftoin);

                    // find the record in the main pool with same freq in the ex and remove
                    foreach (Frequency_Record fi in Frequencies_Pool.ToList())
                    {
                        // update main frequency pool
                        if (fi.FREQUENCY_MHz == ftoin.FREQUENCY_MHz)
                        {
                            Console.WriteLine("Remove from main pool");
                            Frequencies_Pool.Remove(fi);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles adding an instructions into a specific button
        /// manages the dgv updates
        /// manages the list updates
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="currentObject"></param>
        public void Add_Button_Instruction(string trigger, Instruction currentObject)
        {
            try
            {
                Console.WriteLine(currentObject);
                if (currentObject != null)
                {
                    if (INSTRUCTIONS_POOL.Contains(currentObject))
                    {
                        // get index of currentObject in pool
                        int index_to_remove = INSTRUCTIONS_POOL.IndexOf(currentObject);
                        if (index_to_remove >= 0)
                        {

                            //Console.WriteLine(index_to_remove);
                            INSTRUCTIONS_POOL.RemoveAt(index_to_remove);
                        }
                        //INSTRUCTIONS_POOL.Remove(currentObject);
                    }
                    // add to button 1 instruction
                    //dataGridView_btn_1_instr.
                    bool containsTriggerInstr = EWS_Config_output.CONTROL.Any(item => item.TRIGGER == trigger);
                    if (containsTriggerInstr)
                    {
                        // add the Instruction to it
                        EWS_Config_output.Add_Control_Instruction_only(trigger, currentObject.NAME);
                    }
                    else
                    {
                        BindingList<string> il = new BindingList<string>();
                        il.Add(currentObject.NAME);
                        EWS_Config_output.Add_Control(new Control { TRIGGER = trigger, DURATION = 0, INSTRUCTION_LIST = il, MESSAGE_BREAK_DURATION = 0, MESSAGE_BREAK_MINIMUM_SPEED = 0 });
                    }
                }

                //update the Button1-6 Instruction list
                foreach (Instruction ii in EWS_Config_output.INSTRUCTIONS)
                {
                    if (currentObject.NAME == ii.NAME)
                    {
                        Instruction bi = ii;
                        if (trigger == "Button1")
                        {
                            Button1_Instructions.Add(bi);
                        }
                        else if (trigger == "Button2")
                        {
                            Button2_Instructions.Add(bi);
                        }
                        else if (trigger == "Button3")
                        {
                            Button3_Instructions.Add(bi);
                        }
                        else if (trigger == "Button4")
                        {
                            Button4_Instructions.Add(bi);
                        }
                        if (trigger == "Button5")
                        {
                            Button5_Instructions.Add(bi);
                        }
                        else if (trigger == "Button6")
                        {
                            Button6_Instructions.Add(bi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Handles removing an instructions from a specific button back into the maion pool
        /// manages the dgv updates
        /// manages the list updates
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="currentObject"></param>
        public void Remove_Button_Instruction(string trigger, Instruction currentObject)
        {
            try
            {
                Console.WriteLine(currentObject);
                if (currentObject != null)
                {
                    INSTRUCTIONS_POOL.Add(currentObject);
                }

                // iterate each Control
                foreach (Control cc in EWS_Config_output.CONTROL.ToList())
                {
                    // Console.WriteLine(cc);
                    // match the instruction from the button to the name in trigger
                    if (cc.TRIGGER == trigger) // Button1 -6
                    {
                        if (cc.INSTRUCTION_LIST.Count >= 2)
                        {
                            EWS_Config_output.Remove_Control_Instruction_only(trigger, currentObject.NAME);
                            break;
                        }
                        else
                        {
                            EWS_Config_output.Remove_Control(cc);
                            break;
                        }
                    }
                }

                //update the Button1-6 Instruction list
                foreach (Instruction ii in EWS_Config_output.INSTRUCTIONS.ToList())
                {
                    if (currentObject.NAME == ii.NAME)
                    {
                        Instruction bi = ii;
                        if (trigger == "Button1")
                        {
                            if (Button1_Instructions.Contains(bi))
                            {
                                int index_to_remove = Button1_Instructions.IndexOf(bi);
                                if (index_to_remove >= 0)
                                {
                                    Console.WriteLine(index_to_remove);
                                    Button1_Instructions.RemoveAt(index_to_remove);
                                    //Button1_Instructions.Remove(bi);
                                }
                            }
                        }
                        else if (trigger == "Button2")
                        {
                            if (Button2_Instructions.Contains(bi))
                            {
                                Button2_Instructions.Remove(bi);
                            }
                        }
                        else if (trigger == "Button3")
                        {
                            if (Button3_Instructions.Contains(bi))
                            {
                                Button3_Instructions.Remove(bi);
                            }
                        }
                        else if (trigger == "Button4")
                        {
                            if (Button4_Instructions.Contains(bi))
                            {
                                Button4_Instructions.Remove(bi);
                            }
                        }
                        if (trigger == "Button5")
                        {
                            if (Button5_Instructions.Contains(bi))
                            {
                                Button5_Instructions.Remove(bi);
                            }
                        }
                        else if (trigger == "Button6")
                        {
                            if (Button6_Instructions.Contains(bi))
                            {
                                Button6_Instructions.Remove(bi);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Inlcude Freq
        /// <summary>
        /// if combobox drop down clicked
        /// </summary>
        public void Update_Include_Frequency(string freq, string lang, string auto, string lev, string pref, bool ta)
        {
            var item = EWS_Channels.CHANNELS.INCLUDE.First(x => x.FREQUENCY_MHz == freq);
            if (item != null)
            {
                item.LANGUAGE = lang;
                item.AUTO_LEVEL = auto;
                item.POPULAR = pref;
                item.LEVEL = int.Parse(lev);              
                item.TA = (ta == true ? 1 : 0);
            }
            Console.WriteLine(freq + " " + lang);
        }

        public void Add_Frequency_from_Frequency_Pool_to_Freq_Include_List(Frequency_Record currentObject)
        {
            // make sure the frequency is in the list and the object is not null
            if (currentObject != null)
            {

                Console.WriteLine("Remove Freq" + currentObject.ToString());
                // Add to the include list

                EWS_Channels.CHANNELS.INCLUDE.Add(new Frequency_Record { FREQUENCY = currentObject.FREQUENCY, LANGUAGE = Default_language.Ref_Name, LEVEL = 100, AUTO_LEVEL = "AUTO", POPULAR = "NO", TA = 1 });//  Always auto and TA set by default.  Change through advanced settings.   EWS_Channels.CHANNELS.AUTOMATIC.ACTIVE });

                currentObject.LANGUAGE = Default_language.Ref_Name;
                // add to include list for the dgv
                currentObject.AUTO_LEVEL = EWS_Channels.CHANNELS.AUTOMATIC.ACTIVE;

                //Add frequency record to included list
                Frequencies_Included.Add(currentObject);

                // remove from main pool
                Frequencies_Pool.Remove(currentObject);
            }
        }

        // TODO Test
        //public void Remove_Frequency_Freq_Include_List_to_Frequency_Pool(Frequency_Record currentObject)
        public void Remove_Frequency_Freq_Include_List_to_Frequency_Pool(string freq)
        {
            // make sure the frequency is in the list and the object is not null
            if (freq != "")
            {

                Console.WriteLine("Remove Freq from include" + freq);
                // Add to the include list
                //EWS_Channels.INCLUDE.Remove(currentObject);
                EWS_Channels.CHANNELS.INCLUDE.Remove(EWS_Channels.CHANNELS.INCLUDE.First(x => x.FREQUENCY_MHz == freq));

                // remove from include list for the dgv
                var obj = Frequencies_Included.First(x => x.FREQUENCY_MHz == freq);
                if (obj != null)
                {
                    Console.WriteLine(obj.ToString());
                    Frequencies_Included.Remove(Frequencies_Included.First(x => x.FREQUENCY_MHz == freq));

                    // add to main pool
                    Frequencies_Pool.Add(new Frequency_Record { FREQUENCY = obj.FREQUENCY, LANGUAGE = "", LEVEL = 100, AUTO_LEVEL = "NO", POPULAR = "NO", TA = 1 });
                }

            }
        }

        public bool CheckAllLanguagesUsedHaveInstructions(string callingFunction)
        {
            //            bool result = false;
            bool languageMatch = true;
            bool fDefaultLanguageInstructionAvailableForEachUsedButton = true;

            //Check if we have a default language instruction for each used button
            //Button1
            if (Button1_Instructions.Count > 0)
            {
                bool fButton1DefaultLanguageOK = false;
                foreach (Instruction btn1ins in Button1_Instructions)
                {
                    Console.WriteLine("Button 1 Language:  " + btn1ins.LANGUAGE);
                    if (btn1ins.LANGUAGE == Default_language.Ref_Name)
                    {
                        fButton1DefaultLanguageOK = true;
                        break;
                    }
                }

                if (!fButton1DefaultLanguageOK)
                {
                    MessageBox.Show("Button 1 does not have an instruction with the default language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fDefaultLanguageInstructionAvailableForEachUsedButton = false;
                }
            }

            //Button2
            if (Button2_Instructions.Count > 0)
            {
                bool fButton2DefaultLanguageOK = false;
                foreach (Instruction btn2ins in Button2_Instructions)
                {
                    Console.WriteLine("Button 2 Language:  " + btn2ins.LANGUAGE);
                    if (btn2ins.LANGUAGE == Default_language.Ref_Name)
                    {
                        fButton2DefaultLanguageOK = true;
                        break;
                    }
                }

                if (!fButton2DefaultLanguageOK)
                {
                    MessageBox.Show("Button 2 does not have an instruction with the default language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fDefaultLanguageInstructionAvailableForEachUsedButton = false;
                }
            }

            //Button3
            if (Button3_Instructions.Count > 0)
            {
                bool fButton3DefaultLanguageOK = false;
                foreach (Instruction btn3ins in Button3_Instructions)
                {
                    Console.WriteLine("Button 3 Language:  " + btn3ins.LANGUAGE);
                    if (btn3ins.LANGUAGE == Default_language.Ref_Name)
                    {
                        fButton3DefaultLanguageOK = true;
                        break;
                    }
                }

                if (!fButton3DefaultLanguageOK)
                {
                    MessageBox.Show("Button 3 does not have an instruction with the default language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fDefaultLanguageInstructionAvailableForEachUsedButton = false;
                }
            }

            //Button4
            if (Button4_Instructions.Count > 0)
            {
                bool fButton4DefaultLanguageOK = false;
                foreach (Instruction btn4ins in Button4_Instructions)
                {
                    Console.WriteLine("Button 4 Language:  " + btn4ins.LANGUAGE);
                    if (btn4ins.LANGUAGE == Default_language.Ref_Name)
                    {
                        fButton4DefaultLanguageOK = true;
                        break;
                    }
                }

                if (!fButton4DefaultLanguageOK)
                {
                    MessageBox.Show("Button 4 does not have an instruction with the default language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fDefaultLanguageInstructionAvailableForEachUsedButton = false;
                }
            }

            //Button5
            if (Button5_Instructions.Count > 0)
            {
                bool fButton5DefaultLanguageOK = false;
                foreach (Instruction btn5ins in Button5_Instructions)
                {
                    Console.WriteLine("Button 5 Language:  " + btn5ins.LANGUAGE);
                    if (btn5ins.LANGUAGE == Default_language.Ref_Name)
                    {
                        fButton5DefaultLanguageOK = true;
                        break;
                    }
                }

                if (!fButton5DefaultLanguageOK)
                {
                    MessageBox.Show("Button 5 does not have an instruction with the default language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fDefaultLanguageInstructionAvailableForEachUsedButton = false;
                }
            }

            //Button6
            if (Button6_Instructions.Count > 0)
            {
                bool fButton6DefaultLanguageOK = false;
                foreach (Instruction btn6ins in Button6_Instructions)
                {
                    Console.WriteLine("Button 6 Language:  " + btn6ins.LANGUAGE);
                    if (btn6ins.LANGUAGE == Default_language.Ref_Name)
                    {
                        fButton6DefaultLanguageOK = true;
                        break;
                    }
                }

                if (!fButton6DefaultLanguageOK)
                {
                    MessageBox.Show("Button 6 does not have an instruction with the default language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fDefaultLanguageInstructionAvailableForEachUsedButton = false;
                }
            }


            foreach (Frequency_Record fr in EWS_Channels.CHANNELS.INCLUDE)
            {
                if (1 == 1)//used to exclude test for undetermined language (fr.LANGUAGE != "Undetermined").  This is now included
                {
                    Console.WriteLine("Included channel language:  " + fr.LANGUAGE);

                    //Button1
                    if (Button1_Instructions.Count > 0)
                    {
                        bool btn1LanguageMatch = false;

                        foreach (Instruction btn1ins in Button1_Instructions)
                        {
                            Console.WriteLine("Button 1 Language:  " + btn1ins.LANGUAGE);
                            if (btn1ins.LANGUAGE == fr.LANGUAGE)
                            {
                                btn1LanguageMatch = true;
                                break;
                            }

                        }

                        if (!btn1LanguageMatch)
                        {
                            MessageBox.Show("Button 1 does not have an instruction with the " + fr.LANGUAGE + " language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            languageMatch = false;
                        }
                    }

                    //Button2
                    if (Button2_Instructions.Count > 0)
                    {
                        bool btn2LanguageMatch = false;

                        foreach (Instruction btn2ins in Button2_Instructions)
                        {
                            Console.WriteLine("Button 2 Language:  " + btn2ins.LANGUAGE);
                            if (btn2ins.LANGUAGE == fr.LANGUAGE)
                            {
                                btn2LanguageMatch = true;
                                break;
                            }

                        }

                        if (!btn2LanguageMatch)
                        {
                            MessageBox.Show("Button 2 does not have an instruction with the " + fr.LANGUAGE + " language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            languageMatch = false;
                        }
                    }

                    //Button3
                    if (Button3_Instructions.Count > 0)
                    {
                        bool btn3LanguageMatch = false;

                        foreach (Instruction btn3ins in Button3_Instructions)
                        {
                            Console.WriteLine("Button 3 Language:  " + btn3ins.LANGUAGE);
                            if (btn3ins.LANGUAGE == fr.LANGUAGE)
                            {
                                btn3LanguageMatch = true;
                                break;
                            }

                        }

                        if (!btn3LanguageMatch)
                        {
                            MessageBox.Show("Button 3 does not have an instruction with the " + fr.LANGUAGE + " language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            languageMatch = false;
                        }
                    }

                    //Button1
                    if (Button4_Instructions.Count > 0)
                    {
                        bool btn4LanguageMatch = false;

                        foreach (Instruction btn4ins in Button4_Instructions)
                        {
                            Console.WriteLine("Button 4 Language:  " + btn4ins.LANGUAGE);
                            if (btn4ins.LANGUAGE == fr.LANGUAGE)
                            {
                                btn4LanguageMatch = true;
                                break;
                            }

                        }

                        if (!btn4LanguageMatch)
                        {
                            MessageBox.Show("Button 4 does not have an instruction with the " + fr.LANGUAGE + " language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            languageMatch = false;
                        }
                    }

                    //Button5
                    if (Button5_Instructions.Count > 0)
                    {
                        bool btn5LanguageMatch = false;

                        foreach (Instruction btn5ins in Button5_Instructions)
                        {
                            Console.WriteLine("Button 5 Language:  " + btn5ins.LANGUAGE);
                            if (btn5ins.LANGUAGE == fr.LANGUAGE)
                            {
                                btn5LanguageMatch = true;
                                break;
                            }

                        }

                        if (!btn5LanguageMatch)
                        {
                            MessageBox.Show("Button 5 does not have an instruction with the " + fr.LANGUAGE + " language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            languageMatch = false;
                        }
                    }

                    //Button6
                    if (Button6_Instructions.Count > 0)
                    {
                        bool btn6LanguageMatch = false;

                        foreach (Instruction btn6ins in Button6_Instructions)
                        {
                            Console.WriteLine("Button 6 Language:  " + btn6ins.LANGUAGE);
                            if (btn6ins.LANGUAGE == fr.LANGUAGE)
                            {
                                btn6LanguageMatch = true;
                                break;
                            }

                        }

                        if (!btn6LanguageMatch)
                        {
                            MessageBox.Show("Button 6 does not have an instruction with the " + fr.LANGUAGE + " language", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            languageMatch = false;
                        }
                    }
                }

            }



            if (callingFunction == "saveAllToolStripMenuItem_Click")
            {
                if (languageMatch == false)
                {
                    MessageBox.Show("Your configuration is invalid because you don't have an instruction for all selected languages.  \n\nIf you would like to save the configuration for future editing, use the Backup menu", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (fDefaultLanguageInstructionAvailableForEachUsedButton == false)
                {
                    MessageBox.Show("Your configuration is invalid because you don't have an instruction that uses the default language for all used buttons.  \n\nIf you would like to save the configuration for future editing, use the Backup menu", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    languageMatch = false;
                }
            }

            else if (callingFunction == "bacKupToolStripMenuItem_Click")

            {
                if (languageMatch == false || fDefaultLanguageInstructionAvailableForEachUsedButton == false)
                {
                    DialogResult dialogResult = MessageBox.Show("Your configuration is invalid.  Would you like to continue?", "Configuration Error", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                    {
                        languageMatch = true;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        languageMatch = false;
                    }
                }
            }

            return languageMatch;
        }

        #endregion Include Freq

        #region Exclude Freq

        public void Remove_Frequency_from_Frequency_Pool_to_Freq_Exclude_List(Frequency_Record currentObject)
        {
            // make sure the frequency is in the list and the object is not null
            if (currentObject != null)
            {

                Console.WriteLine("Remove Freq" + currentObject.ToString());
                // add to exclude list for the json file
                EWS_Channels.CHANNELS.EXCLUDE.Add(currentObject.FREQUENCY);
                // add to exclude list for the dgv
                Frequencies_Excluded.Add(currentObject);
                // remove from main pool
                Frequencies_Pool.Remove(currentObject);
            }
        }

        internal void Remove_Frequency_Freq_Exclude_List_to_Frequency_Pool(Frequency_Record currentObjectfreq)
        {

            if (currentObjectfreq != null)
            {
                Console.WriteLine(currentObjectfreq.FREQUENCY);
                Console.WriteLine(currentObjectfreq.FREQUENCY_MHz);

                //remove from exclude list
                Frequencies_Excluded.Remove(currentObjectfreq);
                //remove from excluse list json
                EWS_Channels.CHANNELS.EXCLUDE.Remove(currentObjectfreq.FREQUENCY);

                // add to main freq pool
                Frequencies_Pool.Add(currentObjectfreq);

                // now sort
                // Bind the Stations pool
                List<Frequency_Record> sortedList = Frequencies_Pool.OrderBy(x => x.FREQUENCY).ToList();
                BindingList<Frequency_Record> sortedListB = new BindingList<Frequency_Record>(sortedList);
                Frequencies_Pool = sortedListB;
            }


        }
        #endregion Exclude freq
    }


}
