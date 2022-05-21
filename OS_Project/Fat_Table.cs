using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Project
{
    public class Fat_Table
    {
        public static string FileName = "F:\\OS_Project\\fat.txt";
        public static int[] fat = new int[1024];
        public static void initialize_fat()
        {
            for (int i = 0; i < 5; i++)
                fat[i] = -1;
        }
        public static int[] get()
        {
            string path = FileName;
            FileStream Virtual_disk_text = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            Virtual_disk_text.Seek(1024, SeekOrigin.Begin);
            Byte[] bt = new Byte[1024 * 4];
            Virtual_disk_text.Read(bt, 0, bt.Length);
            Buffer.BlockCopy(bt, 0, fat, 0, fat.Length);
            Virtual_disk_text.Close();
            return fat;
        }
        public static void print()
        {
            int[] f = get();
            for (int i = 0; i < 1024; i++)
            {
                Console.WriteLine(i + " " + f[i]);
            }
        }
        public static void write()
        {
            string path = FileName;
            FileStream Virtual_disk_text = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            Virtual_disk_text.Seek(1024, SeekOrigin.Begin);
            Byte[] bt = new Byte[1024 * 4];
            Buffer.BlockCopy(fat, 0, bt, 0, bt.Length);
            Virtual_disk_text.Write(bt, 0, bt.Length);
            Virtual_disk_text.Close();
        }

        public static int available_block()
        {
            int[] f = get();
            int idx = 0;
            for (int i = 0; i < 1024; i++)
            {
                if (f[i] == 0)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }
        public static int get_next(int index)
        {
            int[] f = get();
            return f[index];
        }
        public static void set_next(int index, int value)
        {
            fat[index] = value;
            //Fat_Table.write();
        }
        public static int available_blocks()
        {
            int[] fa = get();

            int cnt = 0;
            for (int i = 5; i < 1024; i++)
            {
                if (fa[i] == 0)
                {
                    cnt++;
                }

            }
            return cnt;
        }
        public static int get_free_space()
        {
            return available_blocks() * 1024;
        }
    }
}
