// File create date:2024/5/4
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class WorkDeskRight : BaseFacilityControl  {
	protected override void InitArea() {
		for (var x = 0; x < 4; x++) {
			for (var y = 0; y < 3; y++) {
				if (y == 0 && x < 2) continue;
				area.Add(new Square(x, y));
			}
		}
	}
}