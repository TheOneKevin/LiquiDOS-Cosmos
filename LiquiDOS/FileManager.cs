
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sys = Cosmos.System;

namespace LiquiDOS
{
    class FileManager
    {
        char[] ch = new char[80]; int pointer = 0; //Dirty
        List<string> lines = new List<string>(); string[] line;
        private void writeToFile(string path)
        {
			try
            {
                //File.WriteAllLines(path, new string[] { "hello", "world" }); //<- Multiline saved!
                File.WriteAllText(path, arrayToText()); //Array to text (stripped from Cosmos) is BAD!
			}
			catch (Exception e){ Console.WriteLine(e.Message); }
        }

        private void drawTopBar()
        {
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("LiquiDOS Text Editor ALPHA v0.01 Use at your own risk!  ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[CTRL+X]Save   [ESC]Exit\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(x, y);
        }

        public void initNano(string path)
        {
            Console.Clear();
            //TODO: Add screen buffer
            drawTopBar();
            Console.CursorTop++;
            ConsoleKeyInfo c;

            while ((c = Console.ReadKey(true)) != null)
            {
                drawTopBar();
                if (c.Key == ConsoleKey.Escape)
				{
					Kernel.clear(); break;
				}
                if((c.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if(c.Key == ConsoleKey.X)
					{
						Kernel.clear();
                        lines.Add(new string(ch).Trim());
                        listCheck();
                        line = lines.ToArray();
                        writeToFile(path);
                        break;
					}
                }
                switch (c.Key)
                {
                    case ConsoleKey.UpArrow: break;
                    case ConsoleKey.DownArrow: break;
                    case ConsoleKey.LeftArrow: break;
                    case ConsoleKey.RightArrow: break;
                    //To delete, we move the cursor, place a whitespace, move the cursor again, replace the char inside the array with whitespace
                    case ConsoleKey.Backspace: if (Console.CursorLeft >= 1) { Console.CursorLeft -= 1; Console.Write(" ");  Console.CursorLeft -= 1; pointer--; ch[pointer] = ' '; }  break;
                    case ConsoleKey.Delete: if (Console.CursorLeft >= 1) { Console.CursorLeft -= 1; Console.Write(" "); Console.CursorLeft -= 1; pointer--; ch[pointer] = ' '; } break;
                    case ConsoleKey.Enter: Console.Write("\n");
                        lines.Add(new string(ch).Trim()); cleanArray(ch); Kernel.PrintDebug(lines.Count + ""); //See if the list is incrementing
                            break; //Increase line number, store line, clean line array, next line
                    default: Console.Write(c.KeyChar); ch[pointer] = c.KeyChar; pointer++; break;
                }
            }
        }

        /*private void stringBuilder(char c)
        {
            ch[pointer] = c;
        }*/

        private void cleanArray(char[] array)
        {
            pointer = 0;
            for(int i = 0; i < array.Length;)
            {
                array[i] = ' ';
                i++;
            }
        }

        private string arrayToText()
        {
            string text = ""; Kernel.PrintDebug("Line length: " + line.Length);
            for (int i = 0; i < line.Length; i++)
            {
                Kernel.PrintDebug(" LineConcat: " + line[i]);
                text = string.Concat(text, line[i]); //<<ALL ARROWS POINT HERE, THIS LITTLE FELLA'
                Kernel.PrintDebug(" LineConcatLine: " + text + "," + i);
            }
            Kernel.PrintDebug(" LineConcatText: " + text);
            return text;
        }

        private void listCheck()
        {
            foreach (var s in lines)
                Kernel.PrintDebug(" List: " + s); //Works!
        }

        private string[] arrayCheck(string[] s)
        {
            foreach (var ss in s)
            {
                Kernel.PrintDebug(" Line: " + ss); //Works!
            }
            return s; //Just return the input...
        }

        public void displayHelp(string input)
        {
            Console.Clear(); cleanArray(ch);
            Console.WriteLine("Press Ctrl+X to save. Press Esc to exit without saving.");
            Console.WriteLine("Files will be overwritten if they exist. If no file exists, one will be");
            Console.WriteLine("created. Right now, nano will not display the contents of the files, so you");
            Console.WriteLine("start with a clean slate every time. Each line is 80 characters long. You");
            Console.WriteLine("cannot edit a line above. You can only edit the current line.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(); initNano(input);
        }
    }
}
