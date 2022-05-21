using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Project
{
    public class file_entry : Directory_Entry
    {
        public directory parent;
        public string content;
        public file_entry(string Name, byte Attr, int firstClust, int size, directory p, string con) : base(Name, Attr, firstClust, size)
        {
            parent = p;
            content = con;
        }

        public static byte[] ConvertContentToBytes(string Con)
        {
            byte[] contentBytes = new byte[Con.Length];
            for (int i = 0; i < Con.Length; i++)
            {
                contentBytes[i] = (byte)Con[i];
            }
            return contentBytes;
        }
        // this function will iterate in whole content and take each character and convert it into byte format.
        public static string ConvertBytesToContent(byte[] bytes)
        {
            string con = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                if ((char)bytes[i] != '\0')
                    con += (char)bytes[i];
                else
                    break;
            }
            return con;
        }

        public void writeFile()
        {
            byte[] byteContent = ConvertContentToBytes(content);
            List<byte[]> data = Virual_Disk.splitToBlocksOfBytes(byteContent);

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
            for (int i = 0; i < data.Count; i++)
            {
                Virual_Disk.write_block(data[i], fc);
                Fat_Table.set_next(fc, -1);
                Fat_Table.write();
                if (lc != -1)
                {
                    Fat_Table.set_next(lc, fc);
                }
                lc = fc;
                fc = Fat_Table.available_block();
            }
            Fat_Table.write();

        }

        public void readFile()
        {
            if (firstCluster != 0)
            {
                content = string.Empty;
                int cluster = firstCluster;
                int next = Fat_Table.get_next(cluster);
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virual_Disk.get_block(cluster));
                    cluster = next;
                    if (cluster != -1)
                        next = Fat_Table.get_next(cluster);
                }
                while (cluster != -1);

                content = ConvertBytesToContent(ls.ToArray());

            }
        }


        public void deleteFile()
        {
            if (firstCluster != 0)
            {
                int clusterIndex = firstCluster;

                int next = Fat_Table.get_next(clusterIndex);

                if (clusterIndex == 5 && next == 0)
                    return;

                do
                {
                    Fat_Table.set_next(clusterIndex, 0);
                    clusterIndex = next;
                    if (clusterIndex != -1)
                        next = Fat_Table.get_next(clusterIndex);

                } while (clusterIndex != -1);
            }
            if (this.parent != null)
            {
                string dirName = new string(filename);
                int index = this.parent.search_directory(dirName);

                if (index != -1)
                {
                    this.parent.Directory_Table.RemoveAt(index);
                    this.parent.write_directory();
                    Fat_Table.write();
                }
            }


        }

        public void clearFileSize()
        {


        }

        public void update_contebnt(file_entry f)
        {

        }

    }
}