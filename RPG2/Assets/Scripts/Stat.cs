public class Stat {
	public int Current { get; set; }
	public int Max { get; set; }

	public Stat(int setMax) {
		Max = setMax;
		Current = setMax;
	}

	public bool IsCurrentZero() {
		return Current <= 0;
	}

	public float GetRatio() {
		return (float)Current / (float)Max;
	}
}
