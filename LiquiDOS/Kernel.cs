using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.FileSystem.VFS;
using LiquiDOS.Drivers;
using System.IO;

namespace LiquiDOS
{
    public class Kernel : Sys.Kernel
    {
        #region Variables

        Sys.FileSystem.CosmosVFS fs;
        //Declare all the drivers (load 'em like linux)
        bool isConsole = true; bool firstTImeUser = true;
        MouseDriver m; VBE v;
        int usergroup = 1; bool isLoggedIn = false; string user = "", ps = ""; //IDK if storing pswd in ram is ok?

        public static string cd = @"0:\";
        //private List<string> commandHistory = new List<string>();
        static string s1 = "                  LiquiDOS created by Kevin Dai, using COSMOS";
        static string s2 = "          Currently under development. Not responsible for any damages";
        static string s3 = "                            Proceed at your own risk";
        static string s4 = "            Welcome to LquiDOS.Type help [pg #] for a list of commands";

        #endregion

        #region Basics

        protected override void BeforeRun()
        {
            //Init the filesystem
            fs = new Sys.FileSystem.CosmosVFS();
            VFSManager.RegisterVFS(fs);
            fs.Initialize();
            //Power.enableACPI();
            //Display welcome message
            Cosmos.System.Kernel.PrintDebug("Kernel loaded sucessfully!");
            Console.WriteLine("Loading complete...");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(s1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s2);
            Console.WriteLine(s3);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s4);
            Console.ForegroundColor = ConsoleColor.White;
            firstTImeUser = firstTimeUser();
        }

