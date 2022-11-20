using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;
using System.Threading;

using CSScriptLib;
using System.Net;

namespace VOCzel_v1
{

    public partial class Form1 : Form
    {

        public static List<string> LoadedCommands = new List<string>();

        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();

        // SpeechRecognition
        SpeechRecognitionEngine defaultCommands = new SpeechRecognitionEngine();

        // SpeechSynthesizer
        static SpeechSynthesizer speech = new SpeechSynthesizer();

        // SpeechRecognition when Start listening 
        SpeechRecognitionEngine startlistening = new SpeechRecognitionEngine();
        static Choices choices = new Choices();

        public static string name = Environment.UserName;

        public Form1()
        {
            InitializeComponent();

            SetupCheck();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadCommands();
            AllCommands.Text = string.Join("\n", LoadedCommands);
            commandCount.Text = LoadedCommands.Count.ToString();
            // Start _recognizer
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammarAsync(new Grammar(choices));
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recEngine_StartRecognizion);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            // Does Commands once KEYWORD is said
            defaultCommands.SetInputToDefaultAudioDevice();
            defaultCommands.LoadGrammarAsync(new Grammar(choices));
            defaultCommands.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recEngine_SecondRecognizion);

        }

        private void recEngine_StartRecognizion(object sender, SpeechRecognizedEventArgs e)
        {

            string result = e.Result.Text;

            if (result == "Voczel")
            {

                result = $"Hello {name}.";
                _recognizer.RecognizeAsyncCancel();
                defaultCommands.RecognizeAsync(RecognizeMode.Multiple);
                // Stop Recognition

                speech.SpeakAsync(result);
                label4.Text = result;

            }

            

        }

        private void recEngine_SecondRecognizion(object sender, SpeechRecognizedEventArgs e)
        {

            string result = e.Result.Text;

            if (result != "Voczel")
            {

                foreach (string commandFile in Directory.GetFiles(Environment.CurrentDirectory + "//Commands"))
                {

                    if (commandFile.Contains(".cs"))
                    {
                        string FirstParse = commandFile.Split('\\').Last();
                        string commandName = FirstParse.Split('.').First();

                        if (result == commandName)
                        {
                            result = $"Running {commandName}";

                            speech.SpeakAsync(result);
                            label4.Text = result;

                            string commandPath = commandFile.Replace("//", "\\");

                            string command = File.ReadAllText(commandPath);

                            Execute(command);

                        }
                    }

                }

            }
            if(result == "Stop Listening")
            {
                result = $"No longer listening";
                speech.SpeakAsync(result);
                label4.Text = result;

                restartListening();

            }
            if (result == "Shutdown")
            {
                result = $"Goodbye {name}";
                speech.SpeakAsync(result);
                label4.Text = result;
                Thread.Sleep(1000);
                Application.Exit();

            }
            if (result == "Mute")
            {
                    speech.SpeakAsync("As you wish " + name);
                    System.Threading.Thread.Sleep(1000);
                    speech.Volume = 0;
                    label4.Text = "As you wish " + name;

            }
            if (result == "Unmute")
            {
                speech.Volume = 100;
                speech.SpeakAsync("As you wish " + name);
                label4.Text = "As you wish " + name;

            }
            if (result == "Reload Commands")
            {

                loadCommands();
                speech.SpeakAsync("As you wish " + name);
                label4.Text = "As you wish " + name;

                AllCommands.Clear();

                AllCommands.Text = string.Join("\n", LoadedCommands);
                commandCount.Text = LoadedCommands.Count.ToString();

            }
            if(result == "Hide")
            {
                this.Hide();
                speech.SpeakAsync("As you wish " + name);
                notifyIcon.Visible = true;
            }
            if(result == "Unhide")
            {
                this.Show();
                speech.SpeakAsync("As you wish " + name);
                notifyIcon.Visible = false;
            }



            restartListening();

        }

        private void restartListening()
        {
            try
            {
                defaultCommands.RecognizeAsyncStop();
                defaultCommands.RecognizeAsyncCancel();

                _recognizer.RecognizeAsyncStop();
                _recognizer.RecognizeAsyncCancel();
                Thread.Sleep(1000);
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch(Exception ex)
            {

                MessageBox.Show("Something failed: " + ex);

                defaultCommands.RecognizeAsyncStop();
                defaultCommands.RecognizeAsyncCancel();

                _recognizer.RecognizeAsyncStop();
                _recognizer.RecognizeAsyncCancel();
                Thread.Sleep(1000);
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void SetupCheck()
        {
            
            if(!Directory.Exists(Environment.CurrentDirectory + "//Commands")){
                Directory.CreateDirectory(Environment.CurrentDirectory + "//Commands");

                WebClient client = new WebClient();
                string reply = client.DownloadString("https://pastebin.com/raw/5kN9ZfVD");

                using (TextWriter tw = new StreamWriter(Environment.CurrentDirectory + "//Commands/example.cs"))
                {

                    tw.Write(reply);

                }

            }

        }

        private void loadCommands()
        {

            LoadedCommands.Clear();
            defaultCommands.UnloadAllGrammars();

            string prefix = "Voczel";
            string stopListenCMD = "Stop Listening";

            string exitProgramCMD = "Shutdown";
            string Mute = "Mute";
            string Unmute = "Unmute";
            string reloadCommands = "Reload Commands";

            string Hide = "Hide";
            string unHide = "Unhide";

            choices.Add(prefix);
            choices.Add(stopListenCMD);
            choices.Add(exitProgramCMD);
            choices.Add(Mute);
            choices.Add(Unmute);
            choices.Add(reloadCommands);
            choices.Add(Hide);
            choices.Add(unHide);

            //Load all other Custom Commands
            foreach (string commandFile in Directory.GetFiles(Environment.CurrentDirectory + "//Commands"))
            {

                if (commandFile.Contains(".cs"))
                {
                    string FirstParse = commandFile.Split('\\').Last();
                    string commandName = FirstParse.Split('.').First();

                    choices.Add(commandName);

                    LoadedCommands.Add(commandName);

                }

            }

            defaultCommands.LoadGrammarAsync(new Grammar(choices));

        }

        public static void Execute(string code)
        {
            try
            {
                dynamic block = CSScript.Evaluator
                               .ReferenceAssembliesFromCode(code)
                               .LoadCode(code);
                block.ExecuteAFunction();
            }
            catch(Exception ex)
            {
                speech.SpeakAsync("Failed to run command.");
                Clipboard.SetText(ex.Message);
                MessageBox.Show(ex.Message);
            }

        }

        

    }
}
