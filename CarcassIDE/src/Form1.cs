using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CarcassIDE
{
    public partial class Form1 : Form
    {
        string openFilePath;
        public Form1()
        {
            InitializeComponent();
        }
        private void InitCustomFontTextBox(RichTextBox box)
        {
            //Create your private font collection object.
            PrivateFontCollection pfc = new PrivateFontCollection();

            //Select your font from the resources.
            //My font here is "Digireu.ttf"

            int fontLength = File.ReadAllBytes("Monaco.ttf").Length;

            // create a buffer to read in to
            byte[] fontdata = File.ReadAllBytes("Monaco.ttf");

            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);

            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);
        }
        private void ApplyMonacoBox(RichTextBox box, int size)//--RICHTEXTBOX.
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile("Monaco.ttf");
            box.Font = new Font(pfc.Families[0], size);
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (openFilePath != null)
            {
                // getting keywords/functions
                string keywords = @"\b(fn|print|return|if|input|asInt)\b";
                MatchCollection keywordMatches = Regex.Matches(richTextBox1.Text, keywords);

                // getting types/classes from the text 
                string types = @"\b( var )\b";
                MatchCollection typeMatches = Regex.Matches(richTextBox1.Text, types);

                // getting comments (inline or multiline)
                string comments = @"(\/\/.+?$|\/\*.+?\*\/)";
                MatchCollection commentMatches = Regex.Matches(richTextBox1.Text, comments, RegexOptions.Multiline);

                // getting strings
                string strings = "\".+?\"";
                MatchCollection stringMatches = Regex.Matches(richTextBox1.Text, strings);

                string numbers = "\\d*";
                MatchCollection numberMatches = Regex.Matches(richTextBox1.Text, numbers);

                // saving the original caret position + forecolor
                int originalIndex = richTextBox1.SelectionStart;
                int originalLength = richTextBox1.SelectionLength;
                Color originalColor = Color.White;

                // MANDATORY - focuses a label before highlighting (avoids blinking)
                label1.Focus();

                // removes any previous highlighting (so modified words won't remain highlighted)
                richTextBox1.SelectionStart = 0;
                richTextBox1.SelectionLength = richTextBox1.Text.Length;
                richTextBox1.SelectionColor = originalColor;

                // scanning...
                foreach (Match m in keywordMatches)
                {
                    richTextBox1.SelectionStart = m.Index;
                    richTextBox1.SelectionLength = m.Length;
                    richTextBox1.SelectionColor = Color.Aqua;
                }

                foreach (Match m in typeMatches)
                {
                    richTextBox1.SelectionStart = m.Index;
                    richTextBox1.SelectionLength = m.Length;
                    richTextBox1.SelectionColor = Color.Azure;
                }

                foreach (Match m in commentMatches)
                {
                    richTextBox1.SelectionStart = m.Index;
                    richTextBox1.SelectionLength = m.Length;
                    richTextBox1.SelectionColor = Color.LightGreen;
                }

                foreach (Match m in stringMatches)
                {
                    richTextBox1.SelectionStart = m.Index;
                    richTextBox1.SelectionLength = m.Length;
                    richTextBox1.SelectionColor = Color.Pink;
                }

                foreach (Match m in numberMatches)
                {
                    richTextBox1.SelectionStart = m.Index;
                    richTextBox1.SelectionLength = m.Length;
                    richTextBox1.SelectionColor = Color.LightCyan;
                }

                // restoring the original colors, for further writing
                richTextBox1.SelectionStart = originalIndex;
                richTextBox1.SelectionLength = originalLength;
                richTextBox1.SelectionColor = originalColor;

                // giving back the focus
                richTextBox1.Focus();
            }
            else
            {
                richTextBox1.Text = "";
                MessageBox.Show("You need to open a file to start writing your code!");
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CarcassIDE");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CarcassIDE\\Projects");
            ApplyMonacoBox(richTextBox1, 9);
            RichTextBoxExtensions.SetInnerMargins(richTextBox1, 10, 7, 0, 0);
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            File.WriteAllLines(openFilePath, richTextBox1.Text.Split('\v'), Encoding.Default);
            System.Diagnostics.Process processs = new System.Diagnostics.Process();
             System.Diagnostics.ProcessStartInfo startsInfo = new System.Diagnostics.ProcessStartInfo();
             startsInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
             startsInfo.CreateNoWindow = false;
             startsInfo.FileName = @"CarcassI.exe";
             startsInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
             startsInfo.Arguments = $"run \"{openFilePath}\"";

             processs.StartInfo = startsInfo;
             processs.Start();
            /*string strCmdText;
            strCmdText = $"/C cd {Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)} && CarcassI.exe run \"{openFilePath}\"";
            MessageBox.Show(strCmdText);
            System.Diagnostics.Process.Start("CMD.exe", strCmdText);*/

        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            openFilePath = openFileDialog1.FileName;
            richTextBox1.Text = File.ReadAllText(openFilePath);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            File.WriteAllLines(openFilePath, richTextBox1.Text.Split('\v'), Encoding.Default);
        }
    }
    public static class RichTextBoxExtensions
    {
        public static void SetInnerMargins(this TextBoxBase textBox, int left, int top, int right, int bottom)
        {
            var rect = textBox.GetFormattingRect();

            var newRect = new Rectangle(left, top, rect.Width - left - right, rect.Height - top - bottom);
            textBox.SetFormattingRect(newRect);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;

            private RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
            {
            }
        }

        [DllImport(@"User32.dll", EntryPoint = @"SendMessage", CharSet = CharSet.Auto)]
        private static extern int SendMessageRefRect(IntPtr hWnd, uint msg, int wParam, ref RECT rect);

        [DllImport(@"user32.dll", EntryPoint = @"SendMessage", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, ref Rectangle lParam);

        private const int EmGetrect = 0xB2;
        private const int EmSetrect = 0xB3;

        private static void SetFormattingRect(this TextBoxBase textbox, Rectangle rect)
        {
            var rc = new RECT(rect);
            SendMessageRefRect(textbox.Handle, EmSetrect, 0, ref rc);
        }

        private static Rectangle GetFormattingRect(this TextBoxBase textbox)
        {
            var rect = new Rectangle();
            SendMessage(textbox.Handle, EmGetrect, (IntPtr)0, ref rect);
            return rect;
        }
    }
}