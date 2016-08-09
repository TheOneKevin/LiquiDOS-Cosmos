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
        public static string cd = @"0:\";
        private List<string> commandHistory = new List<string>();
        static string s1 = "               Copyright (c) 2016 Kevin Dai All Rights Reserved.";
        static string s2 = "         Currently under development. Not responsible for any damages.";
        static string s3 = "                           Proceed at your own risk.";
        static string s4 = "          Welcome to LquiDOS.Type help [pg #] for a list of commands.";

        #endregion

        #region Basics

        protected override void BeforeRun()
        {
            //Init the filesystem
            fs = new Sys.FileSystem.CosmosVFS();
            VFSManager.RegisterVFS(fs);
            fs.Initialize();
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
        }

        protected override void Run()
        {
            //Init the console. Get the current dir (cd)
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(cd); Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> "); Console.ForegroundColor = ConsoleColor.White;
            var input = Console.ReadLine();
            commandHistory.Add(input);
            try
            {
                switch (input.Trim().Split(' ')[0])
                {
                    case "help": help(input); break;
                    case "ls": dir(); break;
                    case "fs": listDrives(); break;
                    case "clear": clear(); break;
                    case "reboot": reboot(); break;
                    case "dev": dev(); break;
                    case "open": openFile(input); break;
                    case "echo": echo(input); break;
                    case "cd": cdDir(input); break;
                    case "utime": time(); date(); break;
                    case "mkdir": mkdir(input); break;
                    case "rm": rm(input); break;
                    case "rmdir": rmdir(input); break;
                    case "nano": nano(input); break;

                    default: error(input); break;
                }
            }
            catch(Exception e) { Console.WriteLine(e.Message); }
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
                case "0":
                    Console.WriteLine("help:   Shows this list again. Usage help [pg#]");
                    Console.WriteLine("ls:     Shows list of files and directories.");
                    Console.WriteLine("fs:     Lists all the drives.");
                    Console.WriteLine("rm:     Removes the file. Usage: rm [path]");
                    Console.WriteLine("rmdir   Removes the directory. Usage: rmdir [path]");
                    Console.WriteLine("cd:     Change the current directory. Usage: cd [path]");
                    Console.WriteLine("mkdir:  Makes a directory. Usage: mkdir [patn]");
                    Console.WriteLine("open:   Opens and displays contents of a file. Usage: open [path]");
                    Console.WriteLine("nano:   Opens and edit the contents of a file. Usage: nano [path]");
                    Console.WriteLine("reboot: Reboots the computer.");
                    Console.WriteLine("clear:  Clear the screen.");
                    Console.WriteLine("echo:   Echoes an input. Usage: echo [text to echo]");
                    Console.WriteLine("utime:  Returns unix style time.");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("For more commands, type help [page number]");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "1":
                    break;
                default: break;
            }

        }

        #endregion

        #region File Operations

        private void mkfile(string input)
        {

        }

        private void rmdir(string input)
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
        }

        private void mkdir(string input)
        {
            //Create a dir
            string path = input.Trim().Substring(5).Trim(); //mkdir <- 5 chars
            if (path.Length >= 2)
            {
                try
                {
                    if (!VFSManager.DirectoryExists(cd + path))
                        fs.CreateDirectory(cd + path);
                    else if (!VFSManager.DirectoryExists(path))
                        fs.CreateDirectory(path);
                    else
                        Console.WriteLine("File/Directory already exists " + cd + path);
                }
                catch(Exception e) { 
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
                if (VFSManager.DirectoryExists(cd + path))
                    cd = cd + path;
                else if (VFSManager.DirectoryExists(path))
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
                    {
                        foreach (string s in File.ReadAllLines(cd + path))
                            Console.WriteLine(s);
                    }
                    else if (File.Exists(path))
                    {
                        foreach (string s in File.ReadAllLines(path))
                            Console.WriteLine(s);
                    }
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
            //Remove a dir
            string path = input.Trim().Substring(4).Trim(); //nano <- 4 chars
            if (path.Length >= 2)
            {
                try
                {
                    FileManager fm = new FileManager();
                    if (File.Exists(cd + path))
                        fm.initNano(cd + path);
                    else if (File.Exists(path))
                        fm.initNano(path);
                    else
                    {
                        fs.CreateFile(cd + path);
                        fm.initNano(cd + path);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void reboot(){ Sys.Power.Reboot(); }
        private void echo(string input) { Console.WriteLine(input.Trim().Substring(input.Trim().Split(' ')[0].Length)); }
        private void time() { Console.WriteLine("Time is: " + Time.Hour() + ":" + Time.Minute() + ":" + Time.Second()); }
        private void date() { Console.WriteLine("Date is (M/D/Y): " + Time.Month() + "/" + Time.DayOfMonth() + "/" + Time.Century() + Time.Year() + " Day: " + Time.DayOfWeek()); }

        #endregion
    }
}