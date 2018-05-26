using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace S2T
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        // Speech Recognizer
        private SpeechRecognizer recognizer;
        private const string SRGS_FILE = "Grammar\\grammar.xml";

        public MainPage()
        {
            this.InitializeComponent();


            initializeSpeechRecognizer();
        }

        private async void initializeSpeechRecognizer()
        {
            // Initialize recognizer
            var language = new Windows.Globalization.Language("en-US");
            recognizer = new SpeechRecognizer(language);
            recognizer.ContinuousRecognitionSession.AutoStopSilenceTimeout = System.TimeSpan.FromMinutes(1); ;

            // Set event handlers
            recognizer.StateChanged += RecognizerStateChanged;
            recognizer.ContinuousRecognitionSession.ResultGenerated += RecognizerResultGenerated;


            // Load Grammer file constraint
            string fileName = String.Format(SRGS_FILE);
            StorageFile grammarContentFile = await Package.Current.InstalledLocation.GetFileAsync(fileName);

            SpeechRecognitionGrammarFileConstraint grammarConstraint = new SpeechRecognitionGrammarFileConstraint(grammarContentFile);

            // Add to grammer constraint
            recognizer.Constraints.Add(grammarConstraint);

            // Compile grammer
            SpeechRecognitionCompilationResult compilationResult = await recognizer.CompileConstraintsAsync();

            Debug.WriteLine("Status: " + compilationResult.Status.ToString());

            // If successful, display the recognition result.
            if (compilationResult.Status == SpeechRecognitionResultStatus.Success)
            {
                Debug.WriteLine("Result: " + compilationResult.ToString());

                await recognizer.ContinuousRecognitionSession.StartAsync();
            }
            else
            {
                Debug.WriteLine("Status: " + compilationResult.Status);
            }
        }

        private async void RecognizerResultGenerated(SpeechContinuousRecognitionSession session, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            // Output debug strings
            Debug.WriteLine(args.Result.Status);
            Debug.WriteLine(args.Result.Text);
            String assistant = args.Result.SemanticInterpretation.Properties.ContainsKey("cmd") ?
                            args.Result.SemanticInterpretation.Properties["cmd"][0].ToString() :
                            "";

            if(assistant.Equals("ALEXA"))
            {
                Debug.WriteLine("Hi I am Alexa");
            }
            if(assistant.Equals("CORTANA"))
            {
                Debug.WriteLine("Hi I am Cortana");
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {

                    StartListen();

                });
            }
        }

        private async void RecognizerStateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            Debug.WriteLine("Speech recognizer state: " + args.State.ToString());
            if (sender.State == SpeechRecognizerState.Idle)
            {
                await sender.ContinuousRecognitionSession.StartAsync();
            }
        }

        private async void StartListen()
        {
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

            await speechRecognizer.CompileConstraintsAsync();

            speechRecognizer.UIOptions.IsReadBackEnabled = false;

            Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeWithUIAsync();

            var messageDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Text spoken");

            await messageDialog.ShowAsync();

            // The media object for controlling and playing audio.
            MediaElement mediaElement = new MediaElement();

            // The object for controlling the speech synthesis engine (voice).
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();

            // Generate the audio stream from plain text.
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(speechRecognitionResult.Text);

            // Send the stream to the media object.
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            //var language = new Windows.Globalization.Language("hi-in");
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

            await speechRecognizer.CompileConstraintsAsync();

            speechRecognizer.UIOptions.IsReadBackEnabled = false;

            Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeWithUIAsync();

            var messageDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Text spoken");

            await messageDialog.ShowAsync();

            // The media object for controlling and playing audio.
            MediaElement mediaElement = new MediaElement();

            // The object for controlling the speech synthesis engine (voice).
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();

            // Generate the audio stream from plain text.
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(speechRecognitionResult.Text);

            // Send the stream to the media object.
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
