using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
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

        private SpeechRecognizer listner;
        private CoreDispatcher dispatcher;
        private StringBuilder dictatedTextBuilder;
        public MainPage()
        {
            this.InitializeComponent();

            this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            this.listner = new SpeechRecognizer();



            

            Task.Factory.StartNew(async () =>
            {
                SpeechRecognitionCompilationResult result = await listner.CompileConstraintsAsync();
                listner.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;

                while (true)
                {
                    if (listner.State == SpeechRecognizerState.Idle)
                    {
                        await listner.ContinuousRecognitionSession.StartAsync();
                    } 
                }
            });

            //Task.Factory.StartNew(async () =>
            //{
            //    while (true)
            //    {
            //        var listner = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            //        //Windows.Media.SpeechRecognition.SpeechRecognitionResult result = await listner.RecognizeAsync();

            //        await listner.Speech
            //        if (result.Text.Contains("hey dial"))
            //        {
            //            await Myfunc();
            //        }
            //    }


            //});
        }

        private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {

            if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||args.Result.Confidence == SpeechRecognitionConfidence.High)
            {
                dictatedTextBuilder = new StringBuilder();
                dictatedTextBuilder.Append(args.Result.Text + " ");

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (dictatedTextBuilder.ToString().Contains("hey dial"))
                    {
                        if (listner.State != SpeechRecognizerState.Idle)
                        {
                            await listner.ContinuousRecognitionSession.CancelAsync();
                        }
                        Start.ClickMode = ClickMode.Press;
                    }
                });
            }
            else
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {

                });
            }
        }

        private async Task Myfunc()
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

            if (listner.State == SpeechRecognizerState.Idle)
            {
                await listner.ContinuousRecognitionSession.StartAsync();
            }
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

            if (listner.State == SpeechRecognizerState.Idle)
            {
                await listner.ContinuousRecognitionSession.StartAsync();
            }

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
