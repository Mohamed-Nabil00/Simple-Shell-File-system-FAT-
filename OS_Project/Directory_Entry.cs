using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Project
{
    public class Directory_Entry
    {
        public char[] filename = new char[11]; // 11 byte
        public byte fileAttribute; //1 byte
        public byte[] fileEmpty = new byte[12]; // zeros 12 byte
        public int fileSize; // 4 byte ---- 0 for folder ---- or the size of the file --- search how to know the size of the file 
        public int firstCluster; // 4 byte

        public Directory_Entry() { }
        public Directory_Entry(string Name, byte Attr, int fc, int size)
        {
            fileAttribute = Attr;
            if (fc == 0)
            {
                fc = Fat_Table.available_block();
                firstCluster = fc;
            }
            else
            {
                firstCluster = fc;
            }

            if (fileAttribute == 0)
            {
                if (Name.Length > 11)
                {
                    Name = Name.Substring(0, 7) + Name.Substring(Name.Length - 4);
                }
            }
            else
            {
                Name = Name.Substring(0, Math.Min(11, Name.Length));
            }
            filename = Name.ToCharArray();
            fileSize = size;
        }

        public byte[] convert_TO_BYTE()
        {

            byte[] data = new byte[32];
            for (int i = 0; i < filename.Length; i++)
            {
                data[i] = Convert.ToByte(filename[i]);
            }
            data[11] = fileAttribute;
            for (int i = 12; i < 24; i++)
            {
                data[i] = fileEmpty[i - 12];
            }
            Byte[] s = new Byte[4];
            s = BitConverter.GetBytes(firstCluster);
            for (int i = 24; i < s.Length + 24; i++)
            {
                data[i] = s[i - 24];
            }
            s = BitConverter.GetBytes(fileSize);
            for (int i = 28; i < s.Length + 28; i++)
            {
                data[i] = (byte)s[i - 28];
            }
            return data;
        }
        public Directory_Entry get_directory_entry(byte[] data)
        {
            byte[] e = new byte[4];
            Directory_Entry dir = new Directory_Entry();
            int i;
            for (i = 0; i < 11; i++)
            {
                dir.filename[i] = Convert.ToChar(data[i]);
            }
            dir.fileAttribute = data[i];
            for (i = 12; i < 24; i++)
            {
                dir.fileEmpty[i - 12] = data[i];
            }
            for (i = 24; i < 28; i++)
            {
                e[i - 24] = data[i];
            }
            dir.firstCluster = BitConverter.ToInt32(e, 0);
            for (i = 28; i < 32; i++)
            {
                e[i - 28] = data[i];
            }
            dir.fileSize = BitConverter.ToInt32(e, 0);
            return dir;
        }
    }
}
