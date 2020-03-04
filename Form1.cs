using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HadalisCollider
{
    public partial class Form1 : Form
    {
        private ushort _unkParam;
        private ushort _bankNumber;
        private uint _unkField0x04;
        private uint _particleCount;
        public static List<Particle> Particles = new List<Particle>();

        public Form1()
        {
            InitializeComponent();
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.FocusedItem == null)
                return;
            int index = listView1.FocusedItem.Index;

            Particle particle = Particles[index];
            propertyGrid1.SelectedObject = particle;
        }

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = listView1.HitTest(e.X, e.Y);
                if (hitTestInfo.Item != null)
                {
                    var loc = e.Location;
                    loc.Offset(listView1.Location);
                    contextMenuStrip1.Show(this, loc);
                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Particles|*.ptl";
                openFileDialog.Title = "Open a Particle File!";

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                string fileName = openFileDialog.FileName;
                GetParticles(fileName);
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count <= 0)
                return;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Particles|*.ptl";
                saveFileDialog.Title = "Select a place to save the particle data.";

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                SaveParticles(saveFileDialog.FileName);
                MessageBox.Show("Exported successfully!", "HadalisCollider");
            }
        }

        private void ImportTrackDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Track Data|*.trd";
                openFileDialog.Title = "Choose Track Data to import.";

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                byte[] data = File.ReadAllBytes(openFileDialog.FileName);

                int index = listView1.FocusedItem.Index;
                Particles[index].TrackData = data;

                MessageBox.Show("Data imported successfully!", "HadalisCollider");
            }
        }


        private void ExportTrackDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Track Data|*.trd";
                saveFileDialog.Title = "Select a place to save the track data.";
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                int index = listView1.FocusedItem.Index;
                File.WriteAllBytes(saveFileDialog.FileName, Particles[index].TrackData);

                MessageBox.Show("Exported successfully!", "HadalisCollider");
            }  
        }

        private void GetParticles(string fileName)
        {
            using (BigEndianReader reader = new BigEndianReader(File.OpenRead(fileName)))
            {
                _unkParam = reader.ReadUInt16();
                _bankNumber = reader.ReadUInt16();
                _unkField0x04 = reader.ReadUInt32();
                _particleCount = reader.ReadUInt32();

                int[] particleOffsets = new int[_particleCount];
                for (int i = 0; i < _particleCount; i++)
                {
                    particleOffsets[i] = reader.ReadInt32();
                }

                for (int i = 0; i < _particleCount; i++)
                {
                    int currentOffset = particleOffsets[i];
                    int size = 0;

                    if (_particleCount == i + 1)
                        size = (int)(reader.BaseStream.Length - currentOffset);
                    else
                        size = (particleOffsets[i + 1] - currentOffset);

                    Particles.Add(new Particle(reader.ReadBytes(size, particleOffsets[i])));
                    listView1.Items.Add($"Particle {i + 1}");
                }
            }
        }

        private void SaveParticles(string fileName)
        {
            using (BigEndianWriter writer = new BigEndianWriter(File.OpenWrite(fileName)))
            {
                long[] particleOffsets = new long[_particleCount];

                writer.Write(_unkParam);
                writer.Write(_bankNumber);
                writer.Write(_unkField0x04);
                writer.Write(_particleCount);

                for (int i = 0; i < _particleCount; i++)
                    writer.Write(0xDEADBEEF);
                writer.Write(0xFFFFFFFF);

                for (int i = 0; i < _particleCount; i++)
                {
                    particleOffsets[i] = writer.BaseStream.Position;
                    writer.Write(Particles[i].WriteParticle());
                }

                writer.BaseStream.Position = 0x0C;
                for (int i = 0; i < _particleCount; i++)
                    writer.Write((uint)particleOffsets[i]);
            }
        }
    }
}
