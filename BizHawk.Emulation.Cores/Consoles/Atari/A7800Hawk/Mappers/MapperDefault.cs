﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;
using System;

namespace BizHawk.Emulation.Cores.Atari.A7800Hawk
{
	// Default mapper with no bank switching
	// Just need to keep track of high score bios stuff
	public class MapperDefault : MapperBase
	{
		public override byte ReadMemory(ushort addr)
		{
			if (addr < 0x480)
			{
				// Nothing here
				return 0xFF;
			}
			else if (addr < 0x1800)
			{
				//could be hsbios RAM here
				if (addr >= 0x1000 && Core._hsbios != null)
				{
					return Core._hsram[addr - 0x1000];
				}
				return 0xFF;
			}
			else if (addr < 0x4000)
			{
				// could be either RAM mirror or ROM
				if (addr >= 0x3000 && Core._hsbios != null)
				{
					return Core._hsbios[addr - 0x3000];
				}
				else
				{
					return Core.RAM[0x800 + addr & 0x7FF];
				}
			}
			else
			{
				// cartridge and other OPSYS
				if ((Core._rom.Length >= 0x10000 - addr) && Core.A7800_control_register.Bit(2))
				{
					return Core._rom[Core._rom.Length - (0x10000 - addr)];
				}
				else if (addr >= 0xF000 && !Core.A7800_control_register.Bit(2))
				{
					return Core._bios[addr - 0xF000];
				}
				else
				{
					return 0xFF;
				}
			}
		}

		public override byte PeekMemory(ushort addr)
		{
			return ReadMemory(addr);
		}

		public override void WriteMemory(ushort addr, byte value)
		{
			if (addr < 0x480)
			{
				// Nothing here
			}
			else if (addr < 0x1800)
			{
				//could be hsbios RAM here
				if (addr >= 0x1000 && Core._hsbios != null)
				{
					Core._hsram[addr - 0x1000] = value;
				}
			}
			else if (addr < 0x4000)
			{
				// could be either RAM mirror or ROM
				if (addr >= 0x3000 && Core._hsbios != null)
				{
				}
				else
				{
					Core.RAM[0x800 + addr & 0x7FF] = value;
				}
			}
			else
			{
				// cartridge and other OPSYS
			}
		}

		public override void PokeMemory(ushort addr, byte value)
		{
			WriteMemory(addr, value);
		}


	}
}