        protected override void Run()
        {
            if (!firstTImeUser) //If we aren't a first time user
            {
                if (isLoggedIn)
                {
                    if (isConsole)
                        consoleLoop();
                    else
                        initGUI();
                }
                else
                    displayLogin();
            }
            else //Else init the install procedure
            {
                Console.Clear();
                Console.WriteLine("LiquiDOS is getting your OS set up, hang on!");
                try
                {
                    fs.CreateFile("0:\\users.dat");
                    File.WriteAllText("0:\\users.dat", "$user:root$pswd:root$date:#NAL#$group:01$name:root");
                    Console.WriteLine(File.ReadAllText("0:\\users.dat"));
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
                Console.WriteLine("Created users.dat! Press any key to reboot...");
                Console.ReadKey();
                reboot("");
            }
        }

        private bool firstTimeUser()
        {
            return File.Exists("0:\\users.dat"); //Files inside directory not supported
        }

        #endregion

        #region Console

        private void consoleLoop()
        {
            //Init the console. Get the current dir (cd)
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(cd); Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> "); Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine();
            //commandHistory.Add(input);
            try
            {
                switch (input.Trim().Split(' ')[0])
                {
                    case "help": help(input); break;
                    case "ls": dir(); break;
                    case "fs": listDrives(); break;
                    case "clear": clear(); break;
                    case "reboot": reboot(input); break;
                    case "shutdown": shutdown(input); break;
                    case "dev": dev(); break;
                    case "open": openFile(input); break;
                    case "echo": echo(input); break;
                    case "cd": cdDir(input); break;
                    case "utime": time(); date(); break;
                    case "mkdir": mkdir(input); break;
                    //case "rm": rm(input); break;
                    //case "rmdir": rmdir(input); break;
                    case "nano": nano(input); break;
                    case "getram": getRam(); break;
                    case "startx": Console.WriteLine("GUI is currently under development, are you sure you want to continue? (y/n):");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                            isConsole = false;
                        break; //Init the GUI
                    case "mkfile":
                        string path = input.Trim().Substring(6).Trim(); //mkfile <- 6 chars
                        if (path.Length >= 2)
                            mkfile(path);
                        break;
                    case "math": parseMath(input); break;
                    case "user": createUser(input); break;
                    
                    default: error(input); break;
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
        private void help(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("File operations are currently under development. Use at your own risk.");
            Console.ForegroundColor = ConsoleColor.White;
            //Have multiple help pages the screen doesn't overflow :)
            string i = "";
            if (input.Split(' ').Length >= 2)
                i = input.Split(' ')[1];
            switch (i)
            {
                case "0": helpText(0); break;
                case "1": helpText(1); break;
                default: helpText(0); break;
            }

        }

        private void helpText(int i)
        {
            switch (i)
            {
                case 0:
                    Console.WriteLine("help:   Shows this list again. Usage help [pg#]");
                    Console.WriteLine("ls:     Shows list of files and directories.");
                    Console.WriteLine("fs:     Lists all the drives.");
                    Console.WriteLine("rm:     Removes the file. Usage: rm [path]");
                    Console.WriteLine("rmdir   Removes the directory. Usage: rmdir [path]");
                    Console.WriteLine("cd:     Change the current directory. Usage: cd [path]");
                    Console.WriteLine("mkdir:  Makes a directory. Usage: mkdir [patn]");
                    Console.WriteLine("open:   Opens and displays contents of a file. Usage: open [path]");
                    Console.WriteLine("nano:   Opens and edit the contents of a file. Usage: nano [path]");
                    Console.WriteLine("reboot: Reboots the computer. Usage: reboot [delay ms]");
                    Console.WriteLine("shutdown: Shutdowns the PC. Usage: shutdown [delay ms]");
                    Console.WriteLine("clear:  Clear the screen.");
                    Console.WriteLine("echo:   Echoes an input. Usage: echo [text to echo]");
                    Console.WriteLine("utime:  Returns unix style time.");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("For more commands, type help [page number]");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 1:
                    Console.WriteLine("getram: Returns the amount of ram available.");
                    Console.WriteLine("startx: Starts the GUI X server.");
                    Console.WriteLine("chmod: Executes a batch file. Usage chmod [path]");
                    Console.WriteLine("mkfile: Creates a file. Usage mkfile [path]");
                    Console.WriteLine("math: Computes the input. Type math -h for help on this command.");
                    Console.WriteLine("user: User operations. Type user -h for help on this command.");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("For more commands, type help [page number]");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        #endregion

        #region File Operations

        private void mkfile(string path)
        {
            try
            {
                if (!File.Exists(cd + path))
                    fs.CreateFile(cd + path);
                else if (!File.Exists(path))
                    fs.CreateFile(path);
                else
                    Console.WriteLine("File already exists " + path);
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        /*private void rmdir(string input)
        {
            //Remove a dir
            string path = input.Trim().Substring(5).Trim(); //rmdir <- 5 chars
            if (path.Length >= 2)
            {
                try
                {
                    if (VFSManager.DirectoryExists(cd + path))
                        VFSManager.DeleteFile(cd + path);
                    else if (VFSManager.DirectoryExists(path))
                        VFSManager.DeleteFile(path);
                    else
                        Console.WriteLine("File does not exist " + cd + path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void rm(string input)
        {
            //Remove a file
            string path = input.Trim().Substring(2).Trim(); //rm <- 2 chars
            if (path.Length >= 2)
            {
                try
                {
                    if(VFSManager.FileExists(cd + path))
                        VFSManager.DeleteFile(cd + path);
                    else if(VFSManager.FileExists(path))
                        VFSManager.DeleteFile(path);
                    else
                        Console.WriteLine("File does not exist " + cd + path);
                }
                catch (Exception e){
                    Console.WriteLine(e.Message);
                }
            }
        }*/

        private void mkdir(string input)
        {
            //Create a dir
            string path = input.Trim().Substring(5).Trim(); //mkdir <- 5 chars
            if (path.Length >= 2)
            {
                try
                {
                    if (!Directory.Exists(cd + path))
                        fs.CreateDirectory(cd + path);
                    else if (!Directory.Exists(path))
                        fs.CreateDirectory(path);
                    else
                        Console.WriteLine("File/Directory already exists " + cd + path);
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void cdDir(string input)
        {
            //Change the current dir
            string path = input.Trim().Substring(2).Trim(); //cd <- 2 chars
            try
            {
                if (Directory.Exists(cd + path))
                    cd = cd + path;
                else if (Directory.Exists(path))
                    cd = path;
                else
                    Console.WriteLine("File does not exist " + cd + path[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void openFile(string input)
        {
            string path = input.Trim().Substring(4).Trim(); //open <- 4 chars
            if (path.Length >= 2)
            {
                try
                {
                    if (File.Exists(cd + path))
                        Console.WriteLine(File.ReadAllText(cd + path));
                    else if (File.Exists(path))
                        Console.WriteLine(File.ReadAllText(path));
                    else
                        Console.WriteLine("File does not exist: " + cd + path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }



        #endregion

        #region File Operations Pt2

        private void dir()
        {
            try {
                if (VFSManager.GetDirectoryListing(cd).Count >= 1)
                {
                    foreach (var s in fs.GetDirectoryListing(cd))
                    {
                        if (File.Exists(s.mFullPath))
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                        else
                            Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(s.mFullPath);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                    Console.WriteLine("No files/folders in this directory.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void dev()
        {
            Console.WriteLine("Method not implemeted yet.");
        }

        private void listDrives()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All drives will be labeled in DOS-style. i.e., 0:\\ instead of C:\\");
            Console.ForegroundColor = ConsoleColor.White;
            
            foreach (var d in VFSManager.GetVolumes())
            {
                Console.WriteLine("Volume Full Path: " + d.mFullPath);
                Console.WriteLine("Volume Name:      " + d.mName);
                Console.WriteLine("Volume Size:      " + d.mSize + " bytes");
            }
        }

        #endregion

        #region Power

        private void shutdown(string input)
        {
            try
            {
                uint i = 0;
                if (input.Split(' ').Length >= 2)
                    uint.TryParse(input.Split(' ')[1], out i);
                else
                    i = 0;
                Console.Clear();
                Console.WriteLine("Halting PC...");
                //PIT pit = new PIT(); pit.waitMS(i);
                Power.shutdown();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private void reboot(string input)
        {
            try
            {
                uint i = 0;
                if (input.Split(' ').Length >= 2)
                    uint.TryParse(input.Split(' ')[1], out i);
                else
                    i = 0;
                Console.Clear();
                Console.WriteLine("Rebooting PC...");
                //PIT pit = new PIT(); pit.waitMS(i);
                Sys.Power.Reboot();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        #endregion

        #region Misc

        private void error(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Syntax error: " + input + " is not a recognised command or executable");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void clear()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(s1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s2);
            Console.WriteLine(s3);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s4);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void nano(string input)
        {
            string path = input.Trim().Substring(4).Trim(); //nano <- 4 chars
            try
            {
                Nano fm = new Nano();
                if (File.Exists(cd + path))
                    fm.initNano(cd + path);
                else if (File.Exists(path))
                    fm.initNano(path);
                else
                {
                    mkfile(cd + path);
                    fm.initNano(cd + path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void parseMath(string input)
        {
            //Remove a dir
            string ex = input.Trim().Substring(4).Trim(); //math <- 4 chars
            int res = 0; MathParser.loadAndCalc(ex, ref res);
            Console.WriteLine("Result: " + res);
        }

        private void echo(string input) { Console.WriteLine(input.Trim().Substring(input.Trim().Split(' ')[0].Length)); }
        private void time() { Console.WriteLine("Time is: " + Time.Hour() + ":" + Time.Minute() + ":" + Time.Second()); }
        private void date() { Console.WriteLine("Date is (M/D/Y): " + Time.Month() + "/" + Time.DayOfMonth() + "/" + Time.Century() + Time.Year() + " Day: " + Time.DayOfWeek()); }
        private void getRam() { Console.WriteLine("Amount of RAM installed: " + Drivers.Power.getRam() + " bytes"); }

        #endregion

        #region GUI

        private void initGUI()
        {
            int lastX = 0, lastY = 0;
            v = new VBE();
            v.init(); m = new MouseDriver(); m.init(v.vbeScreen.ScreenWidth, v.vbeScreen.ScreenHeight);
            v.DrawRect(0, v.vbeScreen.ScreenHeight - 50, v.vbeScreen.ScreenWidth, 50, 0x9A3BFF);
            while (true)
            {
                v.SetPixel(lastX, lastY, v.ClearColor);
                lastX = m.getX(); lastY = m.getY();
                v.SetPixel(m.getX(), m.getY(), 0x000000);
            }
        }

        #endregion

        #region Login

        public void displayLogin()
        {
            string[] logins = File.ReadAllLines(@"0:\users.dat");
            Login l = new Login();
            l.list(logins);
            Console.Write("Enter your username: ");
            string s = Console.ReadLine();
            Console.Write("Enter your password: ");
            string p = maskedEntry();
            Kernel.PrintDebug(logins[0] + logins.Length);
            if (l.validateLogin(0, logins[0], p, s))
            {
                isLoggedIn = true;
                usergroup = l.group; user = s; ps = p;
                Kernel.PrintDebug("We're in!");
            }
        }

        private string maskedEntry()
        {
            //Love the linux way of masking passwords
            string pass = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    //Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        //Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass;
        }

        public void createUser(string input)
        {
            string[] args = input.Trim().Substring(4).Split(' ');
            int flag = 0; string u = "", p = "", d = "";
            if(args.Length >= 1)
            {
                foreach(string s in args)
                {
                    switch(s.Trim())
                    {
                        case "-h": flag = 1; break;
                        case "-u": flag = 2; break;
                        case "-p": flag = 3; break;
                        case "-d": flag = 4; break;
                        case "-g": flag = 5; break;
                        default: flag = 0; break;
                    }
                    if(flag != 0)
                    {
                        if (flag == 1)
                        {
                            Console.WriteLine("user -h: Displays this help message.");
                            Console.WriteLine("user -u [name]: Sets the username of the account.");
                            Console.WriteLine("user -p [name]: Sets the password of the account.");
                            Console.WriteLine("user -d [name]: Sets the display name of the account.");
                            Console.WriteLine("You must set the username value of the account!");
                        }
                        else if (flag == 2)
                            u = s.Trim();
                        else if (flag == 3)
                            p = s.Trim();
                        else if (flag == 4)
                            d = s.Trim();
                        else
                            flag = 0;

                    }
                }
                if(string.IsNullOrWhiteSpace(u))
                {
                    Login l = new Login();
                    string[] logins = File.ReadAllLines(@"0:\users.dat");
                    l.createUser(logins, u, p, 0, d);
                }
                else
                    Console.WriteLine("You must specify a username!");
            }
        } //End void

        #endregion
    }
}