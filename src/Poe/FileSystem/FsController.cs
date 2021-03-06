using System;
using System.Collections.Generic;
using System.IO;
using PoeHUD.Framework;

namespace PoeHUD.Poe.FileSystem
{
	public class FsController
	{
		private readonly Dictionary<string, int> files;
		private readonly Memory mem;
		public readonly BaseItemTypes BaseItemTypes;
		public readonly ModsDat Mods;
		public readonly StatsDat Stats;
		public readonly TagsDat Tags;

		public FsController(Memory mem)
		{
			this.files = new Dictionary<string, int>();
			this.mem = mem;
			this.BaseItemTypes = new BaseItemTypes(mem, this.FindFile("Data/BaseItemTypes.dat"));
			this.Tags = new TagsDat(mem, this.FindFile("Data/Tags.dat"));
			this.Stats = new StatsDat(mem, this.FindFile("Data/Stats.dat"));
			this.Mods = new ModsDat(mem, this.FindFile("Data/Mods.dat"), Stats, Tags);
		}

		public int FindFile(string name)
		{
			int res;
			if (this.files.TryGetValue(name, out res)) 
				return res;
			FillFsMap();
			if (!this.files.TryGetValue(name, out res))
				throw new FileNotFoundException(name);

			return res;
		}
		private void FillFsMap()
		{
			int num = this.mem.ReadInt(this.mem.BaseAddress + mem.offsets.FileRoot, 8);
			for (int num2 = this.mem.ReadInt(num); num2 != num; num2 = this.mem.ReadInt(num2))
			{
				string text = this.mem.ReadStringU(this.mem.ReadInt(num2 + 8), 512);
				if (text.Contains("."))
					this.files[text] = this.mem.ReadInt(num2 + 12);
			}
		}
	}
}
