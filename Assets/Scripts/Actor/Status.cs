using SaveSystem;
using System;

[Serializable]
public class Status {
	public bool Dead;
	public bool Fleeing;
	public bool Sheltered;

	public void Initialize(StatusDto statusDto) {
		Dead = statusDto.Dead;
		Fleeing = statusDto.Fleeing;
		Sheltered = statusDto.Sheltered;
	}
}
