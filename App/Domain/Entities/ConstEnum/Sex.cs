using System.ComponentModel;

namespace App.Domain.Entities.ConstEnum
{
	public enum Sex
	{
		[Description("未知")]
		unkown,
		[Description("男")]
		male,
		[Description("女")]
		female
	}
}
