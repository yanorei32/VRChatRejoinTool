using System;

class Visit {
    public DateTime DateTime { get; private set; }

    public Instance Instance { get; private set; }

    public Visit(Instance instance, string dateTime) 
    {
		this.Instance = instance;
		this.DateTime = DateTime.Parse(dateTime);
	}
}

