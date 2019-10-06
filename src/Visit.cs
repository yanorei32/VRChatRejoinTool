using System;

class Visit {
	DateTime dateTime;
	Instance instance;

	public DateTime DateTime {
		get { return this.dateTime; }
	}

	public Instance Instance {
		get { return this.instance; }
	}

	public Visit(Instance instance, string dateTime) {
		this.instance = instance;
		this.dateTime = DateTime.Parse(dateTime);
	}
}

