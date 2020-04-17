using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class J_instruction:Form1
    {
        string label;
        int address;
        string retaddress;
        public J_instruction(string L)
        {
            address = 0;
            retaddress = "";
            label = L;
        }
        public string translate_instruction()
        {
            for (int i = 0; i < Labels.Count; i++)
            {
                if (label == Labels.ElementAt(i).Key)
                {
                    address = Labels.ElementAt(i).Value;
                    break;
                }
            }
            string B_J = Convert.ToString(jInstruction.ElementAt(0).Value, 2);
            while (B_J.Length != 6)
            {
                B_J = '0' + B_J;
            }
            //Address = Target Address / 4  
            address = address / 4;
            retaddress =Convert.ToString( address,2);
            while(retaddress.Length!=26)
            {
                retaddress = '0' + retaddress;
            }
            retaddress = B_J + retaddress;
            return retaddress;
        }
    }
}
