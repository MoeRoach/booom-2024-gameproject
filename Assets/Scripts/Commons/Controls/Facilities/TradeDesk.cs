// File create date:2024/5/4
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class TradeDesk : BaseFacilityControl  {
	protected override void InitArea() {
		for (var x = 0; x < 5; x++) {
			area.Add(new Square(x, 0));
		}
	}
}