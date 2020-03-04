using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Collections;

namespace HadalisCollider
{
    public class Particle
    {
        [Category("\rParticle Settings"), Description("Formation of the particles being generated.")]
        public ParticleFormation ParticleType { get; set; }
        [Category("\rParticle Settings"), Description("Unknown.")]
        public ushort TexGroup { get; set; }
        [Category("\rParticle Settings"), Description("Amount of time before the particle is generated.")]
        public ushort GenLife { get; set; }
        [Category("\rParticle Settings"), Description("Amount of frames particle lasts for.")]
        public ushort Life { get; set; }
        [Category("\rParticle Settings"), Description("List of parameters that can be set."), ReadOnly(true)]
        public uint Kind { get; set; }
        [Category("\rParticle Settings"), Description("How much gravity affects the particles.")]
        public float Gravity { get; set; }
        [Category("\rParticle Settings"), Description("How much can particles can bounce off each other")]
        public float Friction { get; set; }
        [Category("\rParticle Settings"), Description("Unknown.")]
        public Vector3 VXYZ { get; set; }
        [Category("\rParticle Settings"), Description("The range of the spread of the particles being generated.")]
        public float Radius { get; set; }
        [Category("\rParticle Settings"), Description("The angle at which the particles are sent.")]
        public float Angle { get; set; }
        [Category("\rParticle Settings"), Description("The randomness at which positions particles will be generated at.")]
        public float Random { get; set; }
        [Category("\rParticle Settings"), Description("The size of the particles being generated.")]
        public float Size { get; set; }
        [Category("\rParticle Settings"), Description("Unknown.")]
        public float Param1 { get; set; }
        [Category("\rParticle Settings"), Description("Unknown.")]
        public float Param2 { get; set; }
        [Category("\rParticle Settings"), Description("Unknown.")]
        public float Param3 { get; set; }
        [Category("\rParticle Settings"), Description("More particle settings I don't know how to parse. Controls color, etc.")]
        public byte[] TrackData { get; set; }

