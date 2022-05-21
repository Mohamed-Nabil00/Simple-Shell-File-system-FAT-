using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//435
namespace OS_Project
{
    public class Commands
    {
        public static object Virual_Disk;

        public static string[] input(string str)
        {
            bool f = false; string y = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != ' ') { y += str[i]; f = false; }
                else { if (f == false) { y += str[i]; f = true; } }
            }
            string[] s = y.Split(' ');
            return s;
        }

        public static string getpath()
        {
            return Program.curpath;
        }

        public static void clear()
        {
            Console.Clear();
        }

        public static void exit()
        {

            Environment.Exit(0);
        }


        public static void md(string name)
        {
            int position = Program.current.search_directory(name);
            Program.current.read_direcotry();
            if (position == -1)
            {
                Directory_Entry d = new Directory_Entry(name, 0X10, 0, 0);
                Program.current.Directory_Table.Add(d);
                Program.current.write_directory();
                Fat_Table.set_next(d.firstCluster, -1);

                Fat_Table.write();
                //Fat_Table.print();
                if (Program.current.parent != null)
                {
                    Program.current.parent.update_content(Program.current.get_dirctory());
                }
            }
            else
            {
                Console.WriteLine("directory already exists");
            }
        }

        public static void rd(string name)
        {
            int index = Program.current.search_directory(name);
            if (index != -1)
            {

                if (Program.current.Directory_Table[index].fileAttribute == 0X10)
                {

                    Console.WriteLine("are you sure that you want complete " + name + ", please enter Y for yes or N for no:");
                    string s = Console.ReadLine().ToUpper();
                    if (s == "Y")
                    {
                        int fc = Program.current.Directory_Table[index].firstCluster;
                        directory d = new directory(Program.current, name, 0X10, fc, 0);
                        d.write_directory();
                        d.delete_directory();

                    }
                }
                else
                {
                    Console.WriteLine("cannot delete directory");
                }
            }
            else
            {
                Console.WriteLine("Not Exist");
            }
        }

        public static void del(string fileName)
        {
            string[] path = fileName.Split("\\");
            //if (path.Length > 1)
            //{
            //    for (int i = 1; i < path.Length - 1; i++)
            //        moveToDirUsedInAnother(path[i]);

            //    fileName = path[path.Length - 1];
            //}
            fileName = path[path.Length - 1];
            int j = Program.current.search_directory(fileName);
            if (j != -1)
            {

                if (Program.current.Directory_Table[j].fileAttribute == 0X0)
                {
                    int fc = Program.current.Directory_Table[j].firstCluster;
                    int sz = Program.current.Directory_Table[j].fileSize;

                    file_entry file = new file_entry(fileName, 0x0, fc, sz, Program.current, null);
                    file.deleteFile();

                }
                else
                {
                    Console.WriteLine("The System Cannot Find The file specified");
                }
            }
            else
            {
                Console.WriteLine("The System Cannot Find The file specified");
            }

            //Directory rootDirectory = new Directory("M:", 0x10, 5, null);
            //Program.current = rootDirectory;
            //Program.current.ReadDirectory();
        }

        public static void cd(string name)
        {
            int index = Program.current.search_directory(name);
            if (index != -1)
            {
                if (Program.current.Directory_Table[index].fileAttribute == 0X10)
                {
                    int fc = Program.current.Directory_Table[index].firstCluster;
                    directory d = new directory(Program.current, name, 0X10, fc, 0);
                    d.write_directory();
                    Program.current = d;
                    Program.curpath += (new string(Program.current.filename) + @"\");
                }
                else
                {
                    Console.WriteLine("Can't change current directory ");
                }
            }
            else
            {
                if (Program.current.parent != null)
                {
                    string n = new string(Program.current.parent.filename);
                    if (n == name)
                    {
                        directory d = Program.current.parent;
                        d.read_direcotry();
                        Program.current = d;
                        Program.curpath = Program.curpath.Remove(Program.curpath.Length - 1);
                        while (Program.curpath[Program.curpath.Length - 1] != '\\')
                        {
                            Program.curpath = Program.curpath.Remove(Program.curpath.Length - 1);
                        }

                    }
                }
                else
                    Console.WriteLine("Not Exist");
            }
        }

        public static void dir()
        {
            Program.current.read_direcotry();
            int numf = 0, numd = 0, size_file = 0;
            int n = Program.current.Directory_Table.Count();
            for (int i = 0; i < n; i++)
            {

                if (Program.current.Directory_Table[i].fileAttribute == 0X0)
                {
                    string na = new string(Program.current.Directory_Table[i].filename);
                    Console.WriteLine("         " + Program.current.Directory_Table[i].fileSize + "  " + na);
                    numf++;
                    size_file += Program.current.Directory_Table[i].fileSize;
                }
                else
                {
                    string na = new string(Program.current.Directory_Table[i].filename);
                    Console.WriteLine("<DIR>        " + na);
                    numd++;
                }
            }
            Console.WriteLine(numf + " File(s)         " + size_file + " Bytes");
            Console.WriteLine(numd + " Dir(s)         " + (Fat_Table.get_free_space() - size_file) + " Bytes");
        }

        public static void import(string dest)
        {
            if (File.Exists(dest))
            {
                string content = File.ReadAllText(dest);
                int size = content.Length;
                string[] names = dest.Split("\\");
                string name = new string(names[names.Length - 1]);
                int j = Program.current.search_directory(name);
                if (j == -1)
                {
                    int fc;
                    if (size > 0)
                    {
                        fc = Fat_Table.available_block();
                    }
                    else
                    {
                        fc = 0;
                    }
                    file_entry newFile = new file_entry(name, 0X0, fc, size, Program.current, content);

                    newFile.writeFile();
                    //Fat_Table.print();
                    Directory_Entry d = new Directory_Entry(new string(name), 0X0, fc, size);
                    Program.current.Directory_Table.Add(d);
                    Program.current.write_directory();
                }
                else
                {
                    Console.WriteLine($"{name} is already exist in your virtual disk");
                }

            }
            else
            {
                Console.WriteLine("The file you specified does not exist in your compuret");
            }
        }

        public static void type(string name)
        {
            string[] path = name.Split("\\");
            //if (path.Length > 1)
            //{
            //    for (int i = 1; i < path.Length - 1; i++)
            //        moveToDirUsedInAnother(path[i]);

            //    name = path[path.Length - 1];
            //}
            string filename = new string(path[path.Length - 1]);

            int j = Program.current.search_directory(filename);
            if (j != -1)
            {
                int fc = Program.current.Directory_Table[j].firstCluster;
                int sz = Program.current.Directory_Table[j].fileSize;
                string content = null;
                file_entry file = new file_entry(filename, 0x0, fc, sz, Program.current, content);
                file.readFile();
                Console.WriteLine(file.content);
            }
            else
            {
                Console.WriteLine("The System could not found the file specified");
            }
            //Directory rootDirectory = new Directory("M:", 0x10, 5, null);
            //Program.current = rootDirectory;
            //Program.current.ReadDirectory();
        }

        public static void export(string source, string dest)
        {
            //string[] path = source.Split("\\");
            //if (path.Length > 1)
            //{
            //    for (int i = 1; i < path.Length - 1; i++)
            //        moveToDirUsedInAnother(path[i]);

            //    source = path[path.Length - 1];
            //}
            int j = Program.current.search_directory(source);
            if (j != -1)
            {
                if (System.IO.Directory.Exists(dest))
                {
                    int fc = Program.current.Directory_Table[j].firstCluster;
                    int sz = Program.current.Directory_Table[j].fileSize;
                    string content = null;
                    file_entry file = new file_entry(source, 0x0, fc, sz, Program.current, content);
                    file.readFile();
                    StreamWriter sw = new StreamWriter(dest + "\\" + source);
                    sw.Write(file.content);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    Console.WriteLine("The system cannot find the path specified in hte coputer dis");
                }

            }
            else
            {
                Console.WriteLine("The system cannot find the file you want to export in the virtual disk");
            }
            //Directory rootDirectory = new Directory("M:", 0x10, 5, null);
            //Program.current = rootDirectory;
            //Program.current.ReadDirectory();
        }

        public static void rename(string oldName, string newName)
        {
            //string[] path = oldName.Split("\\"); //old name could be path
            //if (path.Length > 1)
            //{
            //    for (int i = 1; i < path.Length - 1; i++)
            //        moveToDirUsedInAnother(path[i]);

            //    oldName = path[path.Length - 1];
            //}

            int j = Program.current.search_directory(oldName);
            if (j != -1)
            {
                if (Program.current.search_directory(newName) == -1)
                {
                    Directory_Entry d = Program.current.Directory_Table[j];

                    if (newName.Length > 11 && d.fileAttribute == 0X0)
                    {
                        newName = newName.Substring(0, 7) + newName.Substring(newName.Length - 4);
                    }
                    else if (newName.Length > 11 && d.fileAttribute == 0X10)
                        newName = newName.Substring(0, 11);
                    char[] newN = newName.ToCharArray();
                    d.filename = newN;
                    Program.current.write_directory();
                }
                else
                {
                    Console.WriteLine("Doublicate File Name exist or file cannot be found");
                }
            }
            else
            {
                Console.WriteLine("The System Cannot Find the File specified");
            }

            //Directory rootDirectory = new Directory("M:", 0x10, 5, null);
            //Program.current = rootDirectory;
            //Program.current.ReadDirectory();
        }
        public static void MoveToDestination(string p)
        {
            directory d = null;
            string[] arr = p.Split('\\');
            string path = "root";
            directory root = new directory(null, "root", 0X10, 5, 0);
            Program.current = root;
            for (int i = 1; i < arr.Length; i++) //ss -> mohamed sayed
            {
                int j = root.search_directory(arr[i]); // serach for the next folder in the path
                if (j != -1) // if found
                {
                    directory tempOfParent = root;
                    string newName = new string(root.Directory_Table[j].filename);// we get the name of the directory se seek to move to it
                    byte attr = root.Directory_Table[j].fileAttribute;//also we get its arrtributes
                    int fc = root.Directory_Table[j].firstCluster;
                    int sz = root.Directory_Table[j].fileSize;
                    root = new directory(tempOfParent, newName, attr, fc, sz);
                    root.read_direcotry();
                    path += ("\\" + new string(newName));
                }
                else//not found
                {
                    root = null;
                    break;
                }
            }
            if (root != null)
            {
                root.read_direcotry();
                Program.current = root;
            }
            else
            {
                root = new directory(null, "root", 0X10, 5, 0);
                Program.current = root;
                Console.WriteLine("the system cannot find the specified folder.!");
            }
        }




        public static void copy(string source, string dest)
        {
            string[] path = source.Split("\\");
            //if (path.Length > 1)
            //{
            //    for (int i = 1; i < path.Length - 1; i++)
            //        moveToDirUsedInAnother(path[i]);

            //    name = path[path.Length - 1];
            //}
            source = new string(path[path.Length - 1]);
            int j = Program.current.search_directory(source);
            int fc;
            int sz;

            if (source == dest)
            {
                Console.WriteLine("the file cannot be copied onto itself");
                return;
            }


            if (j != -1)
            {
                fc = Program.current.Directory_Table[j].firstCluster;
                sz = Program.current.Directory_Table[j].fileSize;
                //string[] myWay = dest.Split("\\");

                MoveToDestination(dest);

                int x = Program.current.search_directory(source);//ده المكان اللي اتحركتله
                if (x != -1)
                {
                    Console.Write("The File is aleary existed, Do you want to overwrite it ?, please enter Y for yes or N for no:");
                    string choice = Console.ReadLine().ToLower();
                    if (choice.Equals("Y"))
                    {//هل هنا همسح القديم
                        Directory_Entry d = new Directory_Entry(new string(source), 0X0, fc, sz);
                        Program.current.Directory_Table.Add(d);
                        Program.current.write_directory();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    Directory_Entry d = new Directory_Entry(new string(source), 0X0, fc, sz);
                    Program.current.Directory_Table.Add(d);
                    Program.current.write_directory();

                }

                //Directory rootDirectory = new Directory("M:", 0x10, 5, null);
                //Program.current = rootDirectory;
                //Program.current.ReadDirectory();

            }
            else
            {
                Console.WriteLine($"The File ${source} Is Not Existed In your disk");
            }
        }


        public static void help(string arg = "")
        {
            if (arg == "")
            {
                Console.WriteLine();
                Console.WriteLine("cd            Displays the name of or changes the current directory.\n");
                Console.WriteLine("cls           Clears the screen.\n");
                Console.WriteLine("dir           Displays a list of files and subdirectories in a directory.\n");
                Console.WriteLine("quit          Quits the CMD.EXE program (command interpreter) or the current batch script.\n");
                Console.WriteLine("copy          Copies one or more files to another location.\n");
                Console.WriteLine("del           Deletes one or more files.\n");
                Console.WriteLine("help          Provides help information for Windows commands.\n");
                Console.WriteLine("md            Creates a directory.\n");
                Console.WriteLine("rd            Removes a directory.\n");
                Console.WriteLine("rename        Renames a file or files.\n");
                Console.WriteLine("type          Displays the contents of a text file.\n");
                Console.WriteLine("import        import text file(s) from your computer.\n");
                Console.WriteLine("export        export text file(s) to your computer.\n");
            }
            else
            {
                if (arg == "cls")
                {
                    Console.WriteLine("\ncls         Clears the screen.\n");
                }
                else if (arg == "quit")
                {
                    Console.WriteLine("\nquit        Quits the CMD.EXE program (command interpreter) or the current batch script.\n");
                }
                else if (arg == "help")
                {
                    Console.WriteLine("\nhelp        Provides help information for Windows commands.\nHELP [command]\n" +
                        " command - displays help information on that command.\n");
                }
                else if (arg == "dir")
                {
                    Console.WriteLine("dir           Displays a list of files and subdirectories in a directory.\n");
                }
                else if (arg == "copy")
                {
                    Console.WriteLine("copy          Copies one or more files to another location.\n");
                }
                else if (arg == "del")
                {
                    Console.WriteLine("del           Deletes one or more files.\n");
                }
                else if (arg == "help")
                {
                    Console.WriteLine("dir           Displays a list of files and subdirectories in a directory.\n");
                }
                else if (arg == "md")
                {
                    Console.WriteLine("md            Creates a directory.\n");
                }
                else if (arg == "rd")
                {
                    Console.WriteLine("rd            Removes a directory.\n");
                }
                else if (arg == "rename")
                {
                    Console.WriteLine("rename        Renames a file or files.\n");
                }
                else if (arg == "type")
                {
                    Console.WriteLine("type          Displays the contents of a text file.\n");
                }
                else if (arg == "import")
                {
                    Console.WriteLine("import        import text file(s) from your computer.\n");
                }
                else if (arg == "export")
                {
                    Console.WriteLine("export        export text file(s) to your computer.\n");
                }

                else
                {
                    Console.WriteLine($"\nThis command is not supported by the help utility.  Try { arg} /? .\n");
                }
            }
        }

        public static void command(string[] str, string orgi)
        {
            if (str[0] == "quit")
            {
                exit();
            }
            else if (str[0] == "help")
            {
                if (str.Length > 1)
                    help(str[1]);
                else
                    help();
            }
            else if (str[0] == "cls")
            {
                clear();
            }
            else if (str[0] == "cd")
            {
                if (str.Length == 1)
                {

                    Console.WriteLine(Program.curpath + ":");
                }
                else if (str.Length == 2)
                {
                    cd(str[1]);
                }
                else
                {
                    Console.WriteLine("enter correct path");
                }
            }
            else if (str[0] == "md")
            {
                if (str.Length == 2)
                    md(str[1]);
                else
                    Console.WriteLine("enter correct name");
            }
            else if (str[0] == "dir")
            {
                dir();
            }
            else if (str[0] == "rd")
            {
                if (str.Length > 1)
                    rd(str[1]);
                else
                    Console.WriteLine("enter correct name");
            }
            else if (str[0] == "import")
            {
                if (str.Length == 2)
                    import(str[1]);
                else
                    Console.WriteLine("ERROR\n, you shold specify File name to import\n import [dest]filename");
            }
            else if (str[0] == "export")
            {
                if (str.Length == 3)
                    export(str[1], str[2]);
                else
                    Console.WriteLine("ERROR\n, The Correct syntax is \n import   [Source File] [destination]\n");
            }

            else if (str[0] == "type")
            {
                if (str.Length == 2)
                    type(str[1]);
                else
                    Console.WriteLine("ERROR\n, you shold specify file name to show its contnet\n type [dest]filename");

            }

            else if (str[0] == "rename")
            {
                if (str.Length == 3)
                    rename(str[1], str[2]);
                else
                    Console.WriteLine("ERROR\n, The Correct syntax is \n rename[old name][new name]\n");
            }

            else if (str[0] == "del")
            {
                if (str.Length == 2)
                    del(str[1]);
                else
                    Console.WriteLine("ERROR\n, The Correct syntax is \n del[file name]\n");
            }
            else if (str[0] == "copy")
            {
                if (str.Length == 3)
                    copy(str[1], str[2]);
                else
                    Console.WriteLine("ERROR\n, The Correct syntax is \n copy[Source File][destination]\n");
            }

            else if (str[0] == "")
            {
                return;
            }
            else
            {
                Console.WriteLine($"\'{orgi}\' is not recognized as an internal or external command, \noperable program or batch file. \n");

            }
        }
    }
}
