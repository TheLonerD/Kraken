using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;


namespace PKHack
{
	public class BattleBGEffect : RomObject
	{
		private byte[] data = new byte[17];

		public byte Type
		{
			get { return data[2]; }
			set { data[2] = value; }
		}

		public ushort Duration
		{
			get
			{
				return (ushort)(data[0] + (data[1] << 8));
			}
			set
			{
				data[0] = (byte)(value);
				data[1] = (byte)(value >> 8);
			}
		}

		public ushort Frequency
		{
			get
			{
				return (ushort)(data[3] + (data[4] << 8));
			}
			set
			{
				data[3] = (byte)(value);
				data[4] = (byte)(value >> 8);
			}
		}

		public ushort Amplitude
		{
			get
			{
				return (ushort)(data[5] + (data[6] << 8));
			}
			set
			{
				data[5] = (byte)(value);
				data[6] = (byte)(value >> 8);
			}
		}

		public short Compression
		{
			get
			{
				return (short)(data[8] + (data[9] << 8));
			}
			set
			{
				data[8] = (byte)value;
				data[9] = (byte)(value >> 8);
			}
		}

		public ushort FrequencyAcceleration
		{
			get
			{
				return (ushort)(data[10] + (data[11] << 8));
			}
			set
			{
				data[10] = (byte)(value);
				data[11] = (byte)(value >> 8);
			}
		}

		public ushort AmplitudeAcceleration
		{
			get
			{
				return (ushort)(data[12] + (data[13] << 8));
			}
			set
			{
				data[12] = (byte)(value);
				data[13] = (byte)(value >> 8);
			}
		}

		public byte Speed
		{
			get { return data[14]; }
			set { data[14] = value; }
		}

		public short CompressionAcceleration
		{
			get
			{
				return (short)(data[15] + (data[16] << 8));
			}
			set
			{
				data[15] = (byte)value;
				data[16] = (byte)(value >> 8);
			}
		}

		public override void Read(int index)
		{
			Block main = Parent.ReadBlock(0x0AF908 + index * 17);
			for (int i = 0; i < 17; i++)
				data[i] = main.ReadByte();
		}

		public override void Write(int index)
		{
			
		}

		public class Handler : RomObjectHandler
		{
			public override void ReadClass(Rom rom)
			{
				for (int i = 0; i < 135; i++)
				{
					BattleBGEffect e = new BattleBGEffect();
					rom.Add(e);
					e.Read(i);
				}
			}

			public override void WriteClass(Rom rom)
			{
				
			}
		}
	}
}