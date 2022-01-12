using System;

namespace VRChatRejoinTool {
	internal readonly struct Visit {
		public DateTime DateTime { get; }

		public Instance Instance { get; }

		public Visit(Instance instance, string dateTime) {
			this.Instance = instance;
			this.DateTime = DateTime.Parse(dateTime);
		}
	}
}

