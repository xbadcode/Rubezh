﻿
namespace XFiresecAPI
{
	public class XDirectionInternalState : XBaseInternalState
	{
		public XDirection Direction { get; set; }

		public XDirectionInternalState(XDirection direction)
		{
			Direction = direction;
		}
	}
}