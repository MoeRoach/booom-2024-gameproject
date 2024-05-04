// File create date:2024/5/4
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class PresentDesk : BaseFacilityControl  {
	protected override void InitArea() {
		for (var x = 0; x < 2; x++) {
			for (var y = 0; y < 2; y++) {
				area.Add(new Square(x, y));
			}
		}
	}
}