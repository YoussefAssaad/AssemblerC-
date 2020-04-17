using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class I_instructions:Form1
    {
        string opArrI;
        string rsArrI;
        string rtArrI;
        string Immediate;

        string B_opArrI;
        string B_rsArrI;
        string B_rtArrI;
        string B_Immediate;

        string bin_I_inst;

        int op_I, rs_I, rt_I, imm_I,offset;

        public I_instructions(string opCI, string rsCI, string rtCI, string immCI)
        {
            opArrI = opCI;
            rsArrI = rsCI;
            rtArrI = rtCI;
            Immediate = immCI;
        }

        public void fill()
        {
            //fill op
            op_I = int.Parse(opArrI);
            //fill rs
            foreach (var val in Registers)
            {
                if (val.Key == rsArrI)
                {
                    rs_I = val.Value;
                }
            }

            //fill rt
            foreach (var val in Registers)
            {
                if (val.Key == rtArrI)
                {
                    rt_I = val.Value;
                }
            }
            

        }
        public string translate_branch(int p)
        {

            //*****************************
            //fill op
            fill();
            //*****************************


            imm_I = Labels[Immediate];
            //target address=  imm_I -> offset=(target address-pc)/4
            offset = (imm_I - p) / 4;
            string ret_offset="";
            if (offset >= 0)
            {
                ret_offset = Convert.ToString(offset, 2);
                while (ret_offset.Length != 16)
                {
                    ret_offset = '0' + ret_offset;
                }
            }
            else
            {
                B_Immediate = Convert.ToString(offset, 2);
                for (int i = 0; i < 16; i++)
                {
                    ret_offset = B_Immediate[31 - i] + ret_offset;
                }
            }
            B_opArrI = Convert.ToString(op_I, 2);
            B_rsArrI = Convert.ToString(rs_I, 2);
            B_rtArrI = Convert.ToString(rt_I, 2);
            while (B_opArrI.Length != 6)
            {
                B_opArrI = '0' + B_opArrI;
            }
            while (B_rsArrI.Length != 5)
            {
                B_rsArrI = '0' + B_rsArrI;
            }
            while (B_rtArrI.Length != 5)
            {
                B_rtArrI = '0' + B_rtArrI;
            }
            ret_offset = B_opArrI + B_rsArrI + B_rtArrI + ret_offset;
            return ret_offset;
        }
        public string translate_I()
        {
            fill();
            B_opArrI = Convert.ToString(op_I, 2);
            B_rsArrI = Convert.ToString(rs_I, 2);
            B_rtArrI = Convert.ToString(rt_I, 2);
            imm_I = int.Parse(Immediate);
            B_Immediate = Convert.ToString(imm_I, 2);

            while (B_opArrI.Length != 6)
            {
                B_opArrI = '0' + B_opArrI;
            }
            while (B_rsArrI.Length != 5)
            {
                B_rsArrI = '0' + B_rsArrI;
            }
            while (B_rtArrI.Length != 5)
            {
                B_rtArrI = '0' + B_rtArrI;
            }
            while (B_Immediate.Length != 16)
            {
                B_Immediate = '0' + B_Immediate;
            }

            bin_I_inst = B_opArrI + B_rsArrI + B_rtArrI + B_Immediate;
            return bin_I_inst;
        }
    }
}