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

	public override string ToString() {
		return string.Format ("{0} / {1}", Current, Max);
	}
}
