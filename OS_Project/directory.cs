using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Project
{
    public class directory : Directory_Entry
    {
        public List<Directory_Entry> Directory_Table = new List<Directory_Entry>();
        public directory parent;
        public directory()
        {

        }
        public directory(directory p, string Name, byte Attr, int firstClust, int siz) : base(Name, Attr, firstClust, siz)
        {
            parent = p;
        }

        public directory get_dirctory()
        {
            directory d = new directory();
            d.filename = filename;
            d.fileSize = fileSize;
            d.fileEmpty = fileEmpty;
            d.firstCluster = firstCluster;
            d.fileAttribute = fileAttribute;
            d.parent = parent;
            return d;
        }
        public void write_directory()
        {
            byte[] all_entries = new byte[32 * Directory_Table.Count];
            byte[] DEB = new byte[32];

            for (int i = 0; i < Directory_Table.Count; i++)
            {
                DEB = Directory_Table[i].convert_TO_BYTE();
                for (int j = i * 32; j < 32 * (i + 1); j++)
                {
                    all_entries[j] = DEB[j % 32];
                }
            }

            int num_of_blocks = (int)Math.Ceiling(all_entries.Length / 1024.0);
            int num_of_full_blocks = all_entries.Length / 1024;
            int remainder_blocks = all_entries.Length % 1024;

            List<byte[]> data = new List<byte[]>();
            for (int i = 0; i < num_of_blocks; i++)
            {
                byte[] temp = new byte[1024];
                if (i < num_of_full_blocks)
                {
                    for (int j = 0; j < 1024; j++)
                    {
                        temp[j] = all_entries[j + i * 1024];
                    }
                }
                else
                {
                    int indexR = (num_of_full_blocks * 1024);
                    for (int r = 0; r < remainder_blocks; r++)
                    {
                        temp[r] = all_entries[indexR];
                        indexR++;
                    }
                    temp[indexR] = (byte)'#';
                }

                data.Add(temp);
            }
            int fc = 0, lc = -1;
            if (firstCluster != 0)
            {
                fc = firstCluster;
            }
            else
            {
                fc = Fat_Table.available_block();
                firstCluster = fc;
            }
            for (int i = 0; i < num_of_blocks; i++)
            {
                Virual_Disk.write_block(data[i], fc);
                Fat_Table.set_next(fc, -1);
                if (lc != -1)
                {
                    Fat_Table.set_next(lc, fc);

                }
                lc = fc;
            }
            Fat_Table.write();

            fc = Fat_Table.available_block();
        }

        public void read_direcotry()
        {
            Directory_Table.Clear();
            byte[] arr2 = new byte[32];

            List<byte> ls = new List<byte>();
            byte[] d = new byte[32];
            int fc = 0, nc;

            if (firstCluster != 0)
            {
                fc = firstCluster;
            }
            nc = Fat_Table.get_next(fc);
            do
            {
                ls.AddRange(Virual_Disk.get_block(fc));
                if (fc != -1)
                {
                    nc = Fat_Table.get_next(fc);
                }
                fc = nc;
                break;
            }
            while (fc != -1);
            bool flag = false;
            for (int i = 0; i < ls.Count / 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (ls[j + i * 32] == '#')
                    {
                        flag = true;
                        break;
                    }
                    d[j] = ls[j + i * 32];
                }
                if (flag)
                    break;

                if (get_directory_entry(d).firstCluster != 0)
                    Directory_Table.Add(get_directory_entry(d));
            }


        }

        public int search_directory(string name)
        {
            read_direcotry();
            for (int i = 0; i < Directory_Table.Count; i++)
            {
                string s = new string(Directory_Table[i].filename), y = "";

                for (int j = 0; j < s.Length; j++)
                {

                    if (s[j] != 0)
                    {
                        y += s[j];
                    }
                }
                if (y == name)
                {
                    return i;
                }
            }

            return -1;
        }


        public void update_content(Directory_Entry d)
        {

            read_direcotry();
            int index = search_directory(d.filename.ToString());
            if (index != -1)
            {
                Directory_Table.RemoveAt(index);
                Directory_Table.Insert(index, d);
            }
        }

        public void delete_directory()
        {
            bool flag = false;
            int index, next;
            if (firstCluster != 0)
            {
                index = firstCluster;
                next = Fat_Table.get_next(index);
                if (filename != "root".ToCharArray())
                {
                    flag = true;
                    Console.WriteLine("can not delete root");
                    return;
                }
                do
                {
                    Fat_Table.set_next(index, 0);
                    index = next;
                    if (index != -1)
                    {
                        next = Fat_Table.get_next(index);
                    }
                }
                while (index != -1);
            }

            if (parent != null)
            {
                index = parent.search_directory(new string(filename));
                if (index != -1)
                {
                    parent.Directory_Table.RemoveAt(index);
                    parent.write_directory();
                }
                Fat_Table.write();
            }
            if (!flag)
                Console.WriteLine("directory deleted");
        }


    }
}
