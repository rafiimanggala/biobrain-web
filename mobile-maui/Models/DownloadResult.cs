using System;

namespace BioBrain.Models
{
	public class CheckResult
	{
		public bool IsSuccess;
		public int FailsCount;
		public string Message;

		public static CheckResult Fail(string message = "")
		{
			return new CheckResult{IsSuccess = false, Message = message };
		}
		
		public static CheckResult SuccessWithFails(int failsCount)
		{
			return new CheckResult{IsSuccess = true, FailsCount = failsCount};
		}
		
		public static CheckResult Success()
		{
			return new CheckResult{IsSuccess = true};
		}
	}
}