﻿using BizHawk.Emulation.Common;
using System;

namespace BizHawk.Emulation.Cores.Atari.A7800Hawk
{
	public partial class A7800Hawk : IEmulator 
	{
		public IEmulatorServiceProvider ServiceProvider { get; }

		public ControllerDefinition ControllerDefinition => _controllerDeck.Definition;

		//Maria related variables
		public int cycle;
		public int cpu_cycle;
		public bool cpu_is_haltable;
		public bool cpu_is_halted;
		public bool cpu_halt_pending;
		public bool cpu_resume_pending;

		// there are 4 maria cycles in a CPU cycle (fast access, both NTSC and PAL)
		// if the 6532 or TIA are accessed (PC goes to one of those addresses) the next access will be slower by 1/2 a CPU cycle
		// i.e. it will take 6 Maria cycles instead of 4
		public bool slow_access = false;

		public void FrameAdvance(IController controller, bool render, bool rendersound)
		{
			if (_tracer.Enabled)
			{
				cpu.TraceCallback = s => _tracer.Put(s);
			}
			else
			{
				cpu.TraceCallback = null;
			}

			_frame++;

			if (controller.IsPressed("Power"))
			{
				// it seems that theMachine.Reset() doesn't clear ram, etc
				// this should leave hsram intact but clear most other things
				HardReset();
			}

			if (_islag)
			{
				_lagcount++;
			}

			// read the controller state here for now
			GetControllerState(controller);

			maria.RunFrame();
		}

		public void RunCPUCycle()
		{
			cpu_cycle++;
			tia._hsyncCnt++;

			if (cpu_cycle <= (2 + (slow_access ? 1 : 0)))
			{
				cpu_is_haltable = true;
			} else
			{
				cpu_is_haltable = false;
			}

			// the time a cpu cycle takes depends on the status of the address bus
			// any address in range of the TIA or m6532 takes an extra cycle to complete
			if (cpu_cycle == (4 + (slow_access ? 2 : 0)))
			{
				if (!cpu_is_halted)
				{
					cpu.ExecuteOne();
				}
				else
				{
					// we still want to keep track of CPU time even if it is halted, so increment the counter here
					// The basic 6502 has no halt state, this feature is specific to SALLY
					cpu.TotalExecutedCycles++;
				}

				cpu_cycle = 0;

				if (cpu_halt_pending)
				{
					cpu_halt_pending = false;
					cpu_is_halted = true;
				}

				if (cpu_resume_pending)
				{
					cpu_resume_pending = false;
					cpu_is_halted = false;
				}
			}

			// determine if the next access will be fast or slow
			if (cpu.PC < 0x0400)
			{
				if ((cpu.PC & 0xFF) < 0x20)
				{
					if ((A7800_control_register & 0x1) == 0 && (cpu.PC < 0x20))
					{
						slow_access = false;
					}
					else
					{
						slow_access = true;
					}
				}
				else if (cpu.PC < 0x300)
				{
					slow_access = true;
				}
				else
				{
					slow_access = false;
				}
			}
		}

		private void GetControllerState(IController controller)
		{
			InputCallbacks.Call();

			ushort port1 = _controllerDeck.ReadPort1(controller);

			ushort port2 = _controllerDeck.ReadPort2(controller);
		}

		public int Frame => _frame;

		public string SystemId => "A7800"; 

		public bool DeterministicEmulation { get; set; }

		public void ResetCounters()
		{
			_frame = 0;
			_lagcount = 0;
			_islag = false;
		}

		public CoreComm CoreComm { get; }

		public void Dispose()
		{
			maria = null;
			tia = null;
			m6532 = null;
		}
	}
}
