using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace EWS_Config_Tool
{
    /// <summary>
    /// Turns into Output.json
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Output
    {
        [JsonProperty]
        public BindingList<Instruction> INSTRUCTIONS = new BindingList<Instruction>();
        [JsonProperty]
        public BindingList<Control> CONTROL = new BindingList<Control>();
        [JsonProperty]
        public string LANGUAGE = "";

        [JsonProperty]
        public string CFG_VERSION = "";

        public string LANGUAGE_FULL = "Undetermined";
        public Language_Record LangRecord = new Language_Record("UND", "Undetermined");

        public Output() { }

        public void Add_Instruction(Instruction ii)
        {
            INSTRUCTIONS.Add(ii);
        }

        public void Add_Control(Control cc)
        {
            CONTROL.Add(cc);
        }
        public void Add_Control_Instruction_only(string trigger, string instr)
        {
            // get the Control with the Trigger
            bool containsTriggerInstr = CONTROL.Any(item => item.TRIGGER == trigger);
            Control cc = CONTROL.Where(item => item.TRIGGER == trigger).First();
            if (cc != null)
            {
                // add the Instruction to it
                cc.INSTRUCTION_LIST.Add(instr);
            }
        }

        public void Remove_Control(Control cc)
        {
            CONTROL.Remove(cc);
        }
        public void Remove_Control_Instruction_only(string trigger, string instr)
        {
            // get the Control with the Trigger
            bool containsTriggerInstr = CONTROL.Any(item => item.TRIGGER == trigger);
            Control cc = CONTROL.Where(item => item.TRIGGER == trigger).First();
            if (cc != null)
            {
                // remove the Instruction to it
                cc.INSTRUCTION_LIST.Remove(instr);
            }
        }

        /// <summary>
        /// Prints to default Console
        /// </summary>
        public void Display_Instructions()
        {
            foreach (Instruction i in INSTRUCTIONS)
            {
                Console.WriteLine(i.ToString());
            }
        }
        
}

    public class Instruction: ICloneable
    {
        public string NAME { get; set; }
        public string AUDIO_FILE { get; set; }
        public string LANGUAGE { get; set; }        
        public string AUTHORITY { get; set; }

        public override string ToString()
        {
            return NAME + " " + LANGUAGE + " " + AUDIO_FILE + " " + AUTHORITY;
        }

        public object Clone()
        {
            return new Instruction { NAME = this.NAME, AUDIO_FILE = this.AUDIO_FILE, AUTHORITY = this.AUTHORITY, LANGUAGE = this.LANGUAGE };
        }
    }

    public class Control : ICloneable
    {
        public string TRIGGER { get; set; }
        /// <summary>
        /// Instruction list Name
        /// </summary>
        public BindingList<string> INSTRUCTION_LIST { get; set; }
        public int DURATION { get; set; }
        public int MESSAGE_BREAK_MINIMUM_SPEED { get; set; }
        public int MESSAGE_BREAK_DURATION { get; set; }
        public int C_MODE_HYSTERESIS { get; set; }


        public object Clone()
        {
            return new Control { DURATION = this.DURATION, C_MODE_HYSTERESIS = this.C_MODE_HYSTERESIS, MESSAGE_BREAK_MINIMUM_SPEED = this.MESSAGE_BREAK_MINIMUM_SPEED, MESSAGE_BREAK_DURATION = this.MESSAGE_BREAK_DURATION, INSTRUCTION_LIST = new BindingList<string>(this.INSTRUCTION_LIST.ToList()), TRIGGER = this.TRIGGER };
        }
    }

}
