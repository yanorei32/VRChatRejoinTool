struct InputError {
	string message;
	InputErrorLevel inputErrorLevel;

	public string Message { get { return this.message; } }
	public InputErrorLevel InputErrorLevel { get { return this.inputErrorLevel; } }

	public InputError (string message, InputErrorLevel inputErrorLevel) {
		this.message = message;
		this.inputErrorLevel = inputErrorLevel;
	}
}

