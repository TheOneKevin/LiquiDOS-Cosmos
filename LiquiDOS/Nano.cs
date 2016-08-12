using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sys = Cosmos.System;

namespace LiquiDOS
{
    public class Nano
    {
        char[] line = new char[80]; int pointer = 0;
        List<string> lines = new List<string>();
        string[] final;
        public void initNano(string path)
        {
            Console.Clear();
            drawTopBar();
            Console.SetCursorPosition(0, 1);
            ConsoleKeyInfo c;
            while((c = Console.ReadKey(true)) != null)
            {
                drawTopBar();
                char ch = c.KeyChar;
                if (c.Key == ConsoleKey.Escape)
                    break;
                else if((c.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if (c.Key == ConsoleKey.X)
                    {
                        lines.Add(new string(line).Trim()); //Add any unadded lines
                        listCheck(); final = lines.ToArray(); //Store vars
						string foo = concatString(final); //Get the final text
                        Kernel.PrintDebug(concatString(arrayCheck(final)));
						Console.WriteLine("Here comes the concated text: \n" + foo);
						File.WriteAllText(path, foo); //Write to file
                        Console.ReadKey();
                        break;
                    }
                    /*else if(c.Key == ConsoleKey.End)
                    {
                        displayHelp(path);
                    }*/
                }

                switch(c.Key)
                {
                    case ConsoleKey.UpArrow: break;
                    case ConsoleKey.DownArrow: break;
                    case ConsoleKey.LeftArrow: if (pointer > 0) { pointer--; Console.CursorLeft--; } break;
                    case ConsoleKey.RightArrow: if (pointer < 80) { pointer++; Console.CursorLeft++; if (line[pointer] == 0) line[pointer] = ' '; } break;
                    case ConsoleKey.Backspace: deleteChar(); break;
                    case ConsoleKey.Delete: deleteChar(); break;
                    case ConsoleKey.Enter:
                        /*char[] split = doTheTing(); //We split the string at the cursor pos
                        //Store the first half into the list, store the second half into char
                        Console.Write("\n");
                        cleanArray(line); appendIntoChar(split); pointer = split.Length;
                        Console.CursorLeft = 0;
                        Console.CursorTop--; Console.Write(new string(doTheThing()).TrimEnd());
                        Console.CursorTop++; Console.Write(new string(split).TrimEnd());
                        Console.CursorLeft = pointer;*/
                        lines.Add(new string(line).Trim()); cleanArray(line); Console.CursorLeft = 0; Console.CursorTop++;
                        break;
                    default: line[pointer] = ch; pointer++; Console.Write(ch); break;
                }
            }
            Kernel.clear();
        }

        private string concatString(string[] s)
        {
            string t = "";
            if (s.Length >= 1)
            {
                for (int i = 1; i < s.Length; i++)
                {
                    t = string.Concat(t, s[i], Environment.NewLine);
                }
            }
            else
                t = s[0];
            Kernel.PrintDebug("Concat: " + t + "\n");
            return t;
        }

        private void cleanArray(char[] c)
        {
            for (int i = 0; i < c.Length; i++)
                c[i] = ' ';
        }

        private void drawTopBar()
        {
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("LiquiDOS Text Editor ALPHA v0.01 Use at your own risk!  ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[CTRL+X]Save   [ESC]Exit\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(x, y);
        }

        //Decrease the pointer, decrease the cusor pos by 1, write whitespace, decrease
        //cursor pos by 1 again, replace char in buffer with whitespace
        private void deleteChar()
        {
            if ((Console.CursorLeft >= 1) && (pointer >= 1))
            {
                pointer--; Console.CursorLeft--;
                Console.Write(" "); Console.CursorLeft--;
                line[pointer] = ' ';
            }
        }

        private void listCheck()
        {
            foreach (var s in lines)
                Kernel.PrintDebug(" List: " + s + "\n"); //Works!
        }

        private string[] arrayCheck(string[] s)
        {
            foreach (var ss in s)
            {
                Kernel.PrintDebug(" Line: " + ss + "\n"); //Works!
            }
            return s; //Just return the input...
        }

        public void displayHelp(string input)
        {
            Console.Clear(); cleanArray(line);
            Console.WriteLine("Press Ctrl+X to save. Press Esc to exit without saving.");
            Console.WriteLine("Files will be overwritten if they exist. If no file exists, one will be");
            Console.WriteLine("created. Right now, nano will not display the contents of the files, so you");
            Console.WriteLine("start with a clean slate every time. Each line is 80 characters long. You");
            Console.WriteLine("cannot edit a line above. You can only edit the current line.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(); initNano(input);
        }

        //What does this do again? I kinda forgot. Wrote this when I was in a trance :p
        public char[] doTheTing() //Ting?! Srsly?!
        {
            char[] foo = new char[pointer];
            char[] foobar = new char[80 - pointer];
            for (int i = 0; i < pointer; i++)
                foo[i] = line[i];
            for (int i = 0; i < 80 - pointer; i++)
            {
                foobar[i] = line[pointer + i];
            }
            
            lines.Add(new string(foo).TrimEnd());
            return foobar;
        }

        //What does this do again?
        public char[] doTheThing()
        {
            char[] foo = new char[pointer];
            char[] foobar = new char[80 - pointer];
            for (int i = 0; i < pointer; i++)
                foo[i] = line[i];
            for (int i = 0; i < 80 - pointer; i++)
                foobar[i] = line[pointer + i];
            return foo;
        }

        private void appendIntoChar(char[] arrayToAppend)
        {
            for (int i = 0; i < arrayToAppend.Length; i++)
                line[i] = arrayToAppend[i];
        }

        public void getCharUpDown(bool isDown)
        {
            if(isDown)
            {

            }
            else
            {

            }
        }
    }
}
