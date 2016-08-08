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
        private string cd = @"0:\";
        private List<string> commandHistory = new List<string>();

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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Loading complete...");
            Console.WriteLine("Currently under development. Not responsible for any damages. Proceed at your own risk.");
            Console.WriteLine("Copyright (c) 2016 Kevin Dai All Rights Reserved.");
            Console.WriteLine("             Welcome to LquiDOS. Type help for a list of commands.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        protected override void Run()
        {
            //Init the console. Get the current dir (cd)
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(cd); Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("> "); Console.ForegroundColor = ConsoleColor.White;
            var input = Console.ReadLine();
            commandHistory.Add(input);
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

                default: error(input); break;
            }
        }

        private void help(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("File operations are currently under development. Use at your own risk.");
            Console.ForegroundColor = ConsoleColor.White;
            //Have multiple help pages the screen doesn't overflow :)
            int i = 0;
            if (input.Split(' ').Length >= 2)
                int.TryParse(input.Split(' ')[1], out i);
            switch (i)
            {
                case 0:
                    Console.WriteLine("help:   Shows this list again.");
                    Console.WriteLine("ls:     Shows list of files and directories.");
                    Console.WriteLine("fs:     Lists all the drives.");
                    Console.WriteLine("rm:     Removes the directory/file. Usage: rm [path]");
                    Console.WriteLine("cd:     Change the current directory. Usage: cd [path]");
                    Console.WriteLine("mkdir:  Makes a directory. Usage: mkdir [patn]");
                    Console.WriteLine("open:   Opens and displays contents of a file. Usage: open [path]");
                    Console.WriteLine("reboot: Reboots the computer.");
                    Console.WriteLine("clear:  Clear the screen.");
                    Console.WriteLine("echo:   Echoes an input. Usage: echo [text to echo]");
                    Console.WriteLine("utime:  Returns unix style time.");
                    break;
                case 1:
                    break;
                default: break;
            }

        }

        #endregion

        #region File Operations

        private void rm(string input)
        {
            //Remove a dir
            string[] path = input.Trim().Split(' ');
            if (path.Length >= 2)
            {
                try
                {
                    if (VFSManager.DirectoryExists(cd + path[1]))
                        VFSManager.DeleteFile(cd + path[1]);
                    else if (VFSManager.DirectoryExists(path[1]))
                        VFSManager.DeleteFile(path[1]);
                    else if(VFSManager.FileExists(cd + path[1]))
                        VFSManager.DeleteFile(cd + path[1]);
                    else if(VFSManager.FileExists(path[1]))
                        VFSManager.DeleteFile(path[1]);
                    else
                        Console.WriteLine("File/Directory does not exist " + cd + path[1]);
                }
                catch (Exception e){
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void mkdir(string input)
        {
            //Create a dir
            string[] path = input.Trim().Split(' ');
            if (path.Length >= 2)
            {
                try
                {
                    if (!VFSManager.DirectoryExists(cd + path[1]))
                        fs.CreateDirectory(cd + path[1]);
                    else if (!VFSManager.DirectoryExists(path[1]))
                        fs.CreateDirectory(path[1]);
                    else
                        Console.WriteLine("File/Directory already exists " + cd + path[1]);
                }
                catch(Exception e) { 
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void cdDir(string input)
        {
            //Change the current dir
            string[] path = input.Trim().Split(' ');
            if(path.Length >= 2)
            {
                try
                {
                    if (VFSManager.DirectoryExists(cd + path[1]))
                        cd = cd + path[1];
                    else if (VFSManager.DirectoryExists(path[1]))
                        cd = path[1];
                    else
                        Console.WriteLine("File does not exist " + cd + path[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void openFile(string input)
        {
            string lines = "";
            string[] path = input.Trim().Split(' ');
            if (path.Length >= 2)
            {
                try
                {
                    if (File.Exists(cd + path[1]))
                        lines = File.ReadAllText(cd + path[1]);
                    else if (File.Exists(path[1]))
                        lines = File.ReadAllText(path[1]);
                    else
                        Console.WriteLine("File does not exist: " + cd + path[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.Write(lines + "\n");
        }



        #endregion

        #region File Operations Pt2

        private void dir()
        {
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

        private void clear()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Copyright (c) 2016 Kevin Dai, all rights reserved.");
            Console.WriteLine("Welcome to LquiDOS. Type help for a list of commands.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void reboot(){ Sys.Power.Reboot(); }
        private void echo(string input) { Console.WriteLine(input.Trim().Substring(input.Trim().Split(' ')[0].Length)); }
        private void time() { Console.WriteLine("Time is: " + Time.Hour() + ":" + Time.Minute() + ":" + Time.Second()); }
        private void date() { Console.WriteLine("Date is (M/D/Y): " + Time.Month() + "/" + Time.DayOfMonth() + "/" + Time.Century() + Time.Year() + " Day: " + Time.DayOfWeek()); }

        #endregion

    }
}