        [Category("Kind Parameters"), Description("")]
        public bool Gravy { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool FricXYZ { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool TornadoL { get; set; }
        
        [Category("Kind Parameters"), Description("")]
        public bool ComtLUT { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool MirrorS { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool MirrorT { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool PrimeNV { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool ImmRND { get; set; }
        [Category("Kind Parameters"), Description("")]
        public TextureInterpolation TexInter { get; set; }
        
        [Category("Kind Parameters"), Description("")]
        public bool ExecPause { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool PNTJOBJ { get; set; }
        [Category("Kind Parameters"), Description("")]
        public int PNTJOBJNO { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool FlipS { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool FlipT { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool Trail { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool DirVEC { get; set; }
        [Category("Kind Parameters"), Description("")]
        public BlendingMode Blending { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool Fog { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool Point { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool Lighting { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool BillboardG { get; set; }
        [Category("Kind Parameters"), Description("")]
        public bool BillboardA { get; set; }

        private bool ReserveBit4;
        private bool ReserveBit11;
        private bool ReserveBit24;
        private bool ReserveBit26;
        private bool ReserveBit27;
        private bool ReserveBit28;
        private bool ReserveBit29;
        private bool ReserveBit30;

        public Particle(byte[] particleData)
        {
            using (BigEndianReader reader = new BigEndianReader(new MemoryStream(particleData)))
            {
                ParticleType = (ParticleFormation)reader.ReadUInt16();
                TexGroup = reader.ReadUInt16();
                GenLife = reader.ReadUInt16();
                Life = reader.ReadUInt16();
                Kind = reader.ReadUInt32();
                Gravity = reader.ReadSingle();
                Friction = reader.ReadSingle();
                VXYZ = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Radius = reader.ReadSingle();
                Angle = reader.ReadSingle();
                Random = reader.ReadSingle();
                Size = reader.ReadSingle();
                Param1 = reader.ReadSingle();
                Param2 = reader.ReadSingle();
                Param3 = reader.ReadSingle();
                ReadKindField();

                long trackSize = reader.BaseStream.Length - reader.BaseStream.Position;
                TrackData = reader.ReadBytes((int)trackSize);
            }
        }
        public void ReadKindField()
        {
            Binary.BitArray bitArray = new Binary.BitArray(BitConverter.GetBytes(Kind));
            bool[] kindField = bitArray.GetBits();

            Gravy = kindField[0];
            FricXYZ = kindField[1];
            TornadoL = kindField[2];
            ReserveBit4 = kindField[3];
            ComtLUT = kindField[4];
            MirrorS = kindField[5];
            MirrorT = kindField[6];
            PrimeNV = kindField[7];
            ImmRND = kindField[8];
            TexInter = (TextureInterpolation)Convert.ToInt32(kindField[9]);
            ReserveBit11 = kindField[10];
            ExecPause = kindField[11];

            BitArray PNTOBJBits = new BitArray(new bool[] { kindField[12], kindField[13], kindField[14] });
            int[] bytes = new int[1];
            PNTOBJBits.CopyTo(bytes, 0);

            PNTJOBJNO = bytes[0];
            PNTJOBJ = kindField[15];
            BillboardG = kindField[16];
            BillboardA = kindField[17];
            FlipS = kindField[18];
            FlipT = kindField[19];
            Trail = kindField[20];
            DirVEC = kindField[21];
            Blending = (BlendingMode)Convert.ToInt32(kindField[22]);
            ReserveBit24 = kindField[23];
            Fog = kindField[24];
            ReserveBit26 = kindField[25];
            ReserveBit27 = kindField[26];
            ReserveBit28 = kindField[27];
            ReserveBit29 = kindField[28];
            ReserveBit30 = kindField[29];
            Point = kindField[30];
            Lighting = kindField[31];
        }

        public void WriteKindField()
        {
            Binary.BitArray bitArray = new Binary.BitArray();

            bitArray.Add(Gravy);
            bitArray.Add(FricXYZ);
            bitArray.Add(TornadoL);
            bitArray.Add(ReserveBit4);
            bitArray.Add(ComtLUT);
            bitArray.Add(MirrorS);
            bitArray.Add(MirrorT);
            bitArray.Add(PrimeNV);
            bitArray.Add(ImmRND);
            bitArray.Add(Convert.ToBoolean(TexInter));
            bitArray.Add(ReserveBit11);
            bitArray.Add(ExecPause);

            BitArray b = new BitArray(new int[] { PNTJOBJNO });
            bitArray.Add(b.Get(0));
            bitArray.Add(b.Get(1));
            bitArray.Add(b.Get(2));

            bitArray.Add(PNTJOBJ);
            bitArray.Add(BillboardG);
            bitArray.Add(BillboardA);
            bitArray.Add(FlipS);
            bitArray.Add(FlipT);
            bitArray.Add(Trail);
            bitArray.Add(DirVEC);
            bitArray.Add(Convert.ToBoolean(Blending));
            bitArray.Add(ReserveBit24);
            bitArray.Add(Fog);
            bitArray.Add(ReserveBit26);
            bitArray.Add(ReserveBit27);
            bitArray.Add(ReserveBit28);
            bitArray.Add(ReserveBit29);
            bitArray.Add(ReserveBit30);
            bitArray.Add(Point);
            bitArray.Add(Lighting);

            Kind = BitConverter.ToUInt32(bitArray.GetBytes(), 0);
        }
    

        public byte[] WriteParticle()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BigEndianWriter writer = new BigEndianWriter(ms))
                {
                    writer.Write((ushort)ParticleType);
                    writer.Write(TexGroup);
                    writer.Write(GenLife);
                    writer.Write(Life);
                    WriteKindField();
                    writer.Write(Kind);
                    writer.Write(Gravity);
                    writer.Write(Friction);
                    writer.Write(VXYZ.X);
                    writer.Write(VXYZ.Y);
                    writer.Write(VXYZ.Z);
                    writer.Write(Radius);
                    writer.Write(Angle);
                    writer.Write(Random);
                    writer.Write(Size);
                    writer.Write(Param1);
                    writer.Write(Param2);
                    writer.Write(Param3);
                    writer.Write(TrackData);
                }

                return ms.ToArray();
            }
        }
    }
    public enum ParticleFormation
    {
        Disc = 0x00,
        Line = 0x01,
        Tornado = 0x02,
        DiscCT = 0x03,
        DiscCD = 0x04,
        Rect = 0x05,
        Cone = 0x06,
        Cylinder = 0x07,
        Sphere = 0x08
    }

    public enum TextureInterpolation
    {
        Near = 0x00,
        Linear = 0x01,
        //Reserve = 0x02;
    }

    public enum BlendingMode
    {
        Normal = 0x00,
        Add = 0x01
    }
}
