﻿using System;
using System.Collections.Generic;
using System.Linq;

using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Atari.A7800Hawk
{
	public partial class A7800Hawk
	{
		private IMemoryDomains MemoryDomains;

		public void SetupMemoryDomains()
		{
			var domains = new List<MemoryDomain>
			{
				new MemoryDomainDelegate(
					"Main RAM",
					RAM.Length,
					MemoryDomain.Endian.Little,
					addr => RAM[addr],
					(addr, value) => RAM[addr] = value,
					1),
				new MemoryDomainDelegate(
					"TIA Registers",
					TIA_regs.Length,
					MemoryDomain.Endian.Little,
					addr => TIA_regs[addr],
					(addr, value) => TIA_regs[addr] = value,
					1),
				new MemoryDomainDelegate(
					"Maria Registers",
					Maria_regs.Length,
					MemoryDomain.Endian.Little,
					addr => Maria_regs[addr],
					(addr, value) => Maria_regs[addr] = value,
					1),
				new MemoryDomainDelegate(
					"6532 Registers",
					regs_6532.Length,
					MemoryDomain.Endian.Little,
					addr => regs_6532[addr],
					(addr, value) => regs_6532[addr] = value,
					1),
				new MemoryDomainDelegate(
					"Ram Block 0",
					0xB0,
					MemoryDomain.Endian.Little,
					addr => RAM[addr-0x840],
					(addr, value) => RAM[addr-0x840] = value,
					1
				),
				new MemoryDomainDelegate(
					"Ram Block 1",
					0xB0,
					MemoryDomain.Endian.Little,
					addr => RAM[addr-0x940],
					(addr, value) => RAM[addr-0x940] = value,
					1
				),
				new MemoryDomainDelegate(
					"System Bus",
					0X10000,
					MemoryDomain.Endian.Little,
					addr => PeekSystemBus(addr),
					(addr, value) => PokeSystemBus(addr, value),
					1
				)
			};

			MemoryDomains = new MemoryDomainList(domains);
			(ServiceProvider as BasicServiceProvider).Register<IMemoryDomains>(MemoryDomains);
		}

		private byte PeekSystemBus(long addr)
		{
			ushort addr2 = (ushort)(addr & 0xFFFF);
			return ReadMemory(addr2);
		}

		private void PokeSystemBus(long addr, byte value)
		{
			ushort addr2 = (ushort)(addr & 0xFFFF);
			WriteMemory(addr2, value);
		}
	}
}
