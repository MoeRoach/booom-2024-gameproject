// File create date:2024/5/4
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class CollectRock2x1 : BaseObjectControl  {
	protected override void InitArea() {
		for (var x = 0; x < 2; x++) {
			area.Add(new Square(x, 0));
		}
	}
}