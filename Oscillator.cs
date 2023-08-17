using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using static synth.Mic_Synth;

namespace synth
{
    public class Oscillator : GroupBox // через groupbox создаём три экземпляра класса Oscillator
    {
        public Oscillator()
        {
            // добавляем кнопки

            this.Controls.Add(new Button()
            {
                Name = "Sine",
                Size = new Size(94, 29),
                Location = new Point(06, 26),
                Text = "Sine" // синусоидальный сигнал
            });

            this.Controls.Add(new Button()
            {
                Name = "Saw",
                Size = new Size(94, 29),
                Location = new Point(106, 26),
                Text = "Saw" // пилообразный сигнал
            });

            this.Controls.Add(new Button()
            {
                Name = "Square",
                Size = new Size(94, 29),
                Location = new Point(206, 26),
                Text = "Square" // прямоугольный сигнал
            });

            this.Controls.Add(new Button()
            {
                Name = "Triangle",
                Size = new Size(94, 29),
                Location = new Point(06, 76),
                Text = "Triangle" // треугольный сигнал
            });

            this.Controls.Add(new Button()
            {
                Name = "Noise",
                Size = new Size(94, 29),
                Location = new Point(106, 76),
                Text = "Noise" // белый шум
            });

            this.Controls.Add(new Button()
            {
                Name = "Pure",
                Size = new Size(94, 29),
                Location = new Point(206, 76),
                Text = "Pure" // мой сигнал
            });

            // через button_click обновляем состояние кнопок в каждом control

            foreach (Control control in this.Controls)
            {
                control.Click += Button_Click;
            }
        }

        public Wave wave { get; private set; } // сигналы

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender; // перехватываем клик на кнопку 
            this.wave = (Wave)Enum.Parse(typeof(Wave), button.Text); // парсим enum и set кнопку
            foreach (Button others in this.Controls.OfType<Button>()) 
            {
                others.UseVisualStyleBackColor = true;
            }
            button.BackColor = Color.AliceBlue; // подсвечиваем нажатую кнопку
        }
    }
}