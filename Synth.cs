using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace synth
{
    public partial class Mic_Synth : Form
    {
        Random random;
        private const int SAMPLE_RATE = 44100; // частота дискретизации (программа в секунду генерирует 44100 сэмплов)
        private const short BITS_PER_SAMPLE = 16; // задаёт аппроксимацию сигнала 
        private float frequency;
        // частота сигнала, по умолчанию - 440 Hz
        private int temperation;
        // темперация (на какое число будет равномерно делиться октава (октава - отношение 1:2))
        private int base_tone;
        // базовый тон определяет, на степень какого числа будут изменяться частоты 
        // (дефолт - 2: каждая нота на октаву выше/ниже исходной будет в 2 раза выше/ниже исходной)

        public Mic_Synth()
        {
            InitializeComponent();
            trackBar1.Scroll += trackBar1_Scroll;
            trackBar2.Scroll += trackBar2_Scroll;
            trackBar3.Scroll += trackBar3_Scroll;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox1.Text = "Presets";
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.AddRange(new string[] { "Default", "Preset 1", "Preset 2", "Preset 3", "Preset 4" });
            comboBox1.SelectedIndex = 0;
        }

        public enum Wave // перечисление сигналов
        {
            Sine, Saw, Square, Triangle, Noise, Pure
        }

        // прописываем события Scroll

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = String.Format("Frequency: {0}", trackBar1.Value);
            frequency = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = String.Format("Temperation: {0}", trackBar2.Value);
            temperation = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label3.Text = String.Format("Base tone: {0}", trackBar3.Value);
            base_tone = trackBar3.Value;
        }
        
        // создаём обработчик нажатия на клавишу

        private void Mic_Synth_KeyDown_1(object sender, KeyEventArgs e)
        {
            short[] wave = new short[SAMPLE_RATE]; //  нажатие клавиши генерирует новый короткий массив аудиосэмплов длиной в одну секунду
            byte[] biwave = new byte[SAMPLE_RATE * sizeof(short)]; // тот же массив в бинарном виде
            IEnumerable<Oscillator> oscillators = this.Controls.OfType<Oscillator>(); // создаём коллекцию осцилляторов
            random = new Random();

            // задаём значения переменных

            frequency = trackBar1.Value;
            temperation = trackBar2.Value;
            base_tone = trackBar3.Value;

            // вычисляем частоту сигнала по формуле frequency = base_frequency * base_tone ^ (note_number / temperation)

            switch (e.KeyCode)
            {
                case Keys.Q:
                    frequency *= (float)Math.Pow((float)base_tone, 0 / (float)temperation);
                    break;
                case Keys.W:
                    frequency *= (float)Math.Pow((float)base_tone, 1 / (float)temperation);
                    break;
                case Keys.E:
                    frequency *= (float)Math.Pow((float)base_tone, 2 / (float)temperation);
                    break;
                case Keys.R:
                    frequency *= (float)Math.Pow((float)base_tone, 3 / (float)temperation);
                    break;
                case Keys.T:
                    frequency *= (float)Math.Pow((float)base_tone, 4 / (float)temperation);
                    break;
                case Keys.Y:
                    frequency *= (float)Math.Pow((float)base_tone, 5 / (float)temperation);
                    break;
                case Keys.U:
                    frequency *= (float)Math.Pow((float)base_tone, 6 / (float)temperation);
                    break;
                case Keys.I:
                    frequency *= (float)Math.Pow((float)base_tone, 7 / (float)temperation);
                    break;
                case Keys.O:
                    frequency *= (float)Math.Pow((float)base_tone, 8 / (float)temperation);
                    break;
                case Keys.A:
                    frequency *= (float)Math.Pow((float)base_tone, 9 / (float)temperation);
                    break;
                case Keys.S:
                    frequency *= (float)Math.Pow((float)base_tone, 10 / (float)temperation);
                    break;
                case Keys.D:
                    frequency *= (float)Math.Pow((float)base_tone, 11 / (float)temperation);
                    break;
                case Keys.F:
                    frequency *= (float)Math.Pow((float)base_tone, 12 / (float)temperation);
                    break;
                case Keys.G:
                    frequency *= (float)Math.Pow((float)base_tone, 13 / (float)temperation);
                    break;
                case Keys.H:
                    frequency *= (float)Math.Pow((float)base_tone, 14 / (float)temperation);
                    break;
                case Keys.J:
                    frequency *= (float)Math.Pow((float)base_tone, 15 / (float)temperation);
                    break;
                case Keys.K:
                    frequency *= (float)Math.Pow((float)base_tone, 16 / (float)temperation);
                    break;
                case Keys.Z:
                    frequency *= (float)Math.Pow((float)base_tone, 17 / (float)temperation);
                    break;
                case Keys.C:
                    frequency *= (float)Math.Pow((float)base_tone, 18 / (float)temperation);
                    break;
                case Keys.V:
                    frequency *= (float)Math.Pow((float)base_tone, 19 / (float)temperation);
                    break;
                default:
                    break;
            }

            foreach (Oscillator oscillator in oscillators) // задаём форму сигнала для всех осцилляторов
            {
                // задаём форму сигналов (все примеры, кроме последнего, взяты с: 
                // https://learn.microsoft.com/ru-ru/archive/blogs/dawate/intro-to-audio-programming-part-4-algorithms-for-different-sound-waves-in-c)

                int sample_per_wave_length = (short)(SAMPLE_RATE / frequency);
                short step = (short)((short.MaxValue * 2) / sample_per_wave_length);
                short Sample;

                switch (oscillator.wave)
                {
                    case Wave.Sine:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16((short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i)) / oscillators.Count());
                        };
                        break;
                    case Wave.Saw:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            Sample = -short.MaxValue;
                            for (int j = 0; j < sample_per_wave_length && i < SAMPLE_RATE; j++)
                            {
                                Sample += step;
                                wave[i++] += Convert.ToInt16(Sample / oscillators.Count());
                            }
                            i--;
                        };
                        break;
                    case Wave.Square:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16((short.MaxValue * Math.Sign(Math.Sin(Math.PI * 2 * frequency / SAMPLE_RATE * i))) / oscillators.Count());
                        }
                        break;
                    case Wave.Triangle:
                        Sample = -short.MaxValue;
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            if (Math.Abs(Sample + step) > short.MaxValue)
                            {
                                step = (short)-step;
                            }
                            Sample += step;
                            wave[i] += Convert.ToInt16(Sample / oscillators.Count());
                        }
                        break;
                    case Wave.Noise:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16(random.Next(-short.MaxValue, short.MaxValue) / oscillators.Count());
                        }
                        break;
                    case Wave.Pure:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16(short.MaxValue * Math.Round(Math.Sin(Math.PI * 2 * frequency) / SAMPLE_RATE * i / oscillators.Count()));
                        };
                        break;
                    default:
                        break;
                }
            }

            Buffer.BlockCopy(wave, 0, biwave, 0, wave.Length * sizeof(short)); // копирует сгенерированный массив wave в бинарный массив biwave от начала до конца
            using (MemoryStream memoryStream = new MemoryStream()) // создаём поток
            using (BinaryWriter bw = new BinaryWriter(memoryStream)) // записываем сэмпл в поток
            {
                // поскольку процесс создания wave-file мне особо не понятен, оставляю ссылку на статью, с которой была содран макет:
                // http://soundfile.sapp.org/doc/WaveFormat/

                short blockAlign = BITS_PER_SAMPLE / 8; 
                int SubChunkTwoSize = SAMPLE_RATE * blockAlign; 
                bw.Write(new[] { 'R', 'I', 'F', 'F' });
                bw.Write(36 + SubChunkTwoSize); 
                bw.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                bw.Write(16);
                bw.Write((short)1);
                bw.Write((short)1);
                bw.Write(SAMPLE_RATE);
                bw.Write(SAMPLE_RATE * blockAlign);
                bw.Write(blockAlign);
                bw.Write(BITS_PER_SAMPLE);
                bw.Write(new[] { 'd', 'a', 't', 'a' });
                bw.Write(SubChunkTwoSize);
                bw.Write(biwave);
                memoryStream.Position = 0;
                new SoundPlayer(memoryStream).Play(); // воспроизводим поток 
            }
        }

        // создаём выпадающий список

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // назначаем frequency, temperation, base_tone через trackbar
            // выводим текст
            // меняем сами frequency, temperation, base_tone, т.к. через trackbar эти переменные изменятся только при событии Scroll

            if (comboBox1.SelectedIndex == 0)
            {
                trackBar1.Value = 440;
                frequency = trackBar1.Value;
                label1.Text = String.Format("Frequency: {0}", trackBar1.Value);
                trackBar2.Value = 12;
                temperation = trackBar2.Value;
                label2.Text = String.Format("Temperation: {0}", trackBar2.Value);
                trackBar3.Value = 2;
                base_tone = trackBar3.Value;
                label3.Text = String.Format("Base tone: {0}", trackBar3.Value);
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                trackBar1.Value = 410;
                frequency = trackBar1.Value;
                label1.Text = String.Format("Frequency: {0}", trackBar1.Value);
                trackBar2.Value = 12;
                temperation = trackBar2.Value;
                label2.Text = String.Format("Temperation: {0}", trackBar2.Value);
                trackBar3.Value = 3;
                base_tone = trackBar3.Value;
                label3.Text = String.Format("Base tone: {0}", trackBar3.Value);
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                trackBar1.Value = 428;
                frequency = trackBar1.Value;
                label1.Text = String.Format("Frequency: {0}", trackBar1.Value);
                trackBar2.Value = 19;
                temperation = trackBar2.Value;
                label2.Text = String.Format("Temperation: {0}", trackBar2.Value);
                trackBar3.Value = 2;
                base_tone = trackBar3.Value;
                label3.Text = String.Format("Base tone: {0}", trackBar3.Value);
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                trackBar1.Value = 440;
                frequency = trackBar1.Value;
                label1.Text = String.Format("Frequency: {0}", trackBar1.Value);
                trackBar2.Value = 5;
                temperation = trackBar2.Value;
                label2.Text = String.Format("Temperation: {0}", trackBar2.Value);
                trackBar3.Value = 2;
                base_tone = trackBar3.Value;
                label3.Text = String.Format("Base tone: {0}", trackBar3.Value);
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                trackBar1.Value = 440;
                frequency = trackBar1.Value;
                label1.Text = String.Format("Frequency: {0}", trackBar1.Value);
                trackBar2.Value = 12;
                temperation = trackBar2.Value;
                label2.Text = String.Format("Frequency: {0}", trackBar2.Value);
                trackBar3.Value = 5;
                base_tone = trackBar3.Value;
                label3.Text = String.Format("Base tone: {0}", trackBar3.Value);
            }
        }
    }
}