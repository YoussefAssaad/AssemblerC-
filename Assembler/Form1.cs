using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Assembler
{
    public partial class Form1 : Form
    {
        //Dictionaries
        public Dictionary<string, int> rInstruction = new Dictionary<string, int>();
        public Dictionary<string, int> jInstruction = new Dictionary<string, int>();
        public Dictionary<string, int> instructions_op = new Dictionary<string, int>();
        public Dictionary<string, int> instructions_func = new Dictionary<string, int>();
        public Dictionary<string, int> Registers = new Dictionary<string, int>();
        static public Dictionary<string, int> Labels = new Dictionary<string, int>();
        public Dictionary<string, int> variables = new Dictionary<string, int>();

        public List<string> datalines = new List<string>();
        public List<string> textlines = new List<string>();

        public List<string> dataMemory = new List< string>();
        public List<string> codeMemory = new List<string>();
        public int pc = 4;

        public Form1()
        {
            InitializeComponent();
            button2.Hide();
            ///////////////////////////////////////////////
            ///Instructions op 
            rInstruction.Add("add", 0);
            rInstruction.Add("and", 0);
            rInstruction.Add("sub", 0);
            rInstruction.Add("nor", 0);
            rInstruction.Add("or",  0);
            rInstruction.Add("slt", 0);
            instructions_op.Add("addi", 8);
            instructions_op.Add("lw", 35);
            instructions_op.Add("sw", 43);
            instructions_op.Add("beq", 4);
            instructions_op.Add("bne", 5);
            jInstruction.Add("j", 2);

            //////////////////////////////////////////
            ///Instructions funct
            instructions_func.Add("add", 32);
            instructions_func.Add("and", 36);
            instructions_func.Add("sub", 34);
            instructions_func.Add("nor", 39);
            instructions_func.Add("or", 37);
            instructions_func.Add("slt", 42);

            /////////////////////////////////////////
            ///Registers
            Registers.Add("$t0", 8);
            Registers.Add("$t1", 9);
            Registers.Add("$t2", 10);
            Registers.Add("$t3", 11);
            Registers.Add("$t4", 12);
            Registers.Add("$t5", 13);
            Registers.Add("$t6", 14);
            Registers.Add("$t7", 15);
            Registers.Add("$t8", 24);
            Registers.Add("$t9", 25);
            Registers.Add("$s0", 16);
            Registers.Add("$s1", 17);
            Registers.Add("$s2", 18);
            Registers.Add("$s3", 19);
            Registers.Add("$s4", 20);
            Registers.Add("$s5", 21);
            Registers.Add("$s6", 22);
            Registers.Add("$s7", 23);
            Registers.Add("$zero", 0);
            Registers.Add("$at", 1);
            Registers.Add("$v0", 2);
            Registers.Add("$v1", 3);
            Registers.Add("$a0", 4);
            Registers.Add("$a1", 5);
            Registers.Add("$a2", 6);
            Registers.Add("$a3", 7);
            Registers.Add("$k0", 26);
            Registers.Add("$k1", 27);
            Registers.Add("$gp", 28);
            Registers.Add("$sp", 29);
            Registers.Add("$fp", 30);
            Registers.Add("$ra", 31);
            ///////////////////////////////////////


        }
        public void operation(int count,List<string> edit_text)
        {
            if (rInstruction.ContainsKey(edit_text[0+count]))
            {
                //R instruction
                string op = rInstruction[edit_text[0+count]].ToString();
                string rs = edit_text[2+count];
                string rt = edit_text[3+count];
                string rd = edit_text[1+count];
                string shamt = "0";
                string funct = instructions_func[edit_text[0+count]].ToString();
                R_instructions R = new R_instructions(op, rs, rt, rd, shamt, funct);
                string temp = R.translate_R();
                codeMemory.Add(temp);
            }
            else if (instructions_op.ContainsKey(edit_text[0+count]))
            {
                //i instruction
                if (edit_text[0+count] == "lw" || edit_text[0+count] == "sw")
                {
                    string op = instructions_op[edit_text[0+count]].ToString();
                    string rs = edit_text[3+count];
                    string rt = edit_text[1+count];
                    string imm;
                    if (variables.ContainsKey(edit_text[2 + count]))
                    {
                        imm = variables[edit_text[2 + count]].ToString();
                    }
                    else
                    {
                        imm = edit_text[2 + count];
                    }
                    I_instructions I = new I_instructions(op, rs, rt, imm);
                    string ret = I.translate_I();
                    codeMemory.Add(ret);
                }
                else if (edit_text[0+count] == "addi")
                {
                    string op = instructions_op[edit_text[0+count]].ToString();
                    string rs = edit_text[1 + count];
                    string rt = edit_text[2 + count];
                    string imm = edit_text[3+count];
                    I_instructions I = new I_instructions(op, rs, rt, imm);
                    string ret = I.translate_I();
                    codeMemory.Add(ret);
                }
                else if(edit_text[0+count]=="bne"||edit_text[0+count]=="beq")
                {
                    //branching 
                    string op = instructions_op[edit_text[0+count]].ToString();
                    string rs = edit_text[1 + count];
                    string rt = edit_text[2 + count];
                    string imm= edit_text[3 + count];
                    I_instructions i = new I_instructions(op,rs,rt,imm);
                    string ret = i.translate_branch(pc);
                    codeMemory.Add(ret);
                }

            }
            else if (jInstruction.ContainsKey(edit_text[0+count]))
            {
                //j instruction
                J_instruction j = new J_instruction(edit_text[1 + count]);
                string ret = j.translate_instruction();
                codeMemory.Add(ret);
            }
        }
        public void get_labels()
        {
            for (int i = 0; i < textlines.Count; i++)
            {
                if (textlines[i].Contains(":") && textlines[i][0] != '#')
                {
                    string[] txt_line = textlines[i].Split(':');
                    Labels.Add(txt_line[0], i * 4);

                }
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            button2.Show();
            int j, dataCounter = 0 ;
            string[] code_lines = richTextBox1.Text.Split('\n');
            //categorize each line to (data & text)
            for (int i = 0; i < code_lines.Length; i++)
            {
                //////////////////////// .Data ////////////////////////////////////
                if (code_lines[i].Length!=0&&code_lines[i][0]!='#'&&code_lines[i].Contains(".data"))
                {
                    j = i + 1;
                    while (!code_lines[j].Contains(".text"))
                    {

                        if (code_lines[j][0] != '#')
                        {
                            datalines.Add(code_lines[j]);
                        }
                        i++;
                        j++;
                    }
                }
                //////////////////////// .Text ////////////////////////////////////

                if (code_lines[i].Contains(".text"))
                {
                    j = i + 1;
                    i++;
                    while (i != code_lines.Length)
                    {
                        if (code_lines[i].Length!=0&&code_lines[i][0] != '#')
                        {
                            textlines.Add(code_lines[j]);
                        }
                        i++;
                        j++;

                    }

                }


            }
            ////////////////////////////////////////////////////
            ///////////////reading .data block/////////////////////////
            string data = "";
            for (int w = 0; w < datalines.Count; w++)
            {
                char[] split = { ' ', '.', ':', ',' };
                string[] line = datalines[w].Split(split);
                List<string> edit = new List<string>();
                for (int l = 0; l < line.Length; l++)
                {
                    if (line[l].Length != 0)
                    {

                        if (line[l][0] == '#')
                            break;
                       
                           else edit.Add(line[l]);
                        
                    }
                }
                if (edit[1] == "word")
                {
                    List<string> word_values = new List<string>();
                    for (int l = 2; l < edit.Count; l++)
                    {
                        word_values.Add(edit[l]);
                    }
                        variables.Add(edit[0], dataCounter*4);
                        dataCounter += word_values.Count;
                    /////////////converting the data to binary////////////////////
                    for (int l = 0; l < word_values.Count; l++)
                    {
                        data = Convert.ToString(int.Parse(word_values[l]), 2);
                        while (data.Length != 32)
                        {
                            data = '0' + data;

                        }
                        dataMemory.Add(data);
                    }
                }
                else if (edit[1] == "space")
                {
                    int size_of_arr = int.Parse(edit[2]);
                    for (int addmem = 0; addmem < size_of_arr; addmem++)
                    {
                        dataMemory.Add("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

                    }
                    variables.Add(edit[0], dataCounter * 4);
                    dataCounter += size_of_arr;
                }
            }

            //reading .text block
            List<string> edit_text = new List<string>();
            get_labels();
            for (int i = 0; i < textlines.Count; i++)
            {
                edit_text.Clear();
                char[] delimeters = { ' ', '(', ')', ':', ',' };
                string[] text_line = textlines[i].Split(delimeters);
                for (int l = 0; l < text_line.Length; l++)
                {
                    if (text_line[l].Length != 0)
                    {

                        if (text_line[l][0] == '#')
                            break;
                        else
                        {
                            edit_text.Add(text_line[l]);
                        }
                    }
                }

                if (rInstruction.ContainsKey(edit_text[0]) || instructions_op.ContainsKey(edit_text[0]) || jInstruction.ContainsKey(edit_text[0]))
                {
                    //without label
                    operation(0, edit_text);
                }
                else
                {
                    //with label
                    operation(1, edit_text);
                }
                pc += 4;
            }
            ////////////////////////// OUTPUT ///////////////////////////
            string text = "\n #Translation of Data Segment \n \n";
            for (int i = 0; i < dataMemory.Count; i++)
            {
                text += "Data memory (" + i.ToString() + ")     <=  "+ dataMemory[i]+"  ;";
                text += "\n";
            }
            text += "\n#Translation of Code Segment \n \n";
            for (int i = 0; i < codeMemory.Count; i++)
            {
                text += "Data memory (" + i.ToString() + ")     <=  " + codeMemory[i] + "  ;";
                text += "\n";
            }
            richTextBox1.Text = text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string file = richTextBox1.Text;
            string[] lines = file.Split('\n');
            string all_file = "";
            for(int i=0;i<lines.Length;i++)
            {
                all_file += lines[i] + '\n';
            }
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (Stream st = File.Open(saveFileDialog1.FileName, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(st))
                {
                    sw.Write(all_file);
                }
            }
            MessageBox.Show("Done :D");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            rInstruction.Clear();
            jInstruction.Clear();
            instructions_op.Clear();
            instructions_func.Clear();
            Registers.Clear();
            Labels.Clear();
            variables.Clear();
            datalines.Clear();
            textlines.Clear();
            dataMemory.Clear();
            codeMemory.Clear();
        }
        //****************************************


    }
}
