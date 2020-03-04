using System;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.SysConst
{

	[Serializable]
	public abstract class SysConstTable : Entity<long>
	{
		public const int MaxKeyLength = 50;

		public const int MaxValueLength = 300;

		public const int MaxLanguageZhength = 50;

		[StringLength(50)]
		public string Key
		{
			get;
			set;
		}

		[StringLength(300)]
		public string Value
		{
			get;
			set;
		}

		[StringLength(50)]
		public string LanguageZh
		{
			get;
			set;
		}

		public SysConstType SysConstType
		{
			get;
			set;
		} = SysConstType.EditInGUI;

	}
}